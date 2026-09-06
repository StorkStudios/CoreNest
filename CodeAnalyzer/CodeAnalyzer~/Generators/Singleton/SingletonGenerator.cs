using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace StorkStudios.CoreNest.CodeAnalyzer
{
    [Generator]
    public class SingletonGenerator : IIncrementalGenerator
    {
        private class ClassInfo
        {
            public ClassDeclarationSyntax DeclarationSyntax { get; set; }
            public INamedTypeSymbol TypeSymbol { get; set; }

            public static bool IsValid(ClassInfo classInfo)
            {
                return classInfo != null && classInfo.DeclarationSyntax != null && classInfo.TypeSymbol != null;
            }
        }

        private const string SingletonName = "Singleton";
        private const string SingletonAttributeNamespace = "StorkStudios.CoreNest";
        private const string SingletonAttributeFullName = SingletonAttributeNamespace + "." + SingletonName + "Attribute";
        private const string MonoBehaviourFullName = "UnityEngine.MonoBehaviour";
        private const string ScriptableObjectFullName = "UnityEngine.ScriptableObject";
        private static readonly string[] AllowedBaseTypeFullNames = { MonoBehaviourFullName, ScriptableObjectFullName };
        private const string TemplatePath = "StorkStudios.CoreNest.CodeAnalyzer.Generators.Singleton.SingletonTemplate.sbncs";

        private static readonly DiagnosticDescriptor InvalidClassDerivationRule = new DiagnosticDescriptor(
            id: "SSCN001",
            title: "Invalid class derivation",
            messageFormat: "Class '{0}' must derive from either 'MonoBehaviour' or 'ScriptableObject' to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        private static readonly DiagnosticDescriptor ClassNotSealedPartialRule = new DiagnosticDescriptor(
            id: "SSCN002",
            title: "Class is not sealed partial",
            messageFormat: "Class '{0}' must be declared sealed partial to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        private static readonly DiagnosticDescriptor ClassIsGenericRule = new DiagnosticDescriptor(
            id: "SSCN003",
            title: "Class is generic",
            messageFormat: "Class '{0}' must not be generic to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        private static readonly DiagnosticDescriptor SingletonAwakeRule = new DiagnosticDescriptor(
            id: "SSCN004",
            title: "Class contains Awake method",
            messageFormat: "Class '{0}' must use AfterAwake or BeforeAwake methods instead of Awake to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        private static readonly DiagnosticDescriptor SingletonOnDestroyRule = new DiagnosticDescriptor(
            id: "SSCN005",
            title: "Class contains OnDestroy method",
            messageFormat: "Class '{0}' must use AfterDestroy or BeforeDestroy methods instead of OnDestroy to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: IsPotentialSingleton,
                transform: GetClassInfo
            ).Where(ClassInfo.IsValid);

            context.RegisterSourceOutput(syntaxProvider, OnClass);
        }

        private bool IsPotentialSingleton(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                   classDeclarationSyntax.AttributeLists.Any(attrList => attrList.Attributes.Any(attr => attr.Name.ToString() == SingletonName));
        }

        private ClassInfo GetClassInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                return new ClassInfo
                {
                    DeclarationSyntax = classDeclaration,
                    TypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol
                };
            }

            return null;
        }

        private void OnClass(SourceProductionContext context, ClassInfo classInfo)
        {
            AttributeData singletonAttribute = classInfo.TypeSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == SingletonAttributeFullName);
            if (singletonAttribute == null)
            {
                return;
            }

            List<Diagnostic> diagnostics = new List<Diagnostic>();

            string baseTypeFullName = classInfo.TypeSymbol.DerivesFromAnyOf(AllowedBaseTypeFullNames);
            if (baseTypeFullName == null)
            {
                diagnostics.Add(Diagnostic.Create(InvalidClassDerivationRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            bool isSealedPartial = classInfo.DeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) && classInfo.DeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword);
            if (!isSealedPartial)
            {
                diagnostics.Add(Diagnostic.Create(ClassNotSealedPartialRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            bool isGeneric = classInfo.TypeSymbol.IsGenericType;
            if (isGeneric)
            {
                diagnostics.Add(Diagnostic.Create(ClassIsGenericRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            IMethodSymbol awakeMethod = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(m => m.Name == "Awake" && m.Parameters.Length == 0);
            if (awakeMethod != null)
            {
                diagnostics.Add(Diagnostic.Create(SingletonAwakeRule, awakeMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            IMethodSymbol onDestroyMethod = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(m => m.Name == "OnDestroy" && m.Parameters.Length == 0);
            if (onDestroyMethod != null)
            {
                diagnostics.Add(Diagnostic.Create(SingletonOnDestroyRule, onDestroyMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            if (diagnostics.Count > 0)
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
                return;
            }


            using StringWriter sourceStream = new();
            using IndentedTextWriter indentedWriter = new(sourceStream);

            indentedWriter.WriteLine("// auto-generated");
            indentedWriter.WriteLine("using System;");
            indentedWriter.WriteLine("using UnityEngine;");
            
            indentedWriter.WriteLine();

            if (!classInfo.TypeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                indentedWriter.WriteLine($"namespace {classInfo.TypeSymbol.ContainingNamespace.ToDisplayString()}");
                indentedWriter.BeginBlock("{");
            }

            Template template = TemplateRegistry.GetTemplate(TemplatePath);
            string renderedClass = template.Render(new
            {
                ClassName = classInfo.TypeSymbol.Name,
                HasBeforeAwake = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == "BeforeAwake" && m.Parameters.Length == 0),
                HasAfterAwake = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == "AfterAwake" && m.Parameters.Length == 0),
                HasBeforeDestroy = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == "BeforeDestroy" && m.Parameters.Length == 0),
                HasAfterDestroy = classInfo.TypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == "AfterDestroy" && m.Parameters.Length == 0)
            });
            indentedWriter.WriteLines(renderedClass);

            if (!classInfo.TypeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                indentedWriter.EndBlock("}");
            }

            context.AddSource($"{classInfo.TypeSymbol.Name}.Singleton.g.cs", SourceText.From(sourceStream.ToString(), Encoding.UTF8));
        }
    }
}
