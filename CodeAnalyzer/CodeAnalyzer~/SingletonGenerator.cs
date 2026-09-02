using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
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

        private static readonly DiagnosticDescriptor InvalidClassDerivationRule = new DiagnosticDescriptor(
            id: "SSCN001",
            title: "Invalid class derivation",
            messageFormat: "Class '{0}' must derive from either 'MonoBehaviour' or 'ScriptableObject' to be a valid singleton",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        private static readonly DiagnosticDescriptor ClassNotPartialRule = new DiagnosticDescriptor(
            id: "SSCN002",
            title: "Class is not partial",
            messageFormat: "Class '{0}' must be declared partial to be a valid singleton",
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

            string baseTypeFullName = classInfo.TypeSymbol.DerivesFromAnyOf(AllowedBaseTypeFullNames);
            if (baseTypeFullName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidClassDerivationRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
                return;
            }

            bool isPartial = classInfo.DeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            if (!isPartial)
            {
                context.ReportDiagnostic(Diagnostic.Create(ClassNotPartialRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
                return;
            }

            bool isGeneric = classInfo.TypeSymbol.IsGenericType;
            if (isGeneric)
            {
                context.ReportDiagnostic(Diagnostic.Create(ClassIsGenericRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
                return;
            }
        }
    }
}
