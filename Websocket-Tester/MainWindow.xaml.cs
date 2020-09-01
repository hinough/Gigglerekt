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
using obs_websocket.Types;

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

            con.onActiveSceneChange += Con_onActiveSceneChange;
            con.onScenelistUpdate += Con_onScenelistUpdate;

            con.connect(); 
        }

        private void Con_onActiveSceneChange(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblactive.Content = "Active scene: " + e;
                tbOutput.Text += "Active scene changed to "+ e + "\n";
            });
        }

        private void Con_onScenelistUpdate(object sender, List<Scene> scenes)
        {
            this.Dispatcher.Invoke(() =>
            {
                cbScenes.Items.Clear();

                foreach (Scene scene in scenes)
                {
                    cbScenes.Items.Add(scene.name);
                }

                cbScenes.SelectedItem = ((string)lblactive.Content).Replace("Active scene: ", "");

                tbOutput.Text += "Scenelist updated\n";
            });
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            con.getSceneList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            con.setCurrentScene((string)cbScenes.SelectedItem);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            con.setMute("Audio Output 1", true);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            con.setMute("Audio Output 1", false);
        }
    }
}
