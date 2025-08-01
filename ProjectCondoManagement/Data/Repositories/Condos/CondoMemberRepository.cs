using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.CondosDb;
using ProjectCondoManagementAPI.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagementAPI.Data.Repositories.Condos
{
    public class CondoMemberRepository : GenericRepository<CondoMember, DataContextCondos>, ICondoMemberRepository
    {

    }
}
