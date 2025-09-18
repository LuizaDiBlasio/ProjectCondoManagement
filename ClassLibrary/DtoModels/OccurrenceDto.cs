using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class OccurrenceDto
    {
        public int Id { get; set; }

        public string Details { get; set; }

        public string Subject { get; set; }


        [Display(Name ="Date")]
        public DateTime DateAndTime { get; set; }


        [Display(Name = "Resolution Date")]
        public DateTime? ResolutionDate { get; set; }


        [Display(Name = "Units involved")]
        public List<UnitDto>? UnitDtos { get; set; } = new List<UnitDto>();


        [Display(Name = "Status")]
        public bool IsResolved { get; set; } = false;


        public int CondominiumId { get; set; }

        public MeetingDto? Meeting { get; set; }

        public int? MeetingId { get; set; }

        public override string ToString()
        {
            return Subject;
        }
    }
}