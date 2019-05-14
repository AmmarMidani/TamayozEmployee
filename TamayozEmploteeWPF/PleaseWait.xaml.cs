using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Threading;

namespace TamayozEmploteeWPF
{
    /// <summary>
    /// Interaction logic for PleaseWait.xaml
    /// </summary>
    public partial class PleaseWait : Window
    {
        DispatcherTimer dispatcherTimer;
        List<TamayozService.StaticNotification> all_times;

        public PleaseWait()
        {
            InitializeComponent();
            try
            {
                all_times = TamayozService.MySetting.RunTimer();
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("PleaseWait: PleaseWait: " + ex.Message);
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                all_times = TamayozService.MySetting.RunTimer();
                //Debug.WriteLine("all_times:" + all_times.Count);
                foreach (var item in all_times)
                {
                    if (item.Time == DateTime.Now.ToString("HH:mm:00"))
                    {
                        JObject result_of_socket = new JObject();
                        result_of_socket["notif_id"] = item.Id;
                        result_of_socket["title"] = item.Title;
                        result_of_socket["content"] = item.Content;
                        result_of_socket["url"] = "https://employees.t-tamayoz.com/";
                        result_of_socket["icon_url"] = item.Icon_url;
                        RaisePopUp(null, new PopupEventArgs(result_of_socket));
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("PleaseWait: dispatcherTimer_Tick: " + ex.Message);
            }
        }

        private const String APP_ID = "Desktop Toasts Sample";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                listen();
                this.Hide();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("PleaseWait: Window_Loaded: " + ex.Message);
            }
        }

        private void listen()
        {
            var socket = IO.Socket(TamayozService.MySocket.socket_url);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                //Debug.WriteLine("Connected");
                JObject o = new JObject();
                o["user_id"] = TamayozService.MySetting.GetUserID();
                o["user_department_id"] = TamayozService.MySetting.GetDepartmentID();
                o["secureToken"] = "00601b0b6566266e1fc545d0bb3ef11e";
                o["is_device"] = 1;
                o["user_notifs_id"] = TamayozService.MySetting.GetConstIconsIDs();
                socket.Emit(TamayozService.MySocket.OnFirstConnect, o);
            });
            socket.On(TamayozService.MySocket.OnNotificationReceived, (data) =>
            {
                JObject result_of_socket = JObject.Parse(data.ToString());
                JObject o = new JObject();
                o["notif_id"] = result_of_socket["notif_id"].ToString();
                o["secureToken"] = "cce509dd22d98150ab9d4c746721d94d";
                socket.Emit(TamayozService.MySocket.OnNotificationRead, o);
                RaisePopUp(null, new PopupEventArgs(result_of_socket));
            });
            socket.On(TamayozService.MySocket.OnReceiveConstNotification, (data) =>
            {
                //Debug.WriteLine("Const Notification recieved");
                JObject result_of_socket = JObject.Parse(data.ToString());
                TamayozService.MySetting.AddNewStatic(
                    result_of_socket["id"].ToString(),
                    result_of_socket["title"].ToString(),
                    result_of_socket["content"].ToString(),
                    result_of_socket["icon_url"].ToString(),
                    result_of_socket["time"].ToString(),
                    JArray.Parse(result_of_socket["days"].ToString())
                    );
                all_times = TamayozService.MySetting.RunTimer();
            });
            socket.On(TamayozService.MySocket.OnDeleteConstNotification, (data) =>
            {
                //Debug.WriteLine("OnDeleteConstNotification");
                JArray result_of_socket = JArray.Parse(data.ToString());
                foreach (var item in result_of_socket)
                {
                    int IDToDelete = int.Parse(item.ToString());
                    var item2 = all_times.Where(m => m.Id == IDToDelete).FirstOrDefault();
                    if (item2 != null) all_times.Remove(item2);
                }
                TamayozService.MySetting.WriteConstAfterDelete(all_times);
                all_times = TamayozService.MySetting.RunTimer();
            });
            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                //Debug.WriteLine("Disconnected");
            });
        }

        public event EventHandler<PopupEventArgs> OnPopup;
        private void RaisePopUp(object sender, PopupEventArgs e)
        {
            try
            {
                OnPopup?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("PleaseWait: PleaseWait: " + ex.Message);
            }
        }

        public class PopupEventArgs : EventArgs
        {
            public PopupEventArgs(JObject Result)
            {
                this.Result = Result;
            }
            public JObject Result { get; private set; }
        }
    }
}