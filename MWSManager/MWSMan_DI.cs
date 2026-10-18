using MWSManager.Tasks;

namespace MWSMan
{
    public static class MWSMan_DI
    {
        public static IServiceCollection MWSMan_DIRegister(this IServiceCollection services)
        {
            
            services.AddScoped(sp =>
            {
                return new HttpClient(new HttpClientHandler
                {
                    UseCookies = true
                });
            });
            services.AddHostedService<TaskManager>();
            services.AddAntiforgery();
            return services;
        }
    }
}
