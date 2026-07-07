using BeitKnesetDisplay.ViewModels;
using System.Windows.Controls;

namespace BeitKnesetDisplay.Views
{
    public partial class ParashaView : UserControl
    {
        public ParashaView(string parasha)
        {
            InitializeComponent();

            DataContext = new ParashaViewModel(parasha);
        }
    }
}