using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class ScriptableObjectGenerator
    {
        private const string assemblyName = "StorkStudios.CoreNest.DynamicAssembly";

        private static readonly AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            new AssemblyName(assemblyName)
            {
                CultureInfo = CultureInfo.InvariantCulture,
                Flags = AssemblyNameFlags.None,
                ProcessorArchitecture = ProcessorArchitecture.MSIL,
                VersionCompatibility = AssemblyVersionCompatibility.SameDomain
            },
            AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, true);

        private static readonly Dictionary<string, Type> createdTypes = new Dictionary<string, Type>();

        public static Type GetScriptableObjectWrapperType(string fieldName, Type fieldType)
        {
            string className = $"{fieldType.FullName.Replace('.', '_').Replace('`', '_')}_{fieldName}";
            className = char.ToUpper(className[0]) + className[1..];

            if (createdTypes.TryGetValue(className, out Type type))
            {
                return type;
            }

            //todo check if is serializable
            if (!fieldType.IsUnitySerializable())
            {
                return null;
            }

            type = CreateScriptableObjectWrapperType(className, fieldName, fieldType);
            createdTypes.Add(className, type);
            return type;
        }

        private static Type CreateScriptableObjectWrapperType(string className, string fieldName, Type fieldType)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                $"{assemblyName}.{className}",
                TypeAttributes.NotPublic,
                typeof(ScriptableObject));

            typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);

            return typeBuilder.CreateType();
        }
    }
}