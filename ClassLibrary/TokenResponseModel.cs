using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class TokenResponseModel
    {
        public string Token { get; set; }

        public DateTime? Expiration { get; set; } 
    }
}
