using ClinetOnline;
using ClinetOnline.Data;
using ClinetOnline.Extensions;
using ClinetOnline.Options;
using ClinetOnline.ViewModels;
using ClinetOnline.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using System.Diagnostics;
using Application = Microsoft.Maui.Controls.Application;

namespace OnlineClient
{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            IConfigurationBuilder? configurationBuilder = new ConfigurationBuilder()
         //.SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = configurationBuilder.Build();


            ServiceCollection? serviceCollection = new();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            //MainPage = ServiceProvider.GetRequiredService<MainWindow>();

             _mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            BindingContext = _mainWindowViewModel;
            _mainWindowViewModel.LoadAsync();
            _mainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;

            if(_mainWindowViewModel.CurrentViewModel is StartViewModel)
                Shell.Current.GoToAsync("///start");
            else
                Shell.Current.GoToAsync("///addedit");

        }

        private  void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentViewModel")
            {
                if (_mainWindowViewModel.CurrentViewModel is ContactsViewModel contactsViewModel)
                {
                    contactsViewModel.LoadAsync();
                }
            }
        }

        public IServiceProvider ServiceProvider { get; private set; }

        private MainWindowViewModel _mainWindowViewModel;

        public IConfiguration Configuration { get; private set; }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClientServices();
            services.AddViewModels();
            //services.AddSingleton<MainWindow>();
            services.Configure<UrlsOptions>(Configuration.GetSection(UrlsOptions.Urls));

            services.AddSingleton<IContactRepository, ContactRepository>();

            Routing.RegisterRoute("start", typeof(StartView));
            Routing.RegisterRoute("addedit", typeof(AddEditContactView));
            Routing.RegisterRoute("contacts", typeof(ContactsView));
            Routing.RegisterRoute("connect", typeof(ConnectView));
            Routing.RegisterRoute("settings", typeof(SettingsView));
            
            Shell.Current.GoToAsync("///settings");
           
        }

        void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("///settings");
        }

        void Navigated(object sender, System.EventArgs e)
        {
            string destination = ((ShellNavigatedEventArgs)e).Current.Location.OriginalString[2..];
            switch (destination)
            {
                case "dashboard":
                    ((MainWindowViewModel)BindingContext).CurrentViewModel = ServiceProvider.GetRequiredService<ContactsViewModel>();
                    break;
                case "contacts":
                    ((MainWindowViewModel)BindingContext).CurrentViewModel = ServiceProvider.GetRequiredService<ContactsViewModel>();
                    break;
                case "connect":
                    ((MainWindowViewModel)BindingContext).CurrentViewModel = ServiceProvider.GetRequiredService<ConnectViewModel>();
                    break;
                case "share":
                    break;
                case "settings":
                    break;
                case "addedit":
                    ((MainWindowViewModel)BindingContext).CurrentViewModel = ServiceProvider.GetRequiredService<AddEditContactViewModel>();
                    break;
                default:
                    ((MainWindowViewModel)BindingContext).CurrentViewModel = ServiceProvider.GetRequiredService<StartViewModel>();

                    break;
            }
        }
    }
}

