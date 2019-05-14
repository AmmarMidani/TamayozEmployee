using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamayozService
{
   public class MyIcon
    {
        int _ID;
        string _URL;

        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }

        public string URL
        {
            get
            {
                return _URL;
            }

            set
            {
                _URL = value;
            }
        }
    }
}
