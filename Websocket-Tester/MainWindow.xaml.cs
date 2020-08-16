using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using obs_websocket;

namespace Websocket_Tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObsConnection con;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void init()
        {
            con = new ObsConnection("127.0.0.1", "4444", "password");
            con.onAuthFailed += Con_onAuthFailed;
            con.onAuthSuccess += Con_onAuthSuccess;
            con.onConnected += Con_onConnected;
            con.onDisconnected += Con_onDisconnected;
            con.onInformation += Con_onInformation;

            con.connect(); 
        }

        private void Con_onDisconnected(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbOutput.Text += e + "\n";
            });
        }

        private void Con_onInformation(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbOutput.Text += e + "\n";
            });
        }

        private void Con_onConnected(object sender, string e)
        {
           this.Dispatcher.Invoke(() =>
           {
               tbOutput.Text += e + "\n";
           });
        }

        private void Con_onAuthFailed(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbOutput.Text += e + "\n";
            });
        }

        private void Con_onAuthSuccess(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbOutput.Text += e + "\n";
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           init();
        }
    }
}
