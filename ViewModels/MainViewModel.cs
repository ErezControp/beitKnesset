using BeitKnesetDisplay.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Globalization;

namespace BeitKnesetDisplay.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IHebcalService _service;

        private DispatcherTimer _clockTimer = new DispatcherTimer();
        private DispatcherTimer _screenTimer = new DispatcherTimer();


        private int _screenIndex = 0;

        private string _currentTime = "";
        private string _hebrewDate = "";
        private string _parasha = "";
        private object _currentView = new object();

        private ZmanimViewModel _zmanimViewModel;
        private MessagesViewModel _messagesViewModel;
        private SoldiersViewModel _soldiersViewModel;
        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public string HebrewDate
        {
            get => _hebrewDate;
            set
            {
                _hebrewDate = value;
                OnPropertyChanged();
            }
        }

        public string Parasha
        {
            get => _parasha;
            set
            {
                _parasha = value;
                OnPropertyChanged();
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private string _sunrise;
        public string Sunrise
        {
            get => _sunrise;
            set { _sunrise = value; OnPropertyChanged(); }
        }

        private string _sunset;
        public string Sunset
        {
            get => _sunset;
            set { _sunset = value; OnPropertyChanged(); }
        }

        private string _sofZmanShma;
        public string SofZmanShma
        {
            get => _sofZmanShma;
            set { _sofZmanShma = value; OnPropertyChanged(); }
        }

        private string _chatzot;
        public string Chatzot
        {
            get => _chatzot;
            set { _chatzot = value; OnPropertyChanged(); }
        }

        private string _minchaGedola;
        public string MinchaGedola
        {
            get => _minchaGedola;
            set { _minchaGedola = value; OnPropertyChanged(); }
        }

        private string _minchaKetana;
        public string MinchaKetana
        {
            get => _minchaKetana;
            set { _minchaKetana = value; OnPropertyChanged(); }
        }

        private string _plagHaMincha;
        public string PlagHaMincha
        {
            get => _plagHaMincha;
            set { _plagHaMincha = value; OnPropertyChanged(); }
        }
        private string _holidayText;
        public string HolidayText
        {
            get => _holidayText;
            set { _holidayText = value; OnPropertyChanged(); }
        }
        private string _dayName;
        public string DayName
        {
            get => _dayName;
            set { _dayName = value; OnPropertyChanged(); }
        }
        private string _gregorianDate;
        public string GregorianDate
        {
            get => _gregorianDate;
            set { _gregorianDate = value; OnPropertyChanged(); }
        }
        private async void LoadZmanim()
        {
            if (_service == null)
                return;

            var data = await _service.GetDisplayDataAsync();

            Sunrise = data.Sunrise;
            Sunset = data.Sunset;
            SofZmanShma = data.SofZmanShma;
            Chatzot = data.Chatzot;
            MinchaGedola = data.MinchaGedola;
            MinchaKetana = data.MinchaKetana;
            PlagHaMincha = data.PlagHaMincha;
            Parasha = data.Parasha;
            OnPropertyChanged(nameof(Parasha));
        }
        
        private static string ToHebrewNumber(int number)
        {
            number = number % 1000; // 🔥 חשוב מאוד!

            string[] units = { "", "א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט" };
            string[] tens = { "", "י", "כ", "ל", "מ", "נ", "ס", "ע", "פ", "צ" };

            string result = "";

            // מאות
            while (number >= 400)
            {
                result += "ת";
                number -= 400;
            }

            if (number >= 100)
            {
                string[] hundreds = { "", "ק", "ר", "ש" };
                result += hundreds[number / 100];
                number %= 100;
            }

            // 15,16
            if (number == 15) return result + "ט״ו";
            if (number == 16) return result + "ט״ז";

            // עשרות
            if (number >= 10)
            {
                result += tens[number / 10];
                number %= 10;
            }

            // יחידות
            if (number > 0)
                result += units[number];

            // גרשיים
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Length == 1)
                    result += "׳";
                else
                    result = result.Insert(result.Length - 1, "״");
            }

            return result;
        }
        private string GetHoliday(int month, int day)
        {
            return (month, day) switch
            {
                (1, 1) => "ראש השנה",
                (1, 10) => "יום כיפור",
                (1, 15) => "סוכות",
                (1, 22) => "שמחת תורה",

                (3, 25) => "חנוכה",
                (3, 26) => "חנוכה",
                (3, 27) => "חנוכה",

                (7, 15) => "פסח",
                (7, 21) => "שביעי של פסח",

                (9, 6) => "שבועות",

                _ => ""
            };
        }
        private string GetFullHebrewDate(string sunset)
        {
            DateTime now = DateTime.Now;

            // אחרי שקיעה
            if (DateTime.TryParse(sunset, out var sunsetTime))
            {
                if (now.TimeOfDay > sunsetTime.TimeOfDay)
                    now = now.AddDays(1);
            }

            HebrewCalendar hc = new HebrewCalendar();

            int day = hc.GetDayOfMonth(now);
            int month = hc.GetMonth(now);
            int year = hc.GetYear(now);

            string[] months =
            {
                "", "תשרי", "חשוון", "כסלו", "טבת", "שבט",
                "אדר", "ניסן", "אייר", "סיון", "תמוז", "אב", "אלול"
            };

            string[] daysOfWeek =
            {
                "יום ראשון",
                "יום שני",
                "יום שלישי",
                "יום רביעי",
                "יום חמישי",
                "יום שישי",
                "שבת"
            };

            string hebrewDay = ToHebrewNumber(day);
            string hebrewYear = ToHebrewNumber(year % 1000);

            // ✅ ערב שבת
            string prefix = daysOfWeek[(int)now.DayOfWeek];
            if (now.DayOfWeek == DayOfWeek.Friday)
                prefix = "ערב שבת קודש";

            // ✅ ראש חודש
            string roshChodesh = "";
            if (day == 1)
                roshChodesh = "ראש חודש!";
            if (day == 30)
                roshChodesh = "ערב ראש חודש";

            // ✅ חגים בסיסיים
            string holiday = GetHoliday(month, day);

            // ✅ עדכון property
            if (!string.IsNullOrEmpty(holiday))
                HolidayText = holiday;
            else
                HolidayText = roshChodesh; // fallback

            return $"{prefix}, {hebrewDay} ב{months[month]} {hebrewYear} {roshChodesh}";
        }

        public MainViewModel(IHebcalService service)
        {
            _service = service;
                        
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            _zmanimViewModel = new ZmanimViewModel();
            _messagesViewModel = new MessagesViewModel();
            _soldiersViewModel = new SoldiersViewModel();

            StartClock();
            StartScreenRotation();

            ShowScreen();
            LoadZmanim();
        }

        private void StartClock()
        {
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _clockTimer.Tick += (s, e) =>
            {
                var now = DateTime.Now;

                CurrentTime = now.ToString("HH:mm:ss");

                // ✅ יום בשבוע
                DayName = now.ToString("dddd", new System.Globalization.CultureInfo("he-IL"));

                // ✅ תאריך לועזי
                GregorianDate = now.ToString("dd/MM/yyyy");

                // ✅ תאריך עברי כבר עשינו
                if (!string.IsNullOrEmpty(Sunset))
                {
                    HebrewDate = GetFullHebrewDate(Sunset);
                }

            };

            _clockTimer.Start();
        }

        private void StartScreenRotation()
        {
            _screenTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };

            _screenTimer.Tick += (s, e) =>
            {
                _screenIndex = (_screenIndex + 1) % 3;
                ShowScreen();
            };

            _screenTimer.Start();
        }

        private void ShowScreen()
        {
            switch (_screenIndex)
            {
                case 0:
                    CurrentView = _zmanimViewModel;
                    break;

                case 1:
                    CurrentView = _messagesViewModel;
                    break;

                case 2:
                    CurrentView = _soldiersViewModel;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}