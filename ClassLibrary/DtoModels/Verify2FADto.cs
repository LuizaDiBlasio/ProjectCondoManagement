using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class Verify2FADto
    {
        public string Username { get; set; }

        public string Token { get; set; }
    }
}
