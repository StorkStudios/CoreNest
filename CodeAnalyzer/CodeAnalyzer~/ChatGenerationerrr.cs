/*
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorkStudios.CoreNest.CodeAnalyzer
{
    [Generator]
    public sealed class SingletonSourceGenerator : ISourceGenerator
    {
        private const string SingletonAttributeName =
            "StorkStudios.CoreNest.SingletonAttribute";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(
                () => new SingletonSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Don't run against the CoreNest editor assembly.
            //
            // This also protects you from the duplicate type problem
            // you were seeing earlier.
            if (context.Compilation.AssemblyName ==
                "StorkStudios.CoreNest.Editor")
            {
                return;
            }

            if (context.SyntaxReceiver is not SingletonSyntaxReceiver receiver)
                return;

            foreach (ClassDeclarationSyntax classDeclaration
                     in receiver.CandidateClasses)
            {
                GenerateSingleton(context, classDeclaration);
            }
        }

        private static void GenerateSingleton(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration)
        {
            SemanticModel semanticModel =
                context.Compilation.GetSemanticModel(
                    classDeclaration.SyntaxTree);

            INamedTypeSymbol? classSymbol =
                semanticModel.GetDeclaredSymbol(classDeclaration);

            if (classSymbol == null)
                return;

            // ---------------------------------------------------------
            // Check [Singleton]
            // ---------------------------------------------------------

            bool hasSingletonAttribute =
                classSymbol.GetAttributes()
                    .Any(attribute =>
                        attribute.AttributeClass?.ToDisplayString() ==
                        SingletonAttributeName);

            if (!hasSingletonAttribute)
                return;

            // ---------------------------------------------------------
            // Validate partial
            // ---------------------------------------------------------

            if (!classDeclaration.Modifiers.Any(
                    modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
            {
                ReportDiagnostic(
                    context,
                    "CORENEST001",
                    "Singleton class must be declared partial.",
                    classDeclaration.GetLocation());

                return;
            }

            // ---------------------------------------------------------
            // Validate MonoBehaviour inheritance
            // ---------------------------------------------------------

            if (!DerivesFromMonoBehaviour(classSymbol))
            {
                ReportDiagnostic(
                    context,
                    "CORENEST002",
                    "[Singleton] can only be used on classes deriving from MonoBehaviour.",
                    classDeclaration.GetLocation());

                return;
            }

            // ---------------------------------------------------------
            // Validate generic classes
            // ---------------------------------------------------------

            if (classSymbol.TypeParameters.Length > 0)
            {
                ReportDiagnostic(
                    context,
                    "CORENEST003",
                    "[Singleton] cannot be used on generic classes.",
                    classDeclaration.GetLocation());

                return;
            }

            // ---------------------------------------------------------
            // Generate
            // ---------------------------------------------------------

            string source = GenerateSource(classSymbol);

            string hintName =
                $"{classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Singleton.g.cs";

            // Remove "global::" from the filename.
            hintName = hintName.Replace("global::", "");

            context.AddSource(
                hintName,
                SourceText.From(source, Encoding.UTF8));
        }

        private static bool DerivesFromMonoBehaviour(
            INamedTypeSymbol classSymbol)
        {
            INamedTypeSymbol? current = classSymbol;

            while (current != null)
            {
                if (current.ToDisplayString() ==
                    "UnityEngine.MonoBehaviour")
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }

        private static string GenerateSource(
            INamedTypeSymbol classSymbol)
        {
            string namespaceName =
                classSymbol.ContainingNamespace.IsGlobalNamespace
                    ? ""
                    : classSymbol.ContainingNamespace.ToDisplayString();

            string className = classSymbol.Name;

            string namespaceStart =
                string.IsNullOrEmpty(namespaceName)
                    ? ""
                    : $"namespace {namespaceName}\n{{";

            string namespaceEnd =
                string.IsNullOrEmpty(namespaceName)
                    ? ""
                    : "}";

            return $$"""
                // <auto-generated />
                #nullable enable

                using UnityEngine;

                {{namespaceStart}}

                public partial class {{className}}
                {
                    private static {{className}}? _instance;

                    private static bool _isQuitting;

                    public static {{className}}? Instance
                    {
                        get
                        {
                            if (_isQuitting)
                                return null;

                            if (_instance == null)
                            {
                                _instance =
                                    Object.FindFirstObjectByType<{{className}}>();
                            }

                            return _instance;
                        }
                    }

                    public static bool HasInstance
                    {
                        get
                        {
                            return _instance != null;
                        }
                    }

                    protected virtual void Awake()
                    {
                        if (_instance != null && _instance != this)
                        {
                            Destroy(gameObject);
                            return;
                        }

                        _instance = this;
                    }

                    protected virtual void OnDestroy()
                    {
                        if (_instance == this)
                        {
                            _instance = null;
                        }
                    }

                    protected virtual void OnApplicationQuit()
                    {
                        _isQuitting = true;
                    }
                }

                {{namespaceEnd}}
                """;
        }

        private static void ReportDiagnostic(
            GeneratorExecutionContext context,
            string id,
            string message,
            Location location)
        {
            DiagnosticDescriptor descriptor =
                new DiagnosticDescriptor(
                    id,
                    "CoreNest Singleton",
                    message,
                    "CoreNest",
                    DiagnosticSeverity.Error,
                    true);

            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location));
        }
    }

    internal sealed class SingletonSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } =
            new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                if (classDeclaration.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(classDeclaration);
                }
            }
        }
    }
}*/