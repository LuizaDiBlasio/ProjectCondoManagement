namespace ClassLibrary.DtoModels
{
    public class CompanyDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserDto>? Users { get; set; }

        public string? CompanyAdminId { get; set; }

        public IEnumerable<CondominiumDto>? CondominiumDtos { get; set; }

        public IEnumerable<CondoMemberDto>? CondoMemberDtos { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string TaxIdDocument { get; set; }

        public int FinancialAccountId { get; set; }
    }
}
