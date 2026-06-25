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
            
            CurrentView = new ZmanimView();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
