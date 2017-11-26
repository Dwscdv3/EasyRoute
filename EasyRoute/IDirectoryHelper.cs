using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static EasyRoute.Utils;

namespace EasyRoute
{
    public static class IDirectoryHelper
    {
        public static object Call(this IDirectory dir, string path)
        {
            var wd = path.StartsWith('/') ? Settings.Root : dir;
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var args = segment.Split(',').Select(s => s.Trim()).ToArray();
                var methods = wd.GetType().GetMethods()
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
                    .OrderByDescending(m => m, new MethodInfoComparer())
                    .ToArray();
                if (methods.Length <= 0)
                {
                    throw new Exception("Method not found.");
                }
                foreach (var method in methods)
                {
                    var methodParams = method.GetParameters();
                    var actualParams = new object[methodParams.Length];
                    for (var j = 1; j < args.Length; j++)
                    {
                        var t = methodParams[j - 1].ParameterType;
                        actualParams[j - 1] = TryParse(args[j], t);
                    }
                    try
                    {
                        wd = method.Invoke(wd, actualParams);
                    }
                    catch (Exception) // Manually check type to improve performance
                    {
                        continue;
                    }
                    break;
                }
            }
            return wd;
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

        public static void SetAsRoot(this IDirectory dir)
        {
            Settings.Root = dir;
        }
    }
}
