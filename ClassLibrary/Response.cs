using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }


        public object Results { get; set; }


        public string Token { get; set; }  
    }
}

