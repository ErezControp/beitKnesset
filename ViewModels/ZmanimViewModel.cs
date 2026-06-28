using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeitKnesetDisplay.ViewModels
{
    public class ZmanimViewModel : INotifyPropertyChanged
    {
        private string _sunrise = "05:35";
        private string _sunset = "19:50";
        private string _sofZmanShma = "09:09";
        private string _chatzot = "12:43";
        private string _minchaGedola = "13:19";
        private string _minchaKetana = "16:52";
        private string _plagHaMincha = "18:21";

        public string Sunrise
        {
            get => _sunrise;
            set
            {
                _sunrise = value;
                OnPropertyChanged();
            }
        }

        public string Sunset
        {
            get => _sunset;
            set
            {
                _sunset = value;
                OnPropertyChanged();
            }
        }

        public string SofZmanShma
        {
            get => _sofZmanShma;
            set
            {
                _sofZmanShma = value;
                OnPropertyChanged();
            }
        }

        public string Chatzot
        {
            get => _chatzot;
            set
            {
                _chatzot = value;
                OnPropertyChanged();
            }
        }

        public string MinchaGedola
        {
            get => _minchaGedola;
            set
            {
                _minchaGedola = value;
                OnPropertyChanged();
            }
        }

        public string MinchaKetana
        {
            get => _minchaKetana;
            set
            {
                _minchaKetana = value;
                OnPropertyChanged();
            }
        }

        public string PlagHaMincha
        {
            get => _plagHaMincha;
            set
            {
                _plagHaMincha = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}