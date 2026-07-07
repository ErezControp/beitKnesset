using BeitKnesetDisplay.Models;
using System.IO;
using System.Text.Json;

namespace BeitKnesetDisplay.ViewModels
{
    public class ParashaViewModel
    {
        public string ParashaName { get; set; } = "";

        public string ParashaTopics { get; set; } = "";

        public string RebbeMessage { get; set; } = "";

        public ParashaViewModel(string parashaName)
        {
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

            var parashot =
                JsonSerializer.Deserialize<
                    Dictionary<string, ParashaInfo>>(json);

            if (parashot == null)
                return;

            if (!parashot.TryGetValue(parashaName, out var info))
                return;

            ParashaName = parashaName;

            ParashaTopics =
                string.Join(
                    Environment.NewLine,
                    info.Topics.Select(t => $"• {t}"));

            RebbeMessage = info.Message;
        }
    }
}