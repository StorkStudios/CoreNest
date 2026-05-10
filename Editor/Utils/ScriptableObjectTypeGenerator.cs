using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class ScriptableObjectTypeGenerator
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
        private static readonly Dictionary<string, Type> invalidTypes = new Dictionary<string, Type>();

        private static readonly Type emptyWrapperType = CreateParametersWrapperType("EmptyParametersWrapper", null);
        private static readonly ScriptableObject emptyWrapperInstance = ScriptableObject.CreateInstance(emptyWrapperType);

        public static ScriptableObject CreateParametersWrapperInstance(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length <= 0)
            {
                return emptyWrapperInstance;
            }

            string className = $"{methodInfo.DeclaringType.FullName.Replace('.', '_').Replace('`', '_')}_{methodInfo.Name}_ParametersWrapper";
            className = char.ToUpper(className[0]) + className[1..];

            if (invalidTypes.ContainsKey(className))
            {
                return null;
            }

            if (createdTypes.TryGetValue(className, out Type type))
            {
                return ScriptableObject.CreateInstance(type);
            }

            type = CreateParametersWrapperType(className, methodInfo);
            ScriptableObject instance = ScriptableObject.CreateInstance(type);

            if (!ValidateWrapper(instance, methodInfo))
            {
                invalidTypes.Add(className, type);
                return null;
            }

            createdTypes.Add(className, type);
            return instance;
        }

        private static Type CreateParametersWrapperType(string className, MethodInfo methodInfo)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                $"{assemblyName}.{className}",
                TypeAttributes.NotPublic,
                typeof(ScriptableObject));

            Type interaceType = typeof(IMethodParameters);
            typeBuilder.AddInterfaceImplementation(interaceType);

            List<FieldBuilder> fields = new List<FieldBuilder>();

            IEnumerable<ParameterInfo> parameters = methodInfo?.GetParameters() ?? Array.Empty<ParameterInfo>();
            foreach (ParameterInfo parameter in parameters)
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

            typeBuilder.DefineMethodOverride(methodBuilder, interaceType.GetMethod("GetValues"));

            methodBuilder = typeBuilder.DefineMethod(
                "get_Count",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(int),
                Type.EmptyTypes);

            il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldc_I4, fields.Count);
            il.Emit(OpCodes.Ret);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                "Count",
                PropertyAttributes.None,
                typeof(int),
                Type.EmptyTypes);
            propertyBuilder.SetGetMethod(methodBuilder);

            typeBuilder.DefineMethodOverride(methodBuilder, interaceType.GetProperty("Count").GetGetMethod());

            return typeBuilder.CreateType();
        }

        private static bool ValidateWrapper(ScriptableObject wrapperInstance, MethodInfo methodInfo)
        {
            SerializedObject serializedObject = new SerializedObject(wrapperInstance);
            serializedObject.Update();

            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                SerializedProperty property = serializedObject.FindProperty(parameter.Name);
                if (property == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}