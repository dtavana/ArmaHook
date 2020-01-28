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
            SettingsManager settings;
            string json;

            settings = SettingsManager.LoadSettings();

            string[] inputSerialized = input.Split('~');
            if (inputSerialized.Length != 2)
            {
                output.Append(DEFAULT_ERROR + "Could not parse input");
                return;
            }

            string embedKey = inputSerialized[0];
            string inputText = inputSerialized[1];
            bool rawContent = inputText[0] == '{' && inputText[inputText.Length - 1] == '}';

            CustomEmbed embedFromKey = SettingsManager.GetEmbedByKey(embedKey, settings);

            if(embedFromKey == null)
            {
                output.Append(DEFAULT_ERROR + "No settings found for the key: " + embedKey);
                return;
            }

            if (rawContent)
            {
                json = inputText;
            }
            else
            {
                if (settings.UseEmbeds)
                {
                    json = GenerateEmbed(inputText, embedFromKey);
                }
                else
                {
                    json = GenerateRawTextJson(inputText);
                }
            }
            try
            {
                SendRequest(json, embedFromKey);
            }
            catch (WebException e)
            {
                output.Append(DEFAULT_ERROR + "The post request failed to Discord with the content " + json + " and the response " + e.Message);
                return;
            }
            output.Append(DEFAULT_SUCCESS + "Webhook fired successfully with content " + json);
            return;
        }

        public static bool SendRequest(string input, CustomEmbed embedSettings)
        {
            string url = embedSettings.DiscordURL;
            using (WebClient wb = new WebClient())
            {
                wb.Headers.Add("Content-Type", "application/json");
                wb.UploadString(url, "POST", input);
                return true;
            }
        }

        public static string GenerateEmbed(string textToDisplay, CustomEmbed embed)
        {

            Dictionary<string, List<object>> baseObject = new Dictionary<string, List<object>>();
            embed.EmbedData.Add("description", textToDisplay);
            List<object> embedsList = new List<object>();
            embedsList.Add(embed.EmbedData);
            baseObject.Add("embeds", embedsList);
            return JsonConvert.SerializeObject(baseObject);
        }

        public static string GenerateRawTextJson(string textToDisplay)
        {
            return "{\"content\": \"" + textToDisplay + "\"}";
        }
    }
}
