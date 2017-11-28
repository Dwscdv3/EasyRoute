using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyRoute
{
    public class MethodInfoComparer : IComparer<MethodInfo>
    {
        public int Compare(MethodInfo x, MethodInfo y) => CalculatePriority(x) - CalculatePriority(y);

        int CalculatePriority(MethodInfo m)
        {
            var acc = 0;
            foreach (var paramType in m.GetParameters().Select(param => param.ParameterType))
            {
                if (paramType == typeof(int) || paramType == typeof(uint))
                {
                    acc += 100;
                }
                else if (paramType == typeof(long) || paramType == typeof(ulong))
                {
                    acc += 10000;
                }
                else if (paramType == typeof(string))
                {
                    acc += 1000000;
                }
            }
            return acc;
        }
    }
}
