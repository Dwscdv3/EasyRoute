using System;

namespace EasyRoute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        string routeName = null;

        public RouteAttribute() { }
        public RouteAttribute(string routeName)
        {
            this.routeName = routeName;
        }

        public string GetRouteName()
        {
            return routeName;
        }
    }
}
