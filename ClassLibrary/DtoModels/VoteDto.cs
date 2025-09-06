using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class VoteDto
    {
        public int Id { get; set; }

        public bool YesNoVote { get; set; }

        public int CondoMemberId { get; set; }

        public CondoMemberDto CondoMember { get; set; }
    }
}
