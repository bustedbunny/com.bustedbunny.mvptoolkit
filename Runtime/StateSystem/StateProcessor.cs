using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity;

namespace MVPToolkit.StateSystem
{
    public class StateCallback
    {
        public StateCallback(Action action, string[] uris)
        {
            this.action = action;
            this.uris = uris;
        }

        public readonly Action action;
        public readonly string[] uris;
    }

    public static class StateProcessor
    {
        internal static IEnumerable<StateCallback> Parse([NotNull] BasePresentation presentation)
        {
            var type = presentation.GetType();

            yield return new StateCallback(presentation.Enable, GetTypeUris(type));


            // We only need public non-static methods
            var methods = GetAllMethods(type);

            foreach (var callback in ProcessMethods(methods, presentation))
            {
                yield return callback;
            }
        }

        internal static IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            if (type.BaseType != typeof(object))
            {
                foreach (var methodInfo in GetAllMethods(type.BaseType))
                {
                    yield return methodInfo;
                }
            }

            foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                       BindingFlags.NonPublic))
            {
                yield return methodInfo;
            }
        }

        internal static string[] GetTypeUris(Type type)
        {
            var attributes = type.GetCustomAttributes<StateURLAttribute>().ToArray();

            return attributes.Length > 0
                ? GetUris(attributes)
                : new[] { type.Name.Replace("Presentation", "").Trim() };
        }

        internal static string[] GetUris([NotNull] StateURLAttribute[] attributes)
        {
            var uris = new string[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                uris[i] = attributes[i].uri;
            }

            return uris;
        }

        internal static bool CheckMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void))
            {
                Debug.LogWarning($"Only void methods are allowed. Tried to parse type {methodInfo.Name}.");
                return true;
            }

            if (methodInfo.IsStatic)
            {
                Debug.LogWarning($"Only non-static methods are allowed. Tried to parse type {methodInfo.Name}.");
                return true;
            }

            if (methodInfo.GetParameters().Any())
            {
                Debug.LogWarning($"Only parameterless methods are allowed. Tried to parse type {methodInfo.Name}.");
                return true;
            }

            return false;
        }

        private static IEnumerable<StateCallback> ProcessMethods([NotNull] IEnumerable<MethodInfo> methods,
            BasePresentation presentation)
        {
            foreach (var methodInfo in methods)
            {
                var attributes = methodInfo.GetCustomAttributes<StateURLAttribute>().ToArray();
                if (attributes.Length == 0) continue;

                if (CheckMethodInfo(methodInfo))
                {
                    continue;
                }

                var methodDelegate = methodInfo.CreateDelegate(typeof(Action), presentation) as Action;
                yield return new StateCallback(methodDelegate, GetUris(attributes));
            }
        }
    }
}