using BeitKnesetDisplay.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

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
        }
        public MainViewModel(IHebcalService service)
        {
            _service = service;

            HebrewDate = "כ״ב סיון תשפ״ו";
            Parasha = "פרשת השבוע";
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            _zmanimViewModel = new ZmanimViewModel();
            _messagesViewModel = new MessagesViewModel();

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
                CurrentTime = DateTime.Now.ToString("HH:mm:ss");
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
                _screenIndex = (_screenIndex + 1) % 2;
                ShowScreen();
            };

            _screenTimer.Start();
        }

        private void ShowScreen()
        {
            if (_screenIndex == 0)
            {
                CurrentView = _zmanimViewModel;
            }
            else
            {
                CurrentView = _messagesViewModel;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}