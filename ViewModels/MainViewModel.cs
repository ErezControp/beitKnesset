using BeitKnesetDisplay.Services;
using BeitKnesetDisplay.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace BeitKnesetDisplay.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _currentTime;
        private readonly IHebcalService _service;
        private DispatcherTimer _screenTimer;
        private int _screenIndex = 0;
        public string Sunrise { get; set; } = "05:35";
        public string Sunset { get; set; } = "19:50";
        public string SofZmanShma { get; set; } = "09:09";
        public string Chatzot { get; set; } = "12:43";
        public string MinchaGedola { get; set; } = "13:19";
        public string MinchaKetana { get; set; } = "16:52";
        public string PlagHaMincha { get; set; } = "18:21";

        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(); }
        }
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }
        public MainViewModel(IHebcalService service)
        {
            _service = service;

            // שעון
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            };
            timer.Start();

            // התחלה
            ShowScreen();

            // מעבר בין מסכים
            _screenTimer = new DispatcherTimer();
            _screenTimer.Interval = TimeSpan.FromSeconds(10);
            _screenTimer.Tick += (s, e) =>
            {
                _screenIndex++;
                ShowScreen();
            };
            _screenTimer.Start();
        }
        private void ShowScreen()
        {
            int screen = _screenIndex % 2;

            if (screen == 0)
            {
                CurrentView = new ZmanimView
                {
                    DataContext = this
                };
            }
            else
            {
                CurrentView = new MessagesView();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
