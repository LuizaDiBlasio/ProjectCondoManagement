using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Controllers;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class CondoMemberRepository : GenericRepository<CondoMember, DataContextCondos>, ICondoMemberRepository
    {
        private readonly IUserHelper _userHelper;

        public CondoMemberRepository(IUserHelper userHelper)
        {            
            _userHelper = userHelper;
        }


        public async Task<Response> LinkImages(IEnumerable<CondoMember> condoMembers)
        {
            try
            {
                var emails = condoMembers
                     .Select(c => c.Email)
                     .Where(e => !string.IsNullOrWhiteSpace(e))
                     .ToList();

                var users = await _userHelper.GetUsersByEmailsAsync(emails);

                condoMembers = condoMembers
                    .Select(c =>
                    {
                        var user = users.FirstOrDefault(u => u.Email == c.Email);
                        if (user != null)
                        {
                            c.ImageUrl = user.ImageUrl;
                        }
                        return c;
                    })
                    .ToList();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Images linked successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = $"Error linking images: {ex.Message}"
                };

            }


        }
    }
}
