namespace webapi.Managers
{
    using System;
    using webapi.Managers.Contracts;

    public interface IClassManager
    {
        GetClassesResponse GetClasses(ServiceNames? serviceName, ProductNames? productName);

        Property GetProperties(Type type, string name = null, int depth = 0);
    }
}