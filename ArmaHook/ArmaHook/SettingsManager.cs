using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmaHook
{
    class SettingsManager
    {
        public static Dictionary<string, string> LoadSettings()
        {
            string jsonString = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.json");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public static Dictionary<string, object> LoadEmbedSettings()
        {
            string jsonString = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\embed.json");
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
        }
    }
}
