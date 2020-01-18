using RGiesecke.DllExport;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.InteropServices;

namespace ArmaHook
{
    public class DllEntry
    {
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
        public static void RVExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string input)
        {
            outputSize--;

            string DEFAULT_ERROR = "[ArmaHook ERROR]: ";
            string DEFAULT_SUCCESS = "[ArmaHook SUCCESS]: ";
            Dictionary<string, string> settings;

            settings = SettingsManager.LoadSettings();
            bool rawContent = input[0] == '{' && input[input.Length - 1] == '}';
            string json;
            if (rawContent)
            {
                json = input;
            }
            else
            {
                if (settings.ContainsKey("UseCustomEmbedSettings") && bool.Parse(settings["UseCustomEmbedSettings"]))
                {
                    json = GenerateEmbed(input);
                }
                else
                {
                    json = GenerateRawTextJson(input);
                }
            }
            try
            {
                SendRequest(json, settings);
            }
            catch (WebException e)
            {
                WebResponse r = e.Response;
                output.Append(DEFAULT_ERROR + "The post request failed to Discord with the content " + json + " and the response " + e.Message);
                return;
            }
            output.Append(DEFAULT_SUCCESS + "Webhook fired successfully with content " + json);
            return;
        }

        public static bool SendRequest(string input, Dictionary<string, string> settings)
        {
            string url = (string)settings["DiscordURL"];
            using (WebClient wb = new WebClient())
            {
                wb.Headers.Add("Content-Type", "application/json");
                wb.UploadString(url, "POST", input);
                return true;
            }
        }

        public static string GenerateEmbed(string textToDisplay)
        {
            Dictionary<string, List<object>> baseObject = new Dictionary<string, List<object>>();
            Dictionary<string, object> initialEmbedSettings = SettingsManager.LoadEmbedSettings();
            initialEmbedSettings.Add("description", textToDisplay);
            List<object> embedsList = new List<object>();
            embedsList.Add(initialEmbedSettings);
            baseObject.Add("embeds", embedsList);
            return JsonConvert.SerializeObject(baseObject);
        }

        public static string GenerateRawTextJson(string textToDisplay)
        {
            return "{\"content\": \"" + textToDisplay + "\"}";
        }
    }
}
