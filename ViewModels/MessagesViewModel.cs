using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeitKnesetDisplay.ViewModels
{
    public class MessagesViewModel : INotifyPropertyChanged
    {
        private string _mainMessage = "ברוכים הבאים לבית הכנסת";
        private string _subMessage = "שחרית | מנחה | ערבית";

        public string MainMessage
        {
            get => _mainMessage;
            set
            {
                _mainMessage = value;
                OnPropertyChanged();
            }
        }

        public string SubMessage
        {
            get => _subMessage;
            set
            {
                _subMessage = value;
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