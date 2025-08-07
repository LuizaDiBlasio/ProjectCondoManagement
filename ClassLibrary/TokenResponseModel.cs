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

        public string JwtToken { get; set; }

        public DateTime? JwtExpiration { get; set; } 
    }
}
