using ClinetOnline.Extensions;
using ClinetOnline.Models;
using ClinetOnline.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClinetOnline.Services
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;
        private readonly UrlsOptions _urlsOptions;

        public AppService(HttpClient httpClient, IOptions<UrlsOptions> urlsOptions)
        {
            _httpClient = httpClient;
            _urlsOptions = urlsOptions.Value;
        }
        public async Task<State> AppStarted(AppStarted appEvent)
        {
            string jsonContent = JsonSerializer.Serialize(appEvent);
            StringContent contentString = new(jsonContent, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
            HttpResponseMessage response = await _httpClient.PostAsync($"{_urlsOptions.OnlineServiceUrl}{ UrlsOptions.OnlineService.AppsController.NotifyAppEvent()}", contentString);

            return (response.StatusCode == HttpStatusCode.OK) ? await response.ReadContentAs<State>() : null;
        }
    }
}
