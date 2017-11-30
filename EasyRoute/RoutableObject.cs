using System;
using System.Linq;
using System.Reflection;

namespace EasyRoute
{
    [Routable]
    public class RoutableObject
    {
        public virtual object Call(string path,params object[] param)
        {
            var pathList = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathList.Length <= 0)
                throw new ArgumentException("Invalid path.");
            else if (pathList.Length == 1)
            {
                var method = GetMethod(pathList[0]);
                if (method != null)
                    return method.Invoke(this, param);

                var property = GetProperty(pathList[0]);
                if (property != null)
                    return property.GetValue(this);

                var field = GetField(pathList[0]);
                if (field != null)
                    return field.GetValue(this);
                throw new MemberNotFoundException(this, pathList[0]);
            }
            else
            {
                var subPath = pathList[1];
                for (var i = 2; i < pathList.Length; i++)
                    subPath += "/" + pathList[i];

                var property = GetProperty(pathList[0]);
                if (property != null)
                {
                    if (!property.DeclaringType.IsSubclassOf(typeof(RoutableObject)))
                        throw new UnreachableException(path);

                    return (property.GetValue(this) as RoutableObject).Call(subPath, param);
                }

                var field = GetField(pathList[0]);
                if(field != null)
                {
                    if (!field.DeclaringType.IsSubclassOf(typeof(RoutableObject)))
                        throw new UnreachableException(path);
                    return (field.GetValue(this) as RoutableObject).Call(subPath, param);
                }

                throw new MemberNotFoundException(this, pathList[0]);
            }

        }

        protected PropertyInfo GetProperty(string name)
        {
            return this.GetType().GetProperties().Where(
                    property => property.GetCustomAttributes(true).Where(
                        attr =>
                            attr is Routable &&
                            (
                                ((attr as Routable).Name == null && property.Name == name)
                                ||
                                ((attr as Routable).Name == name)
                            )
                    ).FirstOrDefault() != null
                ).FirstOrDefault();
        }

        protected static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetProperties().Where(
                    property => property.GetCustomAttributes(true).Where(
                        attr =>
                            attr is Routable &&
                            (
                                ((attr as Routable).Name == null && property.Name == name)
                                ||
                                ((attr as Routable).Name == name)
                            )
                    ).FirstOrDefault() != null
                ).FirstOrDefault();
        }
        protected static PropertyInfo GetProperty<T>(string name)
        {
            return GetProperty(typeof(T), name);
        }

        protected FieldInfo GetField(string name)
        {
            return this.GetType().GetFields().Where(
                    field => field.GetCustomAttributes(true).Where(
                        attr =>
                            attr is Routable &&
                            (
                                ((attr as Routable).Name == null && field.Name == name)
                                ||
                                ((attr as Routable).Name == name)
                            )
                    ).FirstOrDefault() != null
                ).FirstOrDefault();
        }
        protected static FieldInfo GetField(Type type , string name)
        {
            return type.GetFields().Where(
                    field => field.GetCustomAttributes(true).Where(
                        attr =>
                            attr is Routable &&
                            (
                                ((attr as Routable).Name == null && field.Name == name)
                                ||
                                ((attr as Routable).Name == name)
                            )
                    ).FirstOrDefault() != null
                ).FirstOrDefault();
        }
        protected static FieldInfo GetField<T>(string name)
        {
            return GetField(typeof(T), name);
        }

        protected MethodInfo GetMethod(string methodName)
        {
            return this.GetType().GetMethods().Where(
                method => method.GetCustomAttributes(true).Where(
                    attr =>
                        attr is Routable &&
                        (
                            ((attr as Routable).Name == null && method.Name == methodName)
                            ||
                            ((attr as Routable).Name == methodName)
                        )
                ).FirstOrDefault() != null
            ).FirstOrDefault();

        }
        protected static MethodInfo GetMethod(Type type, string methodName)
        {
            return type.GetMethods().Where(
                   method => method.GetCustomAttributes(true).Where(
                       attr =>
                           attr is Routable &&
                           (
                               ((attr as Routable).Name == null && method.Name == methodName)
                               ||
                               ((attr as Routable).Name == methodName)
                           )
                   ).FirstOrDefault() != null
               ).FirstOrDefault();
        }
        protected static MethodInfo GetMethod<T>(string methodName)
        {
            return GetMethod(typeof(T), methodName);

        }

    }
}
