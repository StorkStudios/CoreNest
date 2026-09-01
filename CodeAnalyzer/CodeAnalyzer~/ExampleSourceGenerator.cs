using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace StorkStudios.CoreNest.CodeAnalyzer
{
    [Generator]
    public class ExampleSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation.AssemblyName != "StorkStudios.CoreNest")
            {
                return;
            }

            System.Console.WriteLine(System.DateTime.Now.ToString());

            var sourceBuilder = new StringBuilder(
            @"
            using System;
            namespace StorkStudios.CoreNest.Generated
            {
                public static class ExampleSourceGenerated
                {
                    public static string GetTestText()
                    {
                        return ""This is from source generator ");

            sourceBuilder.Append(System.DateTime.Now.ToString());

            sourceBuilder.Append(
                @""";
                    }
    }
}
");

            context.AddSource("ExampleSourceGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
