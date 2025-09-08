using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CondominiumWithMeetingsDto
    {
        public int CondominiumId { get; set; }

        public string CondoName { get; set; }

        public List<MeetingDto> MeetingsDto { get; set; }
    }
}
