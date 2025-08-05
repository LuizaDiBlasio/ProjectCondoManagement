using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Helpers
{
    public interface ICondoMemberHelper
    {

        public Task<IEnumerable<CondoMemberDto>> GetCondoMembersAsync();

        public Task<CondoMemberDto> GetCondoMemberAsync(int id);

        public Task<bool> CreateCondoMemberAsync(CondoMemberDto condoMemberDto);

        public Task<bool> EditCondoMemberAsync(CondoMemberDto condoMemberDto);

        public Task<bool> DeleteCondoMemberAsync(int id);

    }
}
