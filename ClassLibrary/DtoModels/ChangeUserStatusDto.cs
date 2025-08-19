using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class ChangeUserStatusDto
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
