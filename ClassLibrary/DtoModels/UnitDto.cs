using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class UnitDto
    {
        public int Id { get; set; }

        [Display(Name ="Condominium")]
        public CondominiumDto? CondominiumDto { get; set; }

        [Display(Name = "Condominium")]
        public int CondominiumId { get; set; }

        public string Floor { get; set; }

        public int Bedrooms { get; set; }

        public IEnumerable<OccurrenceDto>? OccurrenceDtos { get; set; }

        public IEnumerable<CondoMemberDto>? CondoMemberDtos { get; set; }

        public string Door { get; set; }

    }
}