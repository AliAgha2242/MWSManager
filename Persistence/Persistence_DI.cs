using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MWSMan.Core.Persistence.Security;
using MWSMan.Core.Persistence.StaticConfiguration;
using System;


namespace Persistence
{
    public static class Persistence_DI
    {
        public static IServiceCollection Persistence_DIRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MinooSecurity>();
            services.AddSingleton<IMinooConfiguration, MinooConfiguration>();
            services.AddSingleton<IMinooOperationConfiguration, IMinooOperationConfiguration>(p => new MinooOperationConfiguration().LoadOperationsConfig());
            services.AddDbContext<MinooDbContext>(p =>
            {
                p.UseSqlServer(configuration.GetConnectionString("Rabbit-SRV"));
            });
            return services;
        }
    }
}
