using Crude_Bot.Properties;
using obs_websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
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

namespace Crude_Bot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bot chatbot = null;
        ObsConnection obs = null;

        DateTime lastcommand, nextcommand;

        int oldcd = 5;
        public MainWindow()
        {
            InitializeComponent();

            while(Settings.Default.username.Equals(""))
            {
                Settings.Default.username = Interaction.InputBox("Whats my username?", "Input Twitch Username", "gigglerektbot");
            }

            while(Settings.Default.token.Equals(""))
            {
                Settings.Default.token = Interaction.InputBox("Whats my twitch token? https://twitchapps.com/tmi/", "Input Twitch Oauth token", "oauth:xxxxxx");
            }

            while(Settings.Default.channel.Equals(""))
            {
                Settings.Default.channel = Interaction.InputBox("What channel do I connect to?","Twitch Channel","gigglerekt");
            }

            Settings.Default.Save();

            chatbot = new Bot(Settings.Default.username, Settings.Default.token, Settings.Default.channel);

            chatbot.onCommand += commandReceived;
            chatbot.onConnected += connected;

            lastcommand = DateTime.Now.AddMinutes(-5.0);
            //Scenes 
            //  Side by Side-> Britt     !both
            //  Britt Focus -> Britt     !britt
            //  Crafty Focus-> Rach      !crafty

            //Sources
            //  RACH GAMEPLAY
            //  BRITT GAMPEPLAY
        }

        public void commandReceived(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                string[] messages = e.Split(';');
                string username = messages[0];
                string message = messages[1];

                if (DateTime.Now > lastcommand.AddMinutes(double.Parse(tbTwitchCD.Text)))
                {
                    if (message.ToLower().StartsWith("!britt"))
                    {
                        obs.setCurrentScene("Britt Focus");

                        obs.setMute("RACH GAMEPLAY", true);
                        obs.setMute("BRITT GAMEPLAY", false);

                        chatbot.reply("Command successful @" + username + "! Scene switching to Brittca");
                    }
                    else if (message.ToLower().StartsWith("!crafty"))
                    {
                        obs.setCurrentScene("Crafty Focus");

                        obs.setMute("RACH GAMEPLAY", false);
                        obs.setMute("BRITT GAMEPLAY", true);

                        chatbot.reply("Command successful @" + username + "! Scene switching to Crafty");
                    }
                    else if (message.ToLower().StartsWith("!both"))
                    {
                        obs.setCurrentScene("Side by Side");

                        obs.setMute("RACH GAMEPLAY", true);
                        obs.setMute("BRITT GAMEPLAY", false);

                        chatbot.reply("Command successful @" + username + "! Scene switching to Both");
                    }

                    lastcommand = DateTime.Now;
                    nextcommand = lastcommand.AddMinutes(double.Parse(tbTwitchCD.Text));
                }
                else
                {
                    if(oldcd != int.Parse(tbTwitchCD.Text))
                    {
                        if(oldcd > int.Parse(tbTwitchCD.Text))
                        {
                            nextcommand = nextcommand.AddMinutes(-(oldcd - int.Parse(tbTwitchCD.Text)));
                        }
                        else
                        {
                            nextcommand = nextcommand.AddMinutes(int.Parse(tbTwitchCD.Text) + oldcd);
                        }

                        oldcd = int.Parse(tbTwitchCD.Text);
                    }

                    int time = (int)(nextcommand - DateTime.Now).TotalSeconds;

                    Console.Out.WriteLine(time);

                    int minutes = time / 60; //INT WILL REMOVE DECIMALS
                    int seconds = time % 60;

                    if (message.ToLower().StartsWith("!britt"))
                    {
                        chatbot.reply(string.Format("Scene Brittca is on cooldown, please try again later", minutes, seconds));
                    }
                    else if (message.ToLower().StartsWith("!crafty"))
                    {
                        chatbot.reply(string.Format("Scene Crafty is on cooldown, please try again later", minutes, seconds));
                    }
                    else if (message.ToLower().StartsWith("!both"))
                    {
                        chatbot.reply(string.Format("Scene Side by Side is on cooldown, please try again later", minutes, seconds));
                    }
                }
            });
        }

        public void connected(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblTwitchStatus.Content = "Connected to twitch";
                btnCDC.Content = "Disconnect from Twitch";
            });
        }

        public void disconnected(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblTwitchStatus.Content = "Disconnected from twitch";
                btnCDC.Content = "Connect to Twitch";
            });
        }


        private void connectDisconnect(object sender, RoutedEventArgs e)
        {
            if (chatbot.isConnected()) chatbot.disconnect();
            else chatbot.connect();
        }

        private void connectDisconnectOBS(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tbObsIp.Text) ||
               string.IsNullOrEmpty(tbObsPort.Text))
            {
                MessageBox.Show("OBS IP and port must be given, password too if its set in OBS");
            }
            else
            {
                string password = "";

                if (!string.IsNullOrEmpty(tbObsPassword.Text)) password = tbObsPassword.Text;
                lblObsStatus.Content = "Connecting to obs...";
                btnObsCDC.Content = "Disconnect from OBS";

                obs = new ObsConnection(tbObsIp.Text, tbObsPort.Text, password);

                obs.onAuthFailed += Obs_onAuthFailed;
                obs.onAuthSuccess += Obs_onAuthSuccess;
                obs.onConnected += Obs_onConnected;
                obs.onActiveSceneChange += Obs_onActiveSceneChange;

                obs.connect();
            }
        }

        private void Obs_onActiveSceneChange(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblObsStatus.Content = "Changed to scene: " + e;
            });
        }

        private void Obs_onConnected(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblObsStatus.Content = "Connected to OBS";
            });
        }

        private void Obs_onAuthSuccess(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblObsStatus.Content = "Given Password was correct";
            });
        }

        private void Obs_onAuthFailed(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblObsStatus.Content = "Given Password was wrong";
            });
        }
    }
}
