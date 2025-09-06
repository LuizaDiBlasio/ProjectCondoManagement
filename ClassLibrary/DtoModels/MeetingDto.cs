using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateAndTime { get; set; }

        public DocumentDto Report { get; set; }

        public List<CondoMemberDto> CondoMembersDto { get; set; }

        public VotingDto? Voting { get; set; }

        public int? VotingId { get; set; }

        public List<OccurrenceDto> OccurencesDto { get; set; }

        public bool MeetingType { get; set; }  // (regular/extra)

        public string MeetingLink { get; set; }
    }
}