using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class OccurrenceDto
    {
        public int Id { get; set; }

        public string Details { get; set; }

        public DateTime DateAndTime { get; set; }

        public DateTime ResolutionDate { get; set; }

        public IEnumerable<UnitDto> UnitDtos { get; set; }

        public int? Meeting { get; set; }
    }
}
