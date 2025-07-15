using ApiTemplate.Repository;

namespace ApiTemplate.Services
{
    public static class DIContainer
    {
        public static IServiceCollection AddDIContainer(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
