using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class VotingDto
    {
        public int Id { get; set; }

        public int MeetingId { get; set; }

        public MeetingDto Meeting { get; set; }

        public List<VoteDto> Votes { get; set; }

        public bool Result { get; set; }

        public string Matter { get; set; }
    }
}
