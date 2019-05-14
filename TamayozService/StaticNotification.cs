using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamayozService
{
    public class StaticNotification
    {
        int _id;
        string _title;
        string _content;
        string _icon_url;
        string _time;
        string _local_icon_url;
        JArray _days;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        public string Icon_url
        {
            get
            {
                return _icon_url;
            }

            set
            {
                _icon_url = value;
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
            }
        }

        public string Local_icon_url
        {
            get
            {
                return _local_icon_url;
            }

            set
            {
                _local_icon_url = value;
            }
        }

        public JArray Days
        {
            get
            {
                return _days;
            }

            set
            {
                _days = value;
            }
        }
    }
}
