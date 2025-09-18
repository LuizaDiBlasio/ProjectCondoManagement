using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.DtoModels
{
    public class CondominiumDto
    {
        public int Id { get; set; }

        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        public CompanyDto? Company { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }


        [Display(Name = "Condominium")]
        [Required(ErrorMessage = "Condo name is required.")]
        public string CondoName { get; set; }


        [Display(Name = "Condo Manager")]
        public string? ManagerUserId { get; set; }

        [NotMapped]
        public FinancialAccountDto? FinancialAccountDto { get; set; }

        [NotMapped]
        public List<PaymentDto>? Payments { get; set; }

        public int? FinancialAccountId { get; set; }     

        public UserDto? ManagerUser { get; set; }

        public IEnumerable<CondoMemberDto>? CondoMembers { get; set; }

        public IEnumerable<UnitDto>? Units { get; set; }

        public IEnumerable<DocumentDto>? Documents { get; set; }

        public IEnumerable<MeetingDto>? Meetings { get; set; }

        public List<OccurrenceDto>? Occurrences { get; set; }

    }
}