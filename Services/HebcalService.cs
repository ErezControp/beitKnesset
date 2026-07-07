using BeitKnesetDisplay.Models;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BeitKnesetDisplay.Services
{
    public class HebcalService : IHebcalService
    {
        private readonly HttpClient _httpClient;

        private const int GeoNameId = 294760; // Hod HaSharon

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private static string ExtractTime(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "—";

            int idx = text.LastIndexOf(':');

            if (idx < 2)
                return text;

            return text.Substring(idx - 2);
        }
        private static (string candles, string havdalah)
            ExtractShabbatTimes(ShabbatResponse? response)
        {
            if (response?.Items == null)
                return ("—", "—");

            string candles = "—";
            string havdalah = "—";

            foreach (var item in response.Items)
            {
                if (item.Category == "candles")
                {
                    candles = ExtractTime(item.Title);
                }

                if (item.Category == "havdalah")
                {
                    havdalah = ExtractTime(item.Title);
                }
            }

            return (candles, havdalah);
        }
        public HebcalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(8);

            if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                _httpClient.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("BeitKnesetDisplay", "1.0"));
            }
        }

        public async Task<DisplayData> GetDisplayDataAsync()
        {
            var today = DateTime.Now.Date;
            string date = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            string zmanimUrl =
                $"https://www.hebcal.com/zmanim?cfg=json&geonameid={GeoNameId}&date={date}";

            DateTime upcomingShabbat = GetUpcomingShabbat(today);

            string shabbatUrl =
                $"https://www.hebcal.com/shabbat?cfg=json&geonameid={GeoNameId}" +
                $"&gy={upcomingShabbat.Year}&gm={upcomingShabbat.Month}&gd={upcomingShabbat.Day}" +
                $"&lg=he-x-NoNikud&M=on";

            string zmanimCachePath = GetCachePath($"zmanim_{date}.json");
            string shabbatCachePath = GetCachePath($"shabbat_{date}.json");

            var result = new DisplayData();

            // 1. ZMANIM: API first, cache fallback
            string? zmanimJson = await TryGetApiJsonWithCacheFallbackAsync(
                url: zmanimUrl,
                cachePath: zmanimCachePath,
                seedFileName: "zmanim.json");

            if (!string.IsNullOrWhiteSpace(zmanimJson))
            {
                var parsed = ParseZmanimJson(zmanimJson);

                result.Sunrise = parsed.Sunrise;
                result.Sunset = parsed.Sunset;
                result.SofZmanShma = parsed.SofZmanShma;
                result.Chatzot = parsed.Chatzot;
                result.MinchaGedola = parsed.MinchaGedola;
                result.MinchaKetana = parsed.MinchaKetana;
                result.PlagHaMincha = parsed.PlagHaMincha;

                result.StatusMessage = parsed.StatusMessage;
            }
            else
            {
                result.StatusMessage = "לא נמצאו זמני היום - אין API ואין cache";
            }

            // 2. PARASHA: API first, cache fallback
            string? shabbatJson = await TryGetApiJsonWithCacheFallbackAsync(
                url: shabbatUrl,
                cachePath: shabbatCachePath,
                seedFileName: "shabbat.json");

            if (!string.IsNullOrWhiteSpace(shabbatJson))
            {
                result.Parasha = ParseParashaJson(shabbatJson);

                var shabbat =
                    JsonSerializer.Deserialize<ShabbatResponse>(
                        shabbatJson,
                        _jsonOptions);

                var times = ExtractShabbatTimes(shabbat);

                result.CandleLighting = times.candles;
                result.Havdalah = times.havdalah;
            }

            return result;
        }

        private async Task<string?> TryGetApiJsonWithCacheFallbackAsync(
            string url,
            string cachePath,
            string seedFileName)
        {
            // A. Try API
            try
            {
                string? apiJson = await TryGetJsonFromApiAsync(url);

                if (!string.IsNullOrWhiteSpace(apiJson) && LooksLikeJson(apiJson))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
                    await File.WriteAllTextAsync(cachePath, apiJson);

                    return apiJson;
                }
            }
            catch
            {
                // בכוונה לא זורקים החוצה.
                // עוברים ל-cache.
            }

            // B. Try today's cache
            try
            {
                if (File.Exists(cachePath))
                {
                    return await File.ReadAllTextAsync(cachePath);
                }
            }
            catch
            {
                // ממשיכים ל-seed
            }

            // C. Try seed file from output directory
            try
            {
                string seedPath = Path.Combine(AppContext.BaseDirectory, seedFileName);

                if (File.Exists(seedPath))
                {
                    return await File.ReadAllTextAsync(seedPath);
                }
            }
            catch
            {
                // no-op
            }

            return null;
        }

        private async Task<string?> TryGetJsonFromApiAsync(string url)
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    using var response = await _httpClient.GetAsync(url);
                    string content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && LooksLikeJson(content))
                    {
                        return content;
                    }

                    // אם קיבלנו HTML כמו Application Blocked — אין טעם לפרסר.
                    if (!LooksLikeJson(content))
                    {
                        Console.WriteLine("Non-JSON response received:");
                        Console.WriteLine(content.Substring(0, Math.Min(content.Length, 300)));
                    }

                    await Task.Delay(1000 * attempt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(1000 * attempt);
                }
            }

            return null;
        }

        private DisplayData ParseZmanimJson(string json)
        {
            var result = new DisplayData();

            try
            {
                var zmanim = JsonSerializer.Deserialize<ZmanimResponse>(json, _jsonOptions);

                if (zmanim?.Times == null)
                {
                    result.StatusMessage = "JSON נטען אך לא נמצאו times";
                    return result;
                }

                result.Sunrise = FormatIsoTime(zmanim.Times.Sunrise);
                result.Sunset = FormatIsoTime(zmanim.Times.Sunset);
                result.SofZmanShma = FormatIsoTime(zmanim.Times.SofZmanShma);
                result.Chatzot = FormatIsoTime(zmanim.Times.Chatzot);
                result.MinchaGedola = FormatIsoTime(zmanim.Times.MinchaGedola);
                result.MinchaKetana = FormatIsoTime(zmanim.Times.MinchaKetana);
                result.PlagHaMincha = FormatIsoTime(zmanim.Times.PlagHaMincha);

                result.StatusMessage = "";
                return result;
            }
            catch (Exception ex)
            {
                result.StatusMessage = $"שגיאה בקריאת JSON: {ex.Message}";
                return result;
            }
        }

        private string ParseParashaJson(string json)
        {
            try
            {
                var shabbat = JsonSerializer.Deserialize<ShabbatResponse>(json, _jsonOptions);
                return ExtractParasha(shabbat);
            }
            catch
            {
                return "פרשת השבוע: —";
            }
        }

        private static string ExtractParasha(ShabbatResponse? response)
        {
            if (response?.Items == null || response.Items.Count == 0)
                return "פרשת השבוע: —";

            var parashaItem = response.Items
                .FirstOrDefault(i =>
                    string.Equals(i.Category, "parashat", StringComparison.OrdinalIgnoreCase));

            if (parashaItem == null)
                return "פרשת השבוע: —";

            string raw = !string.IsNullOrWhiteSpace(parashaItem.Hebrew)
                ? parashaItem.Hebrew!
                : parashaItem.Title ?? "—";

            return raw.StartsWith("פרשת") ? raw : $"פרשת {raw}";
        }

        private static string FormatIsoTime(string? isoString)
        {
            if (string.IsNullOrWhiteSpace(isoString))
                return "—";

            if (DateTimeOffset.TryParse(isoString, out var dto))
                return dto.ToLocalTime().ToString("HH:mm");

            return "—";
        }

        private static bool LooksLikeJson(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            string trimmed = content.TrimStart();

            return trimmed.StartsWith("{") || trimmed.StartsWith("[");
        }

        private static string GetCachePath(string fileName)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BeitKnesetDisplay",
                "Cache");

            return Path.Combine(folder, fileName);
        }
        private static DateTime GetUpcomingShabbat(DateTime date)
        {
            int daysToAdd = ((int)DayOfWeek.Saturday - (int)date.DayOfWeek + 7) % 7;

            if (daysToAdd == 0)
                return date; // כבר שבת

            return date.AddDays(daysToAdd);
        }
    }
}