using System.Text.Json;
using System.Text.Json.Serialization;

namespace OfflineClient.Models
{
    public class State
    {
        public State() : this("State.json")
        {
        }

        public State(string stateFilePath) : this("", "", stateFilePath)
        {
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

            if (result is not null)
            {
                Path = result.Path;
                SelectedIdentity = result.SelectedIdentity;
                return result;
            }

            return new State();
        }
    }
}
