using ClinetOnline.Common;
using ClinetOnline.Services;
using System;
using System.IO;
using System.Text.Json;

namespace ClinetOnline.ViewModels
{
    public class StartViewModel : BindableBase
    {
        private readonly IAppService _appService;

        public StartViewModel(IAppService appService)
        {
            StartCommand = new RelayCommand(OnStart);
            _appService = appService;
        }
        public event Action Started;
        private async void OnStart()
        {
            Models.State? result = await _appService.AppStarted(new Models.AppStarted());
            if (result != null)
            {
                var state = new { Id = result.Id };
                string stateJson = JsonSerializer.Serialize(state);

                string? localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string? path = Path.Combine(localApplicationData, "ClinetOnline");
                Directory.CreateDirectory(path);
                string? stateFilePath = Path.Combine(path, "State.Json");

                await File.WriteAllTextAsync(stateFilePath, stateJson);

                Started();
            }
        }

        public RelayCommand StartCommand { get; private set; }
    }
}
