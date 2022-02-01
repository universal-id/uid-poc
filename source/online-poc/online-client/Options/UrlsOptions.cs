namespace ClinetOnline.Options
{
    public class UrlsOptions
    {
        public const string Urls = "Urls";

        public string OnlineServiceUrl { get; set; }

        public class OnlineService
        {
            public class AppsController
            {
                public static string NotifyAppEvent() => $"/api/apps";
            }
        }
    }
}
