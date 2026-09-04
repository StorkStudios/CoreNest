using CodeAnalyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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

            List<Diagnostic> diagnostics = new List<Diagnostic>();

            string baseTypeFullName = classInfo.TypeSymbol.DerivesFromAnyOf(AllowedBaseTypeFullNames);
            if (baseTypeFullName == null)
            {
                diagnostics.Add(Diagnostic.Create(InvalidClassDerivationRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            bool isPartial = classInfo.DeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            if (!isPartial)
            {
                diagnostics.Add(Diagnostic.Create(ClassNotPartialRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
            }

            bool isGeneric = classInfo.TypeSymbol.IsGenericType;
            if (isGeneric)
            {
                diagnostics.Add(Diagnostic.Create(ClassIsGenericRule, singletonAttribute.ApplicationSyntaxReference.GetSyntax().GetLocation(), classInfo.TypeSymbol.Name));
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

            indentedWriter.WriteLine($"public partial class {classInfo.TypeSymbol.Name}");
            using (indentedWriter.WithBlock("{", "}"))
            {
                indentedWriter.WriteLine($"public static event Action<{classInfo.TypeSymbol.Name}> OnInitialize;");
                indentedWriter.WriteLine();
                indentedWriter.WriteLine($"public static {classInfo.TypeSymbol.Name} Instance");
                using (indentedWriter.WithBlock("{", "}"))
                {
                    indentedWriter.WriteLine("get");
                    using (indentedWriter.WithBlock("{", "}"))
                    {
                        indentedWriter.WriteLine("if (!IsInstanced)");
                        using (indentedWriter.WithBlock("{", "}"))
                        {
                            indentedWriter.WriteLine($"{classInfo.TypeSymbol.Name} inst = UnityEngine.Object.FindAnyObjectByType<{classInfo.TypeSymbol.Name}>();");
                            indentedWriter.WriteLine("if (inst != null)");
                            using (indentedWriter.WithBlock("{", "}"))
                            {
                                indentedWriter.WriteLine("RegisterInstance(inst);");
                            }
                            indentedWriter.WriteLine("else");
                            using (indentedWriter.WithBlock("{", "}"))
                            {
                                indentedWriter.WriteLine($"Debug.LogWarning(\"Couldn't find {classInfo.TypeSymbol.Name} singleton\");");
                            }
                        }
                        indentedWriter.WriteLine("return instance;");
                    }
                }
                indentedWriter.WriteLine();
                indentedWriter.WriteLine($"private static {classInfo.TypeSymbol.Name} instance;");
                indentedWriter.WriteLine();
                indentedWriter.WriteLine("public static bool IsInitialized { get; private set; } = false;");
                indentedWriter.WriteLine("public static bool IsInstanced { get; private set; } = false;");
                indentedWriter.WriteLine();
                indentedWriter.WriteLine($"public static void CallWhenInitialized(Action<{classInfo.TypeSymbol.Name}> action)");
                using (indentedWriter.WithBlock("{", "}"))
                {
                    indentedWriter.WriteLine("if (!IsInitialized)");
                    using (indentedWriter.WithBlock("{", "}"))
                    {
                        indentedWriter.WriteLine("action?.Invoke(instance);");
                    }
                    indentedWriter.WriteLine("else");
                    using (indentedWriter.WithBlock("{", "}"))
                    {
                        indentedWriter.WriteLine($"void OneShot({classInfo.TypeSymbol.Name} arg)");
                        using (indentedWriter.WithBlock("{", "}"))
                        {
                            indentedWriter.WriteLine("action?.Invoke(instance);");
                            indentedWriter.WriteLine("OnInitialize -= OneShot;");
                        }
                        indentedWriter.WriteLine();
                        indentedWriter.WriteLine("OnInitialize += OneShot;");
                    }
                }
                indentedWriter.WriteLine();
                indentedWriter.WriteLine($"private static void RegisterInstance({classInfo.TypeSymbol.Name} inst)");
                using (indentedWriter.WithBlock("{", "}"))
                {
                    indentedWriter.WriteLine("if (IsInstanced && instance != inst)");
                    using (indentedWriter.WithBlock("{", "}"))
                    {
                        indentedWriter.WriteLine($"Debug.LogError($\"More than one instance of singleton {classInfo.TypeSymbol.Name} registered. First: {{instance.gameObject.name}}, second: {{inst.gameObject.name}}\");");
                    }
                    indentedWriter.WriteLine("else");
                    using (indentedWriter.WithBlock("{", "}"))
                    {
                        indentedWriter.WriteLine("instance = inst;");
                        indentedWriter.WriteLine("IsInstanced = true;");
                    }
                }
                indentedWriter.WriteLine();
                indentedWriter.WriteLine("protected virtual void Awake()");
                using (indentedWriter.WithBlock("{", "}"))
                {
                    indentedWriter.WriteLine("RegisterInstance(this);");
                    indentedWriter.WriteLine("IsInitialized = true;");
                    indentedWriter.WriteLine("OnInitialize?.Invoke(instance);");
                }
                indentedWriter.WriteLine();
                indentedWriter.WriteLine("protected virtual void OnDestroy()");
                using (indentedWriter.WithBlock("{", "}"))
                {
                    indentedWriter.WriteLine("IsInitialized = false;");
                    indentedWriter.WriteLine("IsInstanced = false;");
                    indentedWriter.WriteLine("instance = null;");
                }
            }

            if (!classInfo.TypeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                indentedWriter.EndBlock("}");
            }

            context.AddSource($"{classInfo.TypeSymbol.Name}.Singleton.g.cs", SourceText.From(sourceStream.ToString(), Encoding.UTF8));
        }
    }
}
