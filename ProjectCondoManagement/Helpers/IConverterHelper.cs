using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        public CondoMember ToCondoMember(CondoMemberDto condoMemberDto);

        public CondoMemberDto ToCondoMemberDto(User user);

        public UserDto ToUserDto(User user);

        public CondoMemberDto ToCondoMemberDto(CondoMember condoMember);

        public Task<User> ToEditedUser(EditUserDetailsDto editUserDetailsDto);

        public Task<User> ToEditedProfile(UserDto userDto);

        public Task<CondoMember> FromUserToCondoMember(User user);

        public CompanyDto ToCompanyDto(Company company);

        public User ToUser(UserDto userDto);

        public Company ToCompany(CompanyDto companyDto, bool isNew);

        public CondominiumDto ToCondominiumDto(Condominium condominium);    

        public Condominium ToCondominium(CondominiumDto condominiumDto, bool isNew);

        public OccurrenceDto ToOccurrenceDto(Occurrence occurrence);

        public Occurrence ToOccurrence(OccurrenceDto occurrenceDto, bool isNew);

        public Unit ToUnit(UnitDto unitDto, bool isNew);

        public UnitDto ToUnitDto(Unit unit);

        public Document ToDocument(DocumentDto documentDto, bool isNew);

        public DocumentDto ToDocumentDto(Document document);

    }
}
