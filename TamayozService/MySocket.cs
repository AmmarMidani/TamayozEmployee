using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamayozService
{
   public struct MySocket
    {
        public static string socket_url = "https://employees.t-tamayoz.com:3000";
        public static string OnConnect = "";
        public static string OnDisconnect = "";
        public static string OnFirstConnect = "newSocketCreated";
        public static string OnNotificationReceived = "receiveNotif";
        public static string OnNotificationRead = "makeNotifAsRead";
        public static string OnReceiveConstNotification = "receiveConstNotif";
        public static string OnDeleteConstNotification = "deleteConstNotif";
    }
}
