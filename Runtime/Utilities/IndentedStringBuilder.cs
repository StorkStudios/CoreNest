using System;
using System.Text;

public class IndentedStringBuilder
{
    private class IndentScope : IDisposable
    {
        private IndentedStringBuilder parent;
        private int oldIndentLevel;

        private string suffixLine;

        public IndentScope(IndentedStringBuilder parent, int indentLevel, string prefixLine, string suffixLine)
        {
            this.parent = parent;
            this.suffixLine = suffixLine;
            this.oldIndentLevel = parent.indentLevel;

            if (prefixLine != null)
            {
                parent.AppendLine(prefixLine);
            }

            parent.indentLevel = indentLevel;
        }

        public void Dispose()
        {
            parent.indentLevel = oldIndentLevel;

            if (suffixLine != null)
            {
                parent.AppendLine(suffixLine);
            }
        }
    }

    public StringBuilder StringBuilder { get; private set; }

    public string indentString;
    public int indentLevel = 0;

    public IndentedStringBuilder(StringBuilder stringBuilder, string indentString)
    {
        StringBuilder = stringBuilder;
        this.indentString = indentString;
    }

    public void AppendLine(string line)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            StringBuilder.Append(indentString);
        }
        StringBuilder.AppendLine(line);
    }

    public override string ToString()
    {
        return StringBuilder.ToString();
    }

    public IDisposable BeginIndentScope(int offset)
    {
        return new IndentScope(this, indentLevel + offset, null, null);
    }

    public IDisposable BeginIndentScope(int offset, string prefixLine, string suffixLine)
    {
        return new IndentScope(this, indentLevel + offset, prefixLine, suffixLine);
    }
}
