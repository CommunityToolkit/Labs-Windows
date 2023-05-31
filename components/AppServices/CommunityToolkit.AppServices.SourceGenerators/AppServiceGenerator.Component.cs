// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;
using CommunityToolkit.AppServices.SourceGenerators.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <inheritdoc/>
partial class AppServiceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Generator logic for app service components.
    /// </summary>
    private static class Component
    {
        /// <summary>
        /// Gathers info on a component.
        /// </summary>
        /// <param name="symbol">The input <see cref="INamedTypeSymbol"/> instance to inspect.</param>
        /// <param name="token">The cancellation token for the operation.</param>
        /// <returns>The interface type and service name, or <see langword="default"/>.</returns>
        public static (INamedTypeSymbol? ServiceSymbol, string? ServiceName) GetInfo(INamedTypeSymbol symbol, CancellationToken token)
        {
            foreach (INamedTypeSymbol interfaceSymbol in symbol.Interfaces)
            {
                token.ThrowIfCancellationRequested();

                if (interfaceSymbol.TryGetAppServicesNameFromAttribute(out string? serviceName))
                {
                    return (interfaceSymbol, serviceName);
                }
            }

            return default;
        }

        /// <summary>
        /// Gets a <see cref="ConstructorDeclarationSyntax"/> registering all available service endpoints.
        /// </summary>
        /// <param name="hierarchy">The input hierarchy for the component.</param>
        /// <param name="info">The app service info.</param>
        /// <returns>The <see cref="ConstructorDeclarationSyntax"/> for the component.</returns>
        public static ConstructorDeclarationSyntax GetSyntax(HierarchyInfo hierarchy, AppServiceInfo info)
        {
            using ImmutableArrayBuilder<StatementSyntax> registrationStatements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            // Prepare the endpoint registrations
            foreach (MethodInfo methodInfo in info.Methods)
            {
                if (methodInfo.Parameters.IsEmpty)
                {
                    using ImmutableArrayBuilder<ArgumentSyntax> endpointArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                    if (methodInfo.HasCustomValueSetSerializer)
                    {
                        // This adds the serializer expression:
                        //
                        // new <SERIALIZER_TYPE>()
                        endpointArguments.Add(Argument(ObjectCreationExpression(IdentifierName(methodInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()));
                    }

                    // Add the common arguments:
                    //
                    // <METHOD_NAME>, "<METHOD_NAME>"
                    endpointArguments.Add(Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(methodInfo.MethodName))));
                    endpointArguments.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(methodInfo.MethodName))));

                    // This creates a registration for a parameterless endpoint:
                    //
                    // base.RegisterEndpoint(<ARGUMENTS>);
                    registrationStatements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    BaseExpression(),
                                    IdentifierName("RegisterEndpoint")))
                            .AddArgumentListArguments(endpointArguments.ToArray())));
                }
                else
                {
                    using ImmutableArrayBuilder<StatementSyntax> endpointStatements = ImmutableArrayBuilder<StatementSyntax>.Rent();
                    using ImmutableArrayBuilder<ArgumentSyntax> endpointArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                    foreach (ParameterInfo parameterInfo in methodInfo.Parameters)
                    {
                        if (parameterInfo.Type.HasFlag(ParameterOrReturnType.IProgressOfT))
                        {
                            using ImmutableArrayBuilder<ArgumentSyntax> progressArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                            if (parameterInfo.HasCustomValueSetSerializer)
                            {
                                // This adds the serializer expression, like above:
                                //
                                // new <SERIALIZER_TYPE>()
                                progressArguments.Add(Argument(ObjectCreationExpression(IdentifierName(parameterInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()));
                            }

                            // Add the common argument:
                            //
                            // out <PROGRESS_TYPE> <PROGRESS>
                            progressArguments.Add(
                                Argument(
                                    DeclarationExpression(
                                        parameterInfo.GetSyntax(),
                                        SingleVariableDesignation(Identifier(parameterInfo.Name))))
                                .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword)));

                            // This code prepares an argument retrieval statement for an IProgress<T> parameter:
                            //
                            // parameters.GetProgress(<ARGUMENTS>);
                            endpointStatements.Add(
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("parameters"),
                                            IdentifierName("GetProgress")))
                                    .AddArgumentListArguments(progressArguments.ToArray())));
                        }
                        else if (parameterInfo.Type.HasFlag(ParameterOrReturnType.CancellationToken))
                        {
                            // This code prepares an argument retrieval statement for a CancellationToken parameter:
                            //
                            // parameters.GetCancellationToken(out <CANCELLATION_TOKEN_TYPE> <CANCELLATION_TOKEN>);
                            endpointStatements.Add(
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("parameters"),
                                            IdentifierName("GetCancellationToken")))
                                    .AddArgumentListArguments(
                                        Argument(
                                            DeclarationExpression(
                                                parameterInfo.GetSyntax(),
                                                SingleVariableDesignation(Identifier(parameterInfo.Name))))
                                        .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword)))));
                        }
                        else
                        {
                            using ImmutableArrayBuilder<ArgumentSyntax> parameterRetrievalArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                            if (parameterInfo.HasCustomValueSetSerializer)
                            {
                                // This adds the serializer expression, like above:
                                //
                                // new <SERIALIZER_TYPE>()
                                parameterRetrievalArguments.Add(Argument(ObjectCreationExpression(IdentifierName(parameterInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()));
                            }

                            // Add the common arguments:
                            //
                            // out <PARAMETER_TYPE> <PARAMETER>, "<PARAMETER_NAME>"
                            parameterRetrievalArguments.Add(
                                Argument(
                                    DeclarationExpression(
                                        parameterInfo.GetSyntax(),
                                        SingleVariableDesignation(Identifier(parameterInfo.Name))))
                                .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword)));
                            parameterRetrievalArguments.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(parameterInfo.Name))));

                            // This code prepares an argument retrieval statement for a generic named parameter with a custom serializer:
                            //
                            // parameters.GetParameter(<ARGUMENTS>);
                            endpointStatements.Add(
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("parameters"),
                                            IdentifierName("GetParameter")))
                                    .AddArgumentListArguments(parameterRetrievalArguments.ToArray())));
                        }

                        // Also create the parameter argument syntax
                        endpointArguments.Add(Argument(IdentifierName(parameterInfo.Name)));
                    }

                    // Add the await and optional return statement for the endpoint stub. This generates code as follows:
                    if (methodInfo.ReturnType == ParameterOrReturnType.Task)
                    {
                        // await <METHOD_NAME>(<PARAMETERS>);
                        endpointStatements.Add(
                            ExpressionStatement(
                                AwaitExpression(
                                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(methodInfo.MethodName)))
                                    .AddArgumentListArguments(endpointArguments.ToArray()))));
                    }
                    else
                    {
                        // return await <METHOD_NAME>(<PARAMETERS>);
                        endpointStatements.Add(
                            ReturnStatement(
                                AwaitExpression(
                                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(methodInfo.MethodName)))
                                    .AddArgumentListArguments(endpointArguments.ToArray()))));
                    }

                    using ImmutableArrayBuilder<ArgumentSyntax> endpointRegistrationArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                    // Add the return type custom serializer, if needed
                    if (methodInfo.HasCustomValueSetSerializer)
                    {
                        endpointRegistrationArguments.Add(Argument(ObjectCreationExpression(IdentifierName(methodInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()));
                    }

                    // Add the fixed arguments that are common for all cases.
                    // This creates a registration for an endpoint with parameters:
                    //
                    // ..., async parameters =>
                    // {
                    //     <GET_PARAMETERS_STATEMENTS>
                    //     <RETURN_AWAIT_STATEMENT>
                    // }, "<METHOD_NAME>"
                    endpointRegistrationArguments.Add(Argument(
                        SimpleLambdaExpression(Parameter(Identifier("parameters")))
                        .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword))
                        .AddBlockStatements(endpointStatements.ToArray())));
                    endpointRegistrationArguments.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(methodInfo.MethodName))));

                    // This creates a registration for an endpoint with parameters:
                    //
                    // base.RegisterEndpoint(<ARGUMENTS>);
                    registrationStatements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    BaseExpression(),
                                    IdentifierName("RegisterEndpoint")))
                                .AddArgumentListArguments(endpointRegistrationArguments.ToArray())));
                }
            }


            // This code produces the constructor declaration as follows:
            //
            // /// <summary>Creates a new <see cref="<TYPE_NAME>"/> instance.</summary>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // public <TYPE_NAME>()
            //     : base("<APP_SERVICE_NAME>")
            // {
            //     <REGISTRATION_STATEMENTS>
            // }
            return
                ConstructorDeclaration(Identifier(hierarchy.MetadataName))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithInitializer(
                    ConstructorInitializer(
                        SyntaxKind.BaseConstructorInitializer,
                        ArgumentList(SingletonSeparatedList(
                            Argument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal(info.AppServiceName)))))))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(AppServiceGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(AppServiceGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment($"/// <summary>Creates a new <see cref=\"{hierarchy.MetadataName}\"/> instance.</summary>")), SyntaxKind.OpenBracketToken, TriviaList())),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .AddBodyStatements(registrationStatements.ToArray());
        }
    }
}
