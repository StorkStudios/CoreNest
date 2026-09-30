using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StorkStudios.CoreNest.CodeAnalyzer
{
    public static class TemplateRegistry
    {
        private static readonly Dictionary<string, Template> templates = [];

        public static Template GetTemplate(string templatePath)
        {
            if (!templates.ContainsKey(templatePath))
            {
                templates.Add(templatePath, LoadTemplate(templatePath));
            }

            return templates[templatePath];
        }

        private static Template LoadTemplate(string templatePath)
        {
            Assembly assembly = typeof(TemplateRegistry).Assembly;
            string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n == templatePath);
            if (resourceName == default)
            {
                return Template.Parse($"Template '{templatePath}' not found as an embedded resource.");
            }
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new(stream, Encoding.UTF8);
            return Template.Parse(reader.ReadToEnd());
        }
    }
}
