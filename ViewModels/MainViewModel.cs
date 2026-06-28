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