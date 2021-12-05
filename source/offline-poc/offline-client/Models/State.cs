using System.Text.Json;
using System.Text.Json.Serialization;

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
    }
}
