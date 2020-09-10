using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using obs_websocket.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebSocketSharp;

namespace obs_websocket
{
    public class ObsConnection
    {
        private string ip;
        private string password;

        private List<Scene> scenes = null;

        private ObsStats stats = null;

        private WebSocket ws;

        public ObsConnection( string ip, string port = "80", string password = null)
        {
            this.ip = (ip.StartsWith("ws://")) ? this.ip = ip : this.ip = "ws://" + ip;
            this.ip += ":" + port;

            this.password = password;

            ws = new WebSocket(this.ip);

            ws.OnMessage += messageReceived;
        }

        public void connect()
        {
            onInformation?.Invoke(this, "Attempting connection to OBS...");
            new Thread(() =>
            {
                try
                {
                    ws.Connect();
                    ws.Send(generateJsonRequest("GetAuthRequired", "authcheck"));
                } 
                catch (Exception e)
                {
                    Console.Out.WriteLine("I should detect wrong IP input, but Im not... (╯°□°）╯︵ ┻━┻");
                }
            }).Start();
        }

        public void disconnect()
        {
            ws.Close();
        }

        public bool isConnected()
        {
            return ws.IsAlive;
        }

        ////////////////////////////////////////////////REQUESTS////////////////////////////////////////////////
        public void getSceneList()
        {
            ws.Send(generateJsonRequest("GetSceneList","getscenelist"));
        }

        public void getStats()
        {
            ws.Send(generateJsonRequest("GetStats", "getstats"));
        }

        public void getCurrentScene()
        {
            ws.Send(generateJsonRequest("GetCurrentScene", "activesceneupdate"));
        }

        public void setCurrentScene(string scenename)
        {
            string[] headers = { "scene-name" };
            string[] data = { scenename };

            ws.Send(generateJsonRequest("SetCurrentScene", "setactivescene", headers, data));
        }

        public void setMute(string source, bool mute)
        {
            string[] headers = { "source", "mute" };
            object[] data = {source, mute };

            ws.Send(generateJsonRequest("SetMute", "setmute", headers, data));
        }





        private void authenticate(Dictionary<string,object> data)
        {
            if (((bool)data["authRequired"]))
            {
                onInformation?.Invoke(this, "Authentication required");

                string challenge = (string)data["challenge"];
                string salt = (string)data["salt"];

                string secret_string;
                byte[] secret_hash;
                string secret;

                string auth_resp_string;
                byte[] auth_resp_hash;
                string auth_resp;

                using (SHA256 sha256hash = SHA256.Create())
                {
                    secret_string = this.password + salt;
                    secret_hash = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(secret_string));
                    secret = System.Convert.ToBase64String(secret_hash);

                    auth_resp_string = secret + challenge;
                    auth_resp_hash = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(auth_resp_string));
                    auth_resp = System.Convert.ToBase64String(auth_resp_hash);
                }

                string[] headers = new string[] { "auth" };
                string[] rdata = new string[] { auth_resp };

                ws.Send(generateJsonRequest("Authenticate", "auth", headers, rdata));
            }
            else
            {
                onInformation?.Invoke(this, "No authentication required");
                onConnected?.Invoke(this, "Connected");
            }
        }

        private string generateJsonRequest(string header, string id, string[]headers = null, object[] data = null)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter  sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("request-type");
                writer.WriteValue(header);
                writer.WritePropertyName("message-id");
                writer.WriteValue(id);

                if(headers != null)
                {
                    for(int i = 0; i < headers.Length; i++)
                    {
                        writer.WritePropertyName(headers[i]);
                        
                        if (data[i].GetType() == typeof(bool))
                        {
                            writer.WriteValue((bool)data[i]);
                        }
                            

                        if (data[i].GetType() == typeof(string))
                            writer.WriteValue((string)data[i]);

                    }
                }

                writer.WriteEndObject();
            }

                return sb.ToString();
        }

        private void messageReceived(object sender,  MessageEventArgs e)
        {
            Dictionary<string, object> response =  JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);

            //If message received is a response
            if (response.ContainsKey("message-id"))
            {
                switch (response["message-id"])
                {
                    case "auth":
                        {
                            if (response["status"].Equals("ok"))
                            {
                                onAuthSuccess?.Invoke(this, "Authenticated");
                                onConnected?.Invoke(this, "Connected");
                            }
                            else
                            {
                                ws.Close();
                                onAuthFailed?.Invoke(this, "Authentication failed");
                                onDisconnected?.Invoke(this, "Disconnected");
                            }
                            break;
                        }

                    case "authcheck":
                        {
                            authenticate(response);
                            break;
                        }

                    case "activesceneupdate":
                        {
                            updateActiveScene((string)response["name"]);
                            break;
                        }

                    case "getscenelist":
                        {
                            updateSceneList((string)response["current-scene"], response["scenes"]);
                            break;
                        }

                    case "getstats":
                        {
                            updateStats((JObject)response["stats"]);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }


            //If message received is a event
            else if(response.ContainsKey("update-type"))
            {
                switch(response["update-type"])
                {
                    case "SwitchScenes":
                        {
                            onActiveSceneChange?.Invoke(this, (string)response["scene-name"]);
                            break;
                        }

                    default:
                        {
                            Console.Out.WriteLine("Unknown update: " + response["update-type"]);
                            break;
                        }
                }
            }
            
        }

        private void updateActiveScene(string name)
        {
            foreach (Scene scene in this.scenes)
            {
                if (scene.name.Equals(name)) this.activeScene = scene.name;
            }

            onActiveSceneChange?.Invoke(this, this.activeScene);
        }

        private void updateSceneList(string active, object scenes)
        {
            this.scenes = ((JArray)scenes).ToObject<List<Scene>>();

            foreach(Scene scene in this.scenes)
            {
                if (scene.name.Equals(active)) activeScene = scene.name;
            }

            onActiveSceneChange?.Invoke(this, this.activeScene);
            onScenelistUpdate?.Invoke(this, this.scenes);
        }

        private void updateStats(JObject stats)
        {
            this.stats = JsonConvert.DeserializeObject<ObsStats>(stats.ToString());

            onStatsUpdate?.Invoke(this, this.stats);
        }

        public string activeScene = null;

        public event EventHandler<string> onAuthFailed = null;
        public event EventHandler<string> onAuthSuccess = null;
        public event EventHandler<string> onConnected = null;
        public event EventHandler<string> onDisconnected = null;
        public event EventHandler<string> onInformation = null;

        public event EventHandler<string> onActiveSceneChange = null;
        public event EventHandler<List<Scene>> onScenelistUpdate = null;
        public event EventHandler<ObsStats> onStatsUpdate = null;
    }
}
