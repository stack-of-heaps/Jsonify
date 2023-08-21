namespace webapi.Service
{
    using webapi.Managers;

    public static class InversionOfControl
    {
        public static void AddManagerDependencies(this IServiceCollection services)
        {
            services.AddTransient<IClassManager, ClassManager>();
        }
    }
}
