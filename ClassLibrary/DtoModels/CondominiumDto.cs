using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassLibrary.DtoModels
{
    public class CondominiumDto
    {
        public int Id { get; set; }

        public string Name { get; set; }    

        public int CompanyId { get; set; }

        public string Address { get; set; }

        public string ManagerUserId { get; set; }

        public UserDto Manager {  get; set; }   

        public IEnumerable<UnitDto> UnitDtos { get; set; }

        public IEnumerable<DocumentDto> DocumentDtos { get; set; }

        public IEnumerable<MeetingDto> MeetingDtos { get; set; }

        public IEnumerable<OccurrenceDto> OccurrenceDtos { get; set; }
    }

  
}
