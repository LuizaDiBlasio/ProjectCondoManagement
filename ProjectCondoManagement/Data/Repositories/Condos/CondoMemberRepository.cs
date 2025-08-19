using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly DataContextCondos _context;


        public CondoMemberRepository(IUserHelper userHelper, DataContextCondos context)
        {
            _userHelper = userHelper;
            _context = context;
        }


        public async Task<Response> LinkImages(IEnumerable<CondoMember> condoMembers)
        {
            try
            {
                var emails = condoMembers
                    .Select(c => c.Email)
                    .Where(e => !string.IsNullOrEmpty(e))
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

        public async Task<CondoMember> GetCondoMemberByEmailAsync(string email)
        {
            var condoMember = await GetAll(_context).FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            
            if (condoMember == null)
            {
                return null;
            }

            return condoMember; 
        }
            

        
    }
}
