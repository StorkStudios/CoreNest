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

        public static Type GetScriptableObjectParametersWrapper(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length <= 0)
            {
                return null;
            }

            string className = $"{methodInfo.DeclaringType.FullName.Replace('.', '_').Replace('`', '_')}_{methodInfo.Name}_ParametersWrapper";
            className = char.ToUpper(className[0]) + className[1..];

            if (createdTypes.TryGetValue(className, out Type type))
            {
                return type;
            }

            //todo check if is serializable

            type = CreateScriptableObjectParametersWrapperType(className, methodInfo);
            createdTypes.Add(className, type);
            return type;
        }

        private static Type CreateScriptableObjectParametersWrapperType(string className, MethodInfo methodInfo)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                $"{assemblyName}.{className}",
                TypeAttributes.NotPublic,
                typeof(ScriptableObject));

            typeBuilder.AddInterfaceImplementation(typeof(IMethodParameters));

            List<FieldBuilder> fields = new List<FieldBuilder>();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                fields.Add(typeBuilder.DefineField(parameter.Name, parameter.ParameterType, FieldAttributes.Public));
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetValues",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object[]),
                Type.EmptyTypes);

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldc_I4, fields.Count);
            il.Emit(OpCodes.Newarr, typeof(object));

            for (int i = 0; i < fields.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fields[i]);
                if (fields[i].FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, fields[i].FieldType);
                }
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, typeof(IMethodParameters).GetMethod("GetValues"));

            return typeBuilder.CreateType();
        }
    }
}