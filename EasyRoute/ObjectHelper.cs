using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static EasyRoute.Utils;

namespace EasyRoute.Extensions
{
    public static class ObjectHelper
    {
        public static object Call(this object obj, string path)
        {
            return Call(obj, null, path);
        }

        public static object Call(this Type t, string path)
        {
            return Call(null, t, path);
        }

        private static object Call(object obj, Type t, string path)
        {
            var context = path.StartsWith("/") ? Settings.Root : obj;
            var segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < segments.Length; i++)
            {
                var type = context != null ? context.GetType() : t;
                var segment = segments[i];
                var args = segment.Split(',').Select(s => s.Trim()).ToArray();
                var methods = FilterMethods(type, args);
                if (methods.Length <= 0)
                {
                    throw new Exception("Method not found.");
                }
                context = TryInvoke(context, methods, args);
            }
            return context;
        }

        private static MethodInfo[] FilterMethods(Type t, string[] args)
        {
            return t.GetMethods()
                .Where(m => m.GetParameters().Length >= args.Length - 1)
                .Where(m => m.GetParameters().All(p =>
                    Constants.KnownParameterTypes.Contains(p.ParameterType)))
                .Where(m =>
                {
                    var methodName = Settings.CaseSensitive ? m.Name : m.Name.ToLower();
                    var inputName = Settings.CaseSensitive ? args[0] : args[0].ToLower();
                    if (m.GetCustomAttributes(true).Where(a => a is RouteAttribute)
                        .FirstOrDefault() is RouteAttribute routeNameAttr
                        && IsValidIdentifier(routeNameAttr.GetRouteName() ?? methodName))
                    {
                        var routeName = Settings.CaseSensitive
                            ? routeNameAttr.GetRouteName()
                            : routeNameAttr.GetRouteName().ToLower();
                        return inputName == (routeName ?? methodName);
                    }
                    else if (Settings.RequireAttribute == false)
                    {
                        return inputName == methodName;
                    }
                    return false;
                })
                .OrderBy(m => m, new MethodInfoComparer())
                .ToArray();
        }

        private static object TryInvoke(object context, MethodInfo[] methods, string[] args)
        {
            var success = false;
            foreach (var method in methods)
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != args.Length - 1)
                {
                    continue;
                }
                var actualParams = new object[methodParams.Length];
                var exit = false;
                for (var j = 1; j < args.Length; j++)
                {
                    var t = methodParams[j - 1].ParameterType;
                    if (args[j] == "")
                    {
                        continue;
                    }
                    var arg = TryParse(args[j], t);
                    if (arg == null)
                    {
                        exit = true;
                        break;
                    }
                    actualParams[j - 1] = arg;
                }
                if (exit)
                {
                    continue;
                }
                try
                {
                    context = method.Invoke(context, actualParams);
                    success = true;
                }
                catch (Exception) // Manually check type to improve performance
                {
                    continue;
                }
                break;
            }
            if (!success)
            {
                throw new Exception("Parameters not match.");
            }

            return context;
        }

        private static object TryParse(string s, Type t)
        {
            if (t == typeof(int))
            {
                if (int.TryParse(s, out var o))
                {
                    return o;
                }
                return null;
            }
            else if (t == typeof(uint))
            {
                if (uint.TryParse(s, out var o))
                {
                    return o;
                }
                return null;
            }
            else if (t == typeof(long))
            {
                if (long.TryParse(s, out var o))
                {
                    return o;
                }
                return null;
            }
            else if (t == typeof(ulong))
            {
                if (ulong.TryParse(s, out var o))
                {
                    return o;
                }
                return null;
            }
            else if (t == typeof(string))
            {
                return s;
            }
            else
            {
                return null;
            }
        }

        public static void SetAsRoot(this object obj)
        {
            Settings.Root = obj;
        }
    }
}
