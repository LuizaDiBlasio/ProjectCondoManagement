using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }   

        public string UserId { get; set; }  

        public string Password { get; set; }    
    }
}
