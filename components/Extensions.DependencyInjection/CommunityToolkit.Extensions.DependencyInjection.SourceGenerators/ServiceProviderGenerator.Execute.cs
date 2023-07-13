// ------------------------------------------------------
// Copyright (C) Microsoft. All rights reserved.
// ------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Helpers;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators;

/// <inheritdoc/>
partial class ServiceProviderGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Helpers to generate the service registrations.
    /// </summary>
    private static class Execute
    {
        /// <summary>
        /// A shared annotation used to track arguments.
        /// </summary>
        public static readonly SyntaxAnnotation ArgumentAnnotation = new();

        /// <summary>
        /// Checks whether the input <see cref="SyntaxNode"/> is a valid target for generation.
        /// </summary>
        /// <param name="syntaxNode">The input <see cref="SyntaxNode"/> instance to analyze.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>Whether <paramref name="syntaxNode"/> is a valid generation target.</returns>
        public static bool IsSyntaxTarget(SyntaxNode syntaxNode, CancellationToken token)
        {
            return syntaxNode.IsKind(SyntaxKind.MethodDeclaration);
        }

        /// <summary>
        /// Gathers the info on all registered singleton services.
        /// </summary>
        /// <param name="context">The current <see cref="GeneratorAttributeSyntaxContext"/> instance with the provided info.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>The gathered info for the current service collection, if available.</returns>
        public static ServiceCollectionInfo? GetSingletonInfo(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            return GetInfo(ServiceRegistrationKind.Singleton, context, token);
        }

        /// <summary>
        /// Gathers the info on all registered transient services.
        /// </summary>
        /// <param name="context">The current <see cref="GeneratorAttributeSyntaxContext"/> instance with the provided info.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>The gathered info for the current service collection, if available.</returns>
        public static ServiceCollectionInfo? GetTransientInfo(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            return GetInfo(ServiceRegistrationKind.Transient, context, token);
        }

        /// <summary>
        /// Gathers the info on all registered services.
        /// </summary>
        /// <param name="registrationKind">The registration kind to use.</param>
        /// <param name="context">The current <see cref="GeneratorAttributeSyntaxContext"/> instance with the provided info.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>The gathered info for the current service collection, if available.</returns>
        private static ServiceCollectionInfo? GetInfo(ServiceRegistrationKind registrationKind, GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            // Ensure that the target syntax node is valid:
            //   - It has to be a method declaration
            //   - The method has a single parameter of type Microsoft.Extensions.DependencyInjection.IServiceCollection
            //   - The method returns void or Microsoft.Extensions.DependencyInjection.IServiceCollection
            if (context.TargetNode is not MethodDeclarationSyntax methodDeclaration ||
                context.TargetSymbol is not IMethodSymbol { Parameters: [{ } parameterSymbol] } methodSymbol ||
                !parameterSymbol.Type.HasFullyQualifiedMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection") ||
                !(methodSymbol.ReturnsVoid || methodSymbol.ReturnType.HasFullyQualifiedMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection")))
            {
                return null;
            }

            // Gather the basic method info
            HierarchyInfo hierarchy = HierarchyInfo.From(methodSymbol.ContainingType);
            string methodName = methodSymbol.Name;

            token.ThrowIfCancellationRequested();

            using ImmutableArrayBuilder<ushort> methodModifiers = ImmutableArrayBuilder<ushort>.Rent();

            // Gather all method modifiers
            foreach (SyntaxToken modifier in methodDeclaration.Modifiers)
            {
                methodModifiers.Add((ushort)modifier.Kind());
            }

            token.ThrowIfCancellationRequested();

            using ImmutableArrayBuilder<RegisteredServiceInfo> serviceInfo = ImmutableArrayBuilder<RegisteredServiceInfo>.Rent();

            // Gather all registered services
            foreach (AttributeData attributeData in context.Attributes)
            {
                token.ThrowIfCancellationRequested();

                if (attributeData.ConstructorArguments is [
                    { Kind: TypedConstantKind.Type, Value: INamedTypeSymbol { InstanceConstructors: [IMethodSymbol implementationConstructor, ..] } implementationType },
                    { Kind: TypedConstantKind.Array, Values: ImmutableArray<TypedConstant> serviceTypes }])
                {
                    // Gather all dependent services for the implementation type
                    ImmutableArray<string> constructorArgumentTypes = ImmutableArray.CreateRange(
                        items: implementationConstructor.Parameters,
                        selector: static parameter => parameter.Type.GetFullyQualifiedName());

                    string implementationTypeName = implementationType.GetFullyQualifiedName();
                    ImmutableArray<string> serviceTypeNames = ImmutableArray<string>.Empty;

                    // If there are no specified service types, use the implementation type itself as service type. This is pretty
                    // common for eg. factory types, which don't need to be mocked and are just registered as concrete types.
                    if (serviceTypes.IsEmpty)
                    {
                        serviceTypeNames = ImmutableArray.Create(implementationTypeName);                        
                    }
                    else
                    {
                        using ImmutableArrayBuilder<string> builder = ImmutableArrayBuilder<string>.Rent();

                        // Otherwise, simply gather all service types for the current service registration
                        foreach (TypedConstant serviceType in serviceTypes)
                        {
                            if (serviceType is { Kind: TypedConstantKind.Type, Value: INamedTypeSymbol serviceTypeSymbol })
                            {
                                builder.Add(serviceTypeSymbol.GetFullyQualifiedName());
                            }
                        }

                        serviceTypeNames = builder.ToImmutable();
                    }

                    // Create the model fully describing the current service registration
                    serviceInfo.Add(new RegisteredServiceInfo(
                        RegistrationKind: registrationKind,
                        ImplementationFullyQualifiedTypeName: implementationTypeName,
                        ServiceFullyQualifiedTypeNames: serviceTypeNames,
                        RequiredServiceFullyQualifiedTypeNames: constructorArgumentTypes));
                }
            }

            ServiceProviderMethodInfo methodInfo = new(
                hierarchy,
                methodName,
                parameterSymbol.Name,
                methodSymbol.ReturnsVoid,
                methodModifiers.ToImmutable());

            return new(methodInfo, serviceInfo.ToImmutable());
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the gathered info.
        /// </summary>
        /// <param name="info">The input <see cref="ServiceCollectionInfo"/> instance with the services info.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the gathered info.</returns>
        public static CompilationUnitSyntax GetSyntax(ServiceCollectionInfo info)
        {
            using ImmutableArrayBuilder<StatementSyntax> registrationStatements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            foreach (RegisteredServiceInfo serviceInfo in info.Services)
            {
                // The first service type always acts as "main" registration, and should always be present
                if (serviceInfo.ServiceFullyQualifiedTypeNames.AsSpan() is not [string rootServiceTypeName, ..ReadOnlySpan<string> dependentServiceTypeNames])
                {
                    continue;
                }

                using ImmutableArrayBuilder<ArgumentSyntax> constructorArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                // Prepare the dependent services for the implementation type
                foreach (string constructorServiceType in serviceInfo.RequiredServiceFullyQualifiedTypeNames)
                {
                    // Create an argument for each constructor parameter:
                    //
                    // global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredServices<SERVICE_TYPE>(<PARAMETER_NAME>);
                    constructorArguments.Add(
                        Argument(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions"),
                                    GenericName(Identifier("GetRequiredService"))
                                    .AddTypeArgumentListArguments(IdentifierName(constructorServiceType))))
                            .AddArgumentListArguments(Argument(IdentifierName(info.Method.ServiceCollectionParameterName))))
                        .WithAdditionalAnnotations(ArgumentAnnotation));
                }

                // Prepare the method name, either AddSingleton or AddTransient
                string registrationMethod = $"Add{serviceInfo.RegistrationKind}";

                // Special case when the service is a singleton and no dependent services are present, just use eager instantiation instead:
                //
                // global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton(<PARAMETER_NAME>, typeof(<ROOT_SERVICE_TYPE>), new <IMPLEMENTATION_TYPE>());
                if (serviceInfo.RegistrationKind == ServiceRegistrationKind.Singleton && constructorArguments.Span.IsEmpty)
                {
                    registrationStatements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions"),
                                    IdentifierName("AddSingleton")))
                            .AddArgumentListArguments(
                                Argument(IdentifierName(info.Method.ServiceCollectionParameterName)),
                                Argument(TypeOfExpression(IdentifierName(rootServiceTypeName))),
                                Argument(
                                    ObjectCreationExpression(IdentifierName(serviceInfo.ImplementationFullyQualifiedTypeName))
                                    .WithArgumentList(ArgumentList())))));
                }
                else
                {
                    // Register the main implementation type when at least a dependent service is needed:
                    //
                    // global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.<REGISTRATION_METHOD>(<PARAMETER_NAME>, typeof(<ROOT_SERVICE_TYPE>), static services => new <IMPLEMENTATION_TYPE>(<CONSTRUCTOR_ARGUMENTS>));
                    registrationStatements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions"),
                                    IdentifierName(registrationMethod)))
                            .AddArgumentListArguments(
                                Argument(IdentifierName(info.Method.ServiceCollectionParameterName)),
                                Argument(TypeOfExpression(IdentifierName(rootServiceTypeName))),
                                Argument(
                                    SimpleLambdaExpression(Parameter(Identifier("services")))
                                    .AddModifiers(Token(SyntaxKind.StaticKeyword))
                                    .WithExpressionBody(
                                        ObjectCreationExpression(IdentifierName(serviceInfo.ImplementationFullyQualifiedTypeName))
                                        .AddArgumentListArguments(constructorArguments.ToArray()))))));
                }

                // Register all secondary services, if any
                foreach (string dependentServiceType in dependentServiceTypeNames)
                {
                    // Register the main implementation type:
                    //
                    // global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.<REGISTRATION_METHOD>(<PARAMETER_NAME>, typeof(<DEPENDENT_SERVICE_TYPE>), static services => global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredServices<ROOT_SERVICE_TYPE>(services));
                    registrationStatements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions"),
                                    IdentifierName(registrationMethod)))
                            .AddArgumentListArguments(
                                Argument(IdentifierName(info.Method.ServiceCollectionParameterName)),
                                Argument(TypeOfExpression(IdentifierName(dependentServiceType))),
                                Argument(
                                    SimpleLambdaExpression(Parameter(Identifier("services")))
                                    .AddModifiers(Token(SyntaxKind.StaticKeyword))
                                    .WithExpressionBody(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions"),
                                                GenericName(Identifier("GetRequiredService"))
                                                .AddTypeArgumentListArguments(IdentifierName(rootServiceTypeName))))
                                        .AddArgumentListArguments(Argument(IdentifierName("services"))))))));
                }
            }

            // Return the input service provider, if needed:
            //
            // return <PARAMETER_NAME>;
            if (!info.Method.ReturnsVoid)
            {
                registrationStatements.Add(ReturnStatement(IdentifierName(info.Method.ServiceCollectionParameterName)).WithLeadingTrivia(Comment(" ")));
            }

            // Prepare the return type: either void or IServiceCollection
            TypeSyntax returnType = info.Method.ReturnsVoid switch
            {
                true => PredefinedType(Token(SyntaxKind.VoidKeyword)),
                false => IdentifierName("global::Microsoft.Extensions.DependencyInjection.IServiceCollection")
            };

            // Get the service collection configuration method declaration:
            //
            // /// <inheritdoc/>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // <MODIFIERS> <RETURN_TYPE> <METHOD_NAME>(global::Microsoft.Extensions.DependencyInjection.IServiceCollection <PARAMETER_NAME>)
            // {
            //     <REGISTRATION_STATEMENTS>
            // }
            MethodDeclarationSyntax configureServicesMethodDeclaration =
                MethodDeclaration(returnType, Identifier(info.Method.MethodName))
                .AddModifiers(info.Method.Modifiers.AsImmutableArray().Select(static m => Token((SyntaxKind)m)).ToArray())
                .AddParameterListParameters(
                    Parameter(Identifier(info.Method.ServiceCollectionParameterName))
                    .WithType(IdentifierName("global::Microsoft.Extensions.DependencyInjection.IServiceCollection")))
                .AddBodyStatements(registrationStatements.ToArray())
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ServiceProviderGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ServiceProviderGenerator).Assembly.GetName().Version.ToString())))))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithLeadingTrivia(Comment("/// <inheritdoc/>"));

            // Create the compilation unit with the generated members:
            CompilationUnitSyntax compilationUnit = info.Method.Hierarchy.GetCompilationUnit(ImmutableArray.Create<MemberDeclarationSyntax>(configureServicesMethodDeclaration));

            // Format the annotations
            FormatArgumentNodes(ref compilationUnit, info.Method.Hierarchy.Hierarchy.AsSpan().Length + 3);

            return compilationUnit;
        }

        /// <summary>
        /// Formats the arguments with a given annotation adding leading whitespace.
        /// </summary>
        /// <param name="compilationUnit">The target <see cref="CompilationUnitSyntax"/> instance to modify.</param>
        /// <param name="indentationLevel">The indentation level to format arguments for.</param>
        private static void FormatArgumentNodes(ref CompilationUnitSyntax compilationUnit, int indentationLevel)
        {
            string whitespace = new(' ', indentationLevel * 4);

            while (compilationUnit.GetAnnotatedNodes(ArgumentAnnotation).FirstOrDefault() is SyntaxNode annotatedNode)
            {
                // For each argument node, remove the annotation and add a CRLF and the leading whitespace. We can't
                // loop over the annotated nodes on the target unit directly, as it's changed for every iteration.
                compilationUnit = compilationUnit.ReplaceNode(
                    annotatedNode,
                    annotatedNode.WithoutAnnotations(ArgumentAnnotation).WithLeadingTrivia(CarriageReturnLineFeed, Whitespace(whitespace)));
            }
        }
    }
}