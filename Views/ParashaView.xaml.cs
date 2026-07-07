using BeitKnesetDisplay.ViewModels;
using System.Windows.Controls;

namespace BeitKnesetDisplay.Views
{
    public partial class ParashaView : UserControl
    {
        public ParashaView()
        {
            InitializeComponent();

            DataContext = new ParashaViewModel("פרשת מטות־מסעי");
        }
    }
}