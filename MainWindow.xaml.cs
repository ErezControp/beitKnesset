using BeitKnesetDisplay.Services;
using BeitKnesetDisplay.ViewModels;
using System.Net.Http;
using System.Windows;

namespace BeitKnesetDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var httpClient = new HttpClient();
            IHebcalService service = new HebcalService(httpClient);

            DataContext = new MainViewModel(service);

        }
    }
}