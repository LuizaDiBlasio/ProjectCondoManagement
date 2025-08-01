using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class CondoMemberRepository : GenericRepository<CondoMember, DataContextCondos>, ICondoMemberRepository
    {

    }
}
