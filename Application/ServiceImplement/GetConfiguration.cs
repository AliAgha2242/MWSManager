using Application.ServiceContract;
using MWSMan.Core.Infrastructure.Extenssion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.ServiceImplement
{
    public class GetConfiguration : IGetConfiguration
    {
        
        public async Task GetConfigAsync(string configNameWithExt)
        {
            try
            {
                string middleApiUrl = "http://Rabbit-srv:5051/SendMessage".Replace("/SendMessage", $"/Get{Path.GetFileNameWithoutExtension(configNameWithExt)}");
                using HttpClient client = new HttpClient();
                HttpResponseMessage? response = await client.GetAsync(middleApiUrl);

                var responseContent = await response.Content.ReadAsStringAsync();

                string json = JsonSerializer.Deserialize<string>(responseContent) ?? throw new InvalidOperationException("Invalid JSON string.");
                using JsonDocument document = JsonDocument.Parse(json);
                string formattedJson = JsonSerializer.Serialize(
                document.RootElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configNameWithExt);

                await SafeFileWriter.ReplaceContentAsync(filePath, formattedJson, 3, 2000);

            }
            catch (Exception ex)
            {
                throw ;
            }
        }

       
    }
}
