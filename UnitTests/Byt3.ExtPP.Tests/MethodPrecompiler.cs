﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Byt3.ExtPP.Tests
{
    public static class MethodPrecompiler
    {
        public static void Precompile(string methodName)
        {

            RuntimeMethodHandle handle = FindMethodWithName(methodName).MethodHandle;
            RuntimeHelpers.PrepareMethod(handle);


        }

        public static void PrecompileClass<T>()
        {
            PrecompileClass(typeof(T));
        }

        public static void PrecompileClass(Type t)
        {
            MethodInfo[] infos = t.GetMethods(METHOD_BINDING_FLAGS);
            foreach (MethodInfo runtimeMethodHandle in infos)
            {
                RuntimeHelpers.PrepareMethod(runtimeMethodHandle.MethodHandle);
            }
        }


        private static MethodInfo FindMethodWithName(string methodName)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .SelectMany(type => type.GetMethods(METHOD_BINDING_FLAGS))
                    .FirstOrDefault(method => method.Name == methodName);
        }

        private const BindingFlags METHOD_BINDING_FLAGS =
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static;
    }
}