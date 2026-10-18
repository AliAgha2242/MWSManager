using FluentValidation;
using Infrastructure.DataBaseRepository;
using Infrastructure.FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using Model.PresentationModels;
using Model.PublicModels;
using MWSMan.Core.Model.Events;
using MWSMan.Core.Model.RabbitModel;
using MWSMan.Core.Persistence.StaticConfiguration;
using RabbitMq_Infrastructure.RabbitConnection;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using RabbitSender;
using RabbitSender.Models.LogModels;
using System.Data.SqlClient;


namespace Infrastracture
{
    public static class Infrastructure_DI
    {
        public static IServiceCollection Infrastructure_DIRegister(this IServiceCollection services, IConfiguration config)
        {
            string? Cs = config.GetConnectionString("Rabbit-SRV");
            Cs = Cs ?? throw new InvalidConfigurationException("Connection String Is Not Found");

            services.AddSingleton<VHostModel>(p => new VHostModel("Anbar"));
            services.AddScoped<SqlConnectionManager>(p => new SqlConnectionManager(Cs));
            services.AddSingleton<UserModel>(p =>
            {
                return new UserModel("AdminMinoo", "RabbitAdmin");
            });
            services.AddScoped<IList<ServerSelected>>(p =>
            {
                IMinooConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IMinooConfiguration>();
                return configuration.GetAllSerevr().Select(s => new ServerSelected(s)).ToList();
            });



            SqlConnection connection = new System.Data.SqlClient.SqlConnection(Cs);
            services.AddSingleton<Client>(p =>
            {
                connection.Open();
                return new Client(400.935, null, connection, LogProviderEnum.None);
            });
            services.AddSingleton<RabbitConnectionManager>();
            services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddSingleton<CenterEvent>();
            services.AddValidatorsFromAssemblyContaining<RabbitConsumerRequestHubValidation>();
            services.AddValidatorsFromAssemblyContaining<RabbitTasksValidation>();
            services.AddTransient<DockerDateTime>();
            //services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());
            return services;
        }
    }
}
