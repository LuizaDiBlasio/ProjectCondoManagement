using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class UnitDto
    {
        public int Id { get; set; }

        public CondominiumDto CondominiumDto { get; set; }

        public string Floor { get; set; }

        public IEnumerable<OccurrenceDto> OccurrenceDtos { get; set; }

        public IEnumerable<CondoMemberDto> CondoMemberDtos { get; set; }

        public string Door { get; set; }

    }
}
