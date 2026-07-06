using System;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeitKnesetDisplay.ViewModels
{
    public class SoldiersViewModel : INotifyPropertyChanged
    {
        private readonly string[] _verses =
        {
            "אם אשכחך ירושלים תשכח ימיני",
            "שומר ישראל שמור שארית ישראל",
            "ה׳ עוז לעמו יתן ה׳ יברך את עמו בשלום",
            "כי מציון תצא תורה ודבר ה׳ מירושלים",
            "הנה לא ינום ולא יישן שומר ישראל",
            "עושה שלום במרומיו הוא יעשה שלום עלינו ועל כל ישראל ואמרו אמן",
            "חזק ונתחזק בעד עמנו ובעד ערי אלוקינו"
        };

        private int _index;

        private string _currentVerse;

        public string CurrentVerse
        {
            get => _currentVerse;
            set
            {
                _currentVerse = value;
                OnPropertyChanged();
            }
        }

        public SoldiersViewModel()
        {
            CurrentVerse = _verses[0];

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };

            timer.Tick += (s, e) =>
            {
                _index++;
                CurrentVerse = _verses[_index % _verses.Length];
            };

            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}