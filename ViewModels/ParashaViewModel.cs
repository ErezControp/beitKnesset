using BeitKnesetDisplay.Models;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;


namespace BeitKnesetDisplay.ViewModels
{
    public class ParashaViewModel
    {
        public string ParashaName { get; set; } = "";

        public string ParashaTopics { get; set; } = "";

        public string RebbeMessage { get; set; } = "";

        public ParashaViewModel(string parashaName)
        {
            ParashaName = "TEST";

            ParashaTopics = "TEST TOPICS";

            RebbeMessage = "TEST MESSAGE";

            LoadParasha(parashaName);
        }

        private void LoadParasha(string parashaName)
        {
            string path =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Data",
                    "ParashaContent.json");
            ParashaName = path;
            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var parashot =
                JsonSerializer.Deserialize<
                    Dictionary<string, ParashaInfo>>(json, options);

            if (parashot == null)
                return;

            if (!parashot.TryGetValue(parashaName, out var info))
                return;

            ParashaName =
                $"{parashaName}  Topics={info.Topics.Count}  Msg={(info.Message?.Length ?? 0)}";

            ParashaName = parashaName;

            //ParashaTopics =
            //    string.Join(
            //        Environment.NewLine,
            //        info.Topics.Select(t => $"• {t}"));

            ParashaTopics = "";

            foreach (var topic in info.Topics)
            {
                ParashaTopics += $"• {topic}\n";
            }
            
            RebbeMessage = info.Message;
        }
    }
}