using Application.ServiceContract;
using Application.ServiceImplement;

using Infrastructure.RabbitSenderRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class Application_DI
    {
        public static IServiceCollection Application_DIRegister(this IServiceCollection services)
        {
            

            services.AddScoped<IRabbitSenderRepository, RabbitSenderRepository>();
            services.AddScoped<ISend,Send>();
            services.AddScoped<IRequestHubService, RequestHubService>();
            services.AddScoped<IRabbitTaskService, RabbitTaskService>();
            services.AddScoped<IRabbitTaskServerService, RabbitTaskServerService>();

            services.AddScoped<IFacadDesign, FacadDesign>();
            services.AddScoped<ILogService, LogService>();
            services.AddSingleton<IGetConfiguration,GetConfiguration>();
            return services;
        }
    }
}
