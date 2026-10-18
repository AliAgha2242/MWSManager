using MWSMan.Core.Model.PresentationModels;
using MWSMan.Core.Model.RabbitModel;
using MWSMan.Core.Persistence.StaticConfiguration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MWSMan.Core.Infrastructure.CallApiManager
{
    public class CallApiManager
    {
        public string PreUrl { get; set; }
        public UserModel UserModel { get; }

        private HttpClient _client;
        public CallApiManager(UserModel UserModel, IMinooConfiguration configuration)
        {
            this.UserModel = UserModel;
            _client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{UserModel.UserName}:{UserModel.Password}");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            string ServerIp = configuration.Get<string>("Rabbit:ServerIp");
            int Port = configuration.Get<int?>("Rabbit:ServerApiRabbitPort") ?? 15672;
            PreUrl = $"http://{ServerIp}:{Port}";
            this.UserModel = UserModel;
        }



        public async Task<ErrorModel<T>> GetAsync<T>(string Url)
        {
            var FullUrl = $"{PreUrl}{Url}";
            var response = await _client.GetAsync(FullUrl);
            var result = await response.Content.ReadAsStringAsync();
            if (result.Contains("error"))
            {
                return JsonSerializer.Deserialize<ErrorModel<T>>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new ErrorModel<T>();
            }

            var returnResult = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return new ErrorModel<T>() { data = returnResult };
        }

    }
}
