using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class DatabaseHandler
    {
        [JsonProperty("DatabaseName")]
        public string DatabaseName { get; set; } = "Hifumi";

        [JsonProperty("RavenDB-URL")]
        public string[] DatabaseUrls { get; set; } = { "http://127.0.0.1:8080" };

        [JsonProperty("X509CertificatePath")]
        private string CertificatePath { get; set; }

        [JsonIgnore]
        public X509Certificate2 Certificate2 { get => !string.IsNullOrWhiteSpace(CertificatePath) ? new X509Certificate2(CertificatePath) : null; }

        public static async Task<DatabaseHandler> LoadDBConfigAsync()
        {
            var dbConfigPath = $"{Directory.GetCurrentDirectory()}/DatabaseConfig.json";
            if (File.Exists(dbConfigPath)) return JsonConvert.DeserializeObject<DatabaseHandler>(await File.ReadAllTextAsync(dbConfigPath));

            await File.WriteAllTextAsync(dbConfigPath, JsonConvert.SerializeObject(new DatabaseHandler(), Formatting.Indented));
            return JsonConvert.DeserializeObject<DatabaseHandler>(await File.ReadAllTextAsync(dbConfigPath));
        }
    }
}
