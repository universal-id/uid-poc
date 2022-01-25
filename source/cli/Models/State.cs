using System.Text.Json;
using System.Text.Json.Serialization;
using UniversalIdentity.Library;

namespace UniversalIdentity.Cli.Models
{
    public class State
    {
        public State()
        {
        }

        public State(string stateFilePath) : this("", "", stateFilePath)
        {
            Load();
        }

        public State(string path, string selectedIdentity, string stateFilePath)
        {
            Path = path;
            SelectedIdentity = selectedIdentity;
            StateFilePath = stateFilePath;
        }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("selectedIdentity")]
        public string SelectedIdentity { get; set; }
        public string StateFilePath { get; init; }

        public string Save()
        {
            string fileName = StateFilePath;
            string jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(fileName, jsonString);

            return jsonString;
        }

        public State Load()
        {
            string fileName = StateFilePath;
            string jsonString = File.ReadAllText(fileName);

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new ArgumentNullException(nameof(jsonString));
            }

            State result = JsonSerializer.Deserialize<State>(jsonString) ?? new State();

            Path = result.Path;
            SelectedIdentity = result.SelectedIdentity;

            return result;
        }

        public void StartCommunications()
        {
            if (string.IsNullOrEmpty(Path)) throw new Exception();
            State.IdBoxService = new IdBoxService(Path);
        }

        private static IdBoxService idBoxService;
        public static IdBoxService IdBoxService
        {
            get
            {
                return idBoxService;
            }

            set
            {
                if (idBoxService != value
                    && idBoxService != null)
                {
                    idBoxService.Communication.Dispose();
                }

                idBoxService = value;
            }
        }

    }
}
