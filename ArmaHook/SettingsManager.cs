using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmaHook
{
    class SettingsManager
    {
        public bool UseEmbeds { get; set; }
        public List<CustomEmbed> Embeds { get; set; }
        public static SettingsManager LoadSettings()
        {
            string jsonString = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.json");
            SettingsManager settings = JsonConvert.DeserializeObject<SettingsManager>(jsonString);
            return settings;
        }

        public static CustomEmbed GetEmbedByKey(string key, SettingsManager settings)
        {
            foreach (CustomEmbed embed in settings.Embeds)
            {
                if (embed.EmbedKey == key)
                {
                    return embed;
                }
            }
            return null;
        }
    }
}
