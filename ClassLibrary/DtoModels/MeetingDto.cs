using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class MeetingDto
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }

        public CondominiumDto Condominium { get; set; }

        public DateTime DateAndTime { get; set; }

        public Document Report { get; set; }

        public IEnumerable<CondoMemberDto> CondoMembers { get; set; }

        public IEnumerable<VotingDto> Votings { get; set; }

        public IEnumerable<OccurrenceDto> Occurences { get; set; }

        public bool MeetingType { get; set; }  // (regular/extra)
    }
}
