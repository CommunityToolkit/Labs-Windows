// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Attributes;
using CommunityToolkit.Tooling.SampleGen.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.Tooling.SampleGen;

/// <summary>
/// For the generated sample pane options, this generator creates the backing properties needed for binding in the UI,
/// as well as implementing the <see cref="IToolkitSampleGeneratedOptionPropertyContainer"/> for relaying data between the options pane and the generated property.
/// </summary>
[Generator]
public class ToolkitSampleOptionGenerator : IIncrementalGenerator
{
    private readonly HashSet<string> _handledPropertyNames = new();
    private readonly HashSet<ISymbol> _handledContainingClasses = new(SymbolEqualityComparer.Default);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax c && c.AttributeLists.Count > 0,
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node))
            .Where(static m => m is not null)
            .Select(static (x, _) => x!);

        // Get all attributes + the original type symbol.
        var allAttributeData = classes.SelectMany((sym, _) => sym.GetAttributes().Select(x => (sym, x)));

        // Find and reconstruct attributes.
        var sampleAttributeOptions = allAttributeData
            .Select((x, _) =>
            {
                if (x.Item2.TryReconstructAs<ToolkitSampleBoolOptionAttribute>() is ToolkitSampleBoolOptionAttribute boolOptionAttribute)
                    return (Attribute: (ToolkitSampleOptionBaseAttribute)boolOptionAttribute, ContainingClassSymbol: x.Item1, Type: typeof(ToolkitSampleBoolOptionMetadataViewModel));

                if (x.Item2.TryReconstructAs<ToolkitSampleMultiChoiceOptionAttribute>() is ToolkitSampleMultiChoiceOptionAttribute multiChoiceOptionAttribute)
                    return (Attribute: (ToolkitSampleOptionBaseAttribute)multiChoiceOptionAttribute, ContainingClassSymbol: x.Item1, Type: typeof(ToolkitSampleMultiChoiceOptionMetadataViewModel));

                if(x.Item2.TryReconstructAs<ToolkitSampleNumericOptionAttribute>() is ToolkitSampleNumericOptionAttribute numericOptionAttribute)
                    return (Attribute: (ToolkitSampleOptionBaseAttribute)numericOptionAttribute, ContainingClassSymbol: x.Item1, Type: typeof(ToolkitSampleNumericOptionMetadataViewModel));

                if (x.Item2.TryReconstructAs<ToolkitSampleTextOptionAttribute>() is ToolkitSampleTextOptionAttribute textOptionAttribute)
                    return (Attribute: (ToolkitSampleOptionBaseAttribute)textOptionAttribute, ContainingClassSymbol: x.Item1, Type: typeof(ToolkitSampleTextOptionMetadataViewModel));

                return default;
            })
            .Where(x => x != default);

        context.RegisterSourceOutput(sampleAttributeOptions, (ctx, data) =>
        {
            if (_handledContainingClasses.Add(data.ContainingClassSymbol))
            {
                if (data.ContainingClassSymbol is ITypeSymbol typeSym && !typeSym.AllInterfaces.Any(x => x.HasFullyQualifiedName("global::System.ComponentModel.INotifyPropertyChanged")))
                {
                    var inpcImpl = BuildINotifyPropertyChangedImplementation(data.ContainingClassSymbol);
                    ctx.AddSource($"{data.ContainingClassSymbol}.NotifyPropertyChanged.g", inpcImpl);
                }

                var propertyContainerSource = BuildGeneratedPropertyMetadataContainer(data.ContainingClassSymbol);
                ctx.AddSource($"{data.ContainingClassSymbol}.GeneratedPropertyContainer.g", propertyContainerSource);
            }

            var name = $"{data.ContainingClassSymbol}.Property.{data.Attribute.Name}.g";

            if (_handledPropertyNames.Add(name))
            {
                var dependencyPropertySource = BuildProperty(data.ContainingClassSymbol, data.Attribute.Name, data.Attribute.TypeName, data.Type);
                ctx.AddSource(name, dependencyPropertySource);
            }
        });

    }

    private static string BuildINotifyPropertyChangedImplementation(ISymbol containingClassSymbol)
    {
        return $@"#nullable enable
using System.ComponentModel;

namespace {containingClassSymbol.ContainingNamespace}
{{
    public partial class {containingClassSymbol.Name} : {nameof(System.ComponentModel.INotifyPropertyChanged)}
    {{
		public event PropertyChangedEventHandler? PropertyChanged;
    }}
}}
";
    }

    private static string BuildGeneratedPropertyMetadataContainer(ISymbol containingClassSymbol)
    {
        return $@"#nullable enable
using System.ComponentModel;
using System.Collections.Generic;

namespace {containingClassSymbol.ContainingNamespace}
{{
    public partial class {containingClassSymbol.Name} : {typeof(IToolkitSampleGeneratedOptionPropertyContainer).Namespace}.{nameof(IToolkitSampleGeneratedOptionPropertyContainer)}
    {{
        private IEnumerable<{typeof(IGeneratedToolkitSampleOptionViewModel).FullName}>? _generatedPropertyMetadata;

        public IEnumerable<{typeof(IGeneratedToolkitSampleOptionViewModel).FullName}>? GeneratedPropertyMetadata
        {{
            get => _generatedPropertyMetadata;
            set
            {{
                if (!(_generatedPropertyMetadata is null))
                {{
                    foreach (var item in _generatedPropertyMetadata)
                        item.PropertyChanged -= OnPropertyChanged;
                }}
                
                if (!(value is null))
                {{
                    foreach (var item in value)
                        item.PropertyChanged += OnPropertyChanged;
                }}               

                _generatedPropertyMetadata = value;
            }}
        }}

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    }}
}}
";
    }

    private static string BuildProperty(ISymbol containingClassSymbol, string propertyName, string typeName, Type viewModelType)
    {
        return $@"#nullable enable
using System.ComponentModel;
using System.Linq;

namespace {containingClassSymbol.ContainingNamespace}
{{
    public partial class {containingClassSymbol.Name}
    {{
        public {typeName} {propertyName}
        {{
            get => (({typeName})(({viewModelType.FullName})GeneratedPropertyMetadata!.First(x => x.Name == ""{propertyName}""))!.Value!)!;
            set
            {{
				if (GeneratedPropertyMetadata?.FirstOrDefault(x => x.Name == nameof({propertyName})) is {viewModelType.FullName} metadata)
				{{
                    metadata.Value = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({propertyName})));
				}}
            }}
        }}
    }}
}}
";
    }
}
