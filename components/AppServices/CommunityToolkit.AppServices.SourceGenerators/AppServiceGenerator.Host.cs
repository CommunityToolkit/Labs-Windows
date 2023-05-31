// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;
using CommunityToolkit.AppServices.SourceGenerators.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <inheritdoc/>
partial class AppServiceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Generator logic for app service hosts.
    /// </summary>
    private static class Host
    {
        /// <summary>
        /// Gets the app service name for a host interface.
        /// </summary>
        /// <param name="attributeData">The data for the attribute over the service interface.</param>
        /// <returns>The service name, or <see langword="null"/>.</returns>
        public static bool TryGetAppServiceName(AttributeData attributeData, [NotNullWhen(true)] out string? appServiceName)
        {
            if (attributeData.ConstructorArguments[0].Value is string { Length: > 0 } name)
            {
                appServiceName = name;

                return true;
            }

            appServiceName = null;

            return false;
        }

        /// <summary>
        /// Gets a <see cref="ConstructorDeclarationSyntax"/> registering the service name.
        /// </summary>
        /// <param name="hierarchy">The input hierarchy for the host.</param>
        /// <param name="info">The app service info.</param>
        /// <returns>The <see cref="ConstructorDeclarationSyntax"/> for the host.</returns>
        public static ConstructorDeclarationSyntax GetConstructorSyntax(HierarchyInfo hierarchy, AppServiceInfo info)
        {

            // This code produces the constructor declaration as follows:
            //
            // /// <summary>Creates a new <see cref="<TYPE_NAME>"/> instance.</summary>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // public <TYPE_NAME>()
            //     : base("<APP_SERVICE_NAME>")
            // {
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
                .WithBody(Block());
        }

        /// <summary>
        /// Gets a <see cref="MethodDeclarationSyntax"/> collection with all host methods for a given service.
        /// </summary>
        /// <param name="info">The app service info.</param>
        /// <returns>The <see cref="MethodDeclarationSyntax"/> collection for the host.</returns>
        public static ImmutableArray<MethodDeclarationSyntax> GetMethodDeclarationsSyntax(AppServiceInfo info)
        {
            using ImmutableArrayBuilder<MethodDeclarationSyntax> methodDeclarations = ImmutableArrayBuilder<MethodDeclarationSyntax>.Rent();

            // Prepare the method declarations
            foreach (MethodInfo methodInfo in info.Methods)
            {
                using ImmutableArrayBuilder<StatementSyntax> requestStatements = ImmutableArrayBuilder<StatementSyntax>.Rent();
                using ImmutableArrayBuilder<ParameterSyntax> methodParameters = ImmutableArrayBuilder<ParameterSyntax>.Rent();

                // This code produces a statement creating the request:
                //
                // var request = base.CreateAppServiceRequest();
                requestStatements.Add(
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())))
                        .AddVariables(
                            VariableDeclarator(Identifier("request"))
                            .WithInitializer(EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        BaseExpression(),
                                        IdentifierName("CreateAppServiceRequest"))))))));

                // Add the parameters and the optional progress, if any
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
                        // <PARAMETER>
                        progressArguments.Add(Argument(IdentifierName(parameterInfo.Name)));

                        // This produces the following expression:
                        //
                        // request = request.WithProgress(<PARAMETER>);
                        requestStatements.Add(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("request"),
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("request"),
                                            IdentifierName("WithProgress")))
                                    .AddArgumentListArguments(progressArguments.ToArray()))));
                    }
                    else if (parameterInfo.Type.HasFlag(ParameterOrReturnType.CancellationToken))
                    {
                        // This produces the following expression:
                        //
                        // request = request.WithCancellationToken(<PARAMETER>);
                        requestStatements.Add(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("request"),
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("request"),
                                            IdentifierName("WithCancellationToken")))
                                    .AddArgumentListArguments(
                                        Argument(IdentifierName(parameterInfo.Name))))));
                    }
                    else
                    {
                        using ImmutableArrayBuilder<ArgumentSyntax> parameterArguments = ImmutableArrayBuilder<ArgumentSyntax>.Rent();

                        if (parameterInfo.HasCustomValueSetSerializer)
                        {
                            // This adds the serializer expression:
                            //
                            // new <SERIALIZER_TYPE>()
                            parameterArguments.Add(Argument(ObjectCreationExpression(IdentifierName(parameterInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()));
                        }

                        // Add the common arguments:
                        //
                        // <PARAMETER>, "<PARAMETER_NAME>"
                        parameterArguments.Add(Argument(IdentifierName(parameterInfo.Name)));
                        parameterArguments.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(parameterInfo.Name))));

                        // This produces the following expression:
                        //
                        // request = request.WithParameter(<ARGUMENTS>);
                        requestStatements.Add(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("request"),
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("request"),
                                            IdentifierName("WithParameter")))
                                    .AddArgumentListArguments(parameterArguments.ToArray()))));
                    }

                    // Also prepare the method parameter
                    methodParameters.Add(
                        Parameter(Identifier(parameterInfo.Name))
                        .WithType(parameterInfo.GetSyntax()));
                }

                TypeSyntax returnType;

                // Send the request, with the following code:
                if (methodInfo.ReturnType == ParameterOrReturnType.Task)
                {
                    // global::System.Threading.Tasks.Task
                    returnType = IdentifierName(Identifier("global::System.Threading.Tasks.Task"));

                    // return request.SendAndWaitForResultAsync();
                    requestStatements.Add(
                        ReturnStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("request"),
                                    IdentifierName(Identifier("SendAndWaitForResultAsync"))))));
                }
                else
                {
                    // global::System.Threading.Tasks.Task<<RETURN_TYPE>>
                    returnType =
                        GenericName(Identifier("global::System.Threading.Tasks.Task"))
                        .AddTypeArgumentListArguments(ParameterInfo.GetSyntax(methodInfo.ReturnType, methodInfo.FullyQualifiedReturnTypeName));

                    // Prepare the serializer, if available:
                    //
                    // new <SERIALIZER_TYPE>(), or nothing
                    ArgumentSyntax[] arguments = methodInfo.HasCustomValueSetSerializer switch
                    {
                        true => new[] { Argument(ObjectCreationExpression(IdentifierName(methodInfo.FullyQualifiedValueSetSerializerTypeName)).AddArgumentListArguments()) },
                        false => Array.Empty<ArgumentSyntax>()
                    };

                    // Prepare the type arguments for the method invocation:
                    //
                    // <<SERIALIZER_TYPE>, <RETURN_TYPE>>
                    TypeSyntax[] typeArguents = methodInfo.HasCustomValueSetSerializer switch
                    {
                        true => new[] { IdentifierName(methodInfo.FullyQualifiedValueSetSerializerTypeName), ParameterInfo.GetSyntax(methodInfo.ReturnType, methodInfo.FullyQualifiedReturnTypeName) },
                        false => new[] { ParameterInfo.GetSyntax(methodInfo.ReturnType, methodInfo.FullyQualifiedReturnTypeName) }
                    };

                    // return request.SendAndWaitForResultAsync<<RETURN_TYPE>>(<ARGUMENTS>);
                    requestStatements.Add(
                        ReturnStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("request"),
                                    GenericName(Identifier("SendAndWaitForResultAsync"))
                                    .AddTypeArgumentListArguments(typeArguents)))
                            .AddArgumentListArguments(arguments)));
                }

                // Prepare the method declaration:
                //
                // /// <inheritdoc/>
                // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
                // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                // public <RETURN_TYPE> <METHOD_NAME>(<PARAMETERS>)
                // {
                //     <STATEMENTS>
                // }
                methodDeclarations.Add(
                    MethodDeclaration(
                        returnType,
                        Identifier(methodInfo.MethodName))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(methodParameters.ToArray())
                    .AddBodyStatements(requestStatements.ToArray())
                    .AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(AppServiceGenerator).FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(AppServiceGenerator).Assembly.GetName().Version.ToString()))))))
                        .WithOpenBracketToken(Token(TriviaList(Comment("/// <inheritdoc/>")), SyntaxKind.OpenBracketToken, TriviaList())),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage"))))));
            }


            return methodDeclarations.ToImmutable();
        }
    }
}
