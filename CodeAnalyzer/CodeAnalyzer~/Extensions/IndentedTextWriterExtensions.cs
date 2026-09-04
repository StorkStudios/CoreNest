using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalyzer.Extensions
{
    public static class IndentedTextWriterExtensions
    {
        public static IDisposable WithBlock(this IndentedTextWriter writer, string blockStart = "{", string blockEnd = "}", int indentLevel = 1)
        {
            writer.BeginBlock(blockStart, indentLevel);
            return new ActionDisposable(() => writer.EndBlock(blockEnd, indentLevel));
        }

        public static void BeginBlock(this IndentedTextWriter writer, string blockStart = "{", int indentLevel = 1)
        {
            writer.WriteLine(blockStart);
            writer.Indent += indentLevel;
        }

        public static void EndBlock(this IndentedTextWriter writer, string blockEnd = "}", int indentLevel = 1)
        {
            writer.Indent -= indentLevel;
            writer.WriteLine(blockEnd);
        }
    }
}
