using BeitKnesetDisplay.Services;
using BeitKnesetDisplay.ViewModels;
using System.Runtime.InteropServices;
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
            PowerManager.PreventSleep();
            var httpClient = new HttpClient();
            IHebcalService service = new HebcalService(httpClient);

            DataContext = new MainViewModel(service);

        }
        public static class PowerManager
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern uint SetThreadExecutionState(uint esFlags);

            private const uint ES_CONTINUOUS = 0x80000000;
            private const uint ES_SYSTEM_REQUIRED = 0x00000001;
            private const uint ES_DISPLAY_REQUIRED = 0x00000002;

            public static void PreventSleep()
            {
                SetThreadExecutionState(
                    ES_CONTINUOUS |
                    ES_SYSTEM_REQUIRED |
                    ES_DISPLAY_REQUIRED);
            }
        }
    }
}