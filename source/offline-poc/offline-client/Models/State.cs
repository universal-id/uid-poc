using System.Text.Json;
using System.Text.Json.Serialization;
using UniversalIdentity.Library;

namespace OfflineClient.Models
{
    public class State
    {

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("selectedIdentity")]
        public string SelectedIdentity { get; set; }

        public string Save()
        {
            string fileName = "State.json";
            string jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(fileName, jsonString);

            return jsonString;
        }

        public State Load()
        {
            string fileName = "State.json";
            string jsonString = File.ReadAllText(fileName);

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new ArgumentNullException(nameof(jsonString));
            }

            State result = JsonSerializer.Deserialize<State>(jsonString) ?? new State();

            if (result is not null)
            {
                Path = result.Path;
                SelectedIdentity = result.SelectedIdentity;
                return result;
            }

            return new State();
        }

        public static void StartCommunications()
        {
            var state = new State().Load();
            if (string.IsNullOrEmpty(state.Path)) throw new System.Exception();
            State.IdBoxService = new IdBoxService(state.Path);
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
                if(idBoxService != value
                    && idBoxService != null)
                {
                    idBoxService.Communication.Dispose();
                }

                idBoxService = value;
            }
        }
    }
}
