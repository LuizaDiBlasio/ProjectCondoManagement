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


        public async Task<Response<object>> LinkImages(IEnumerable<CondoMember> condoMembers)
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

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Images linked successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error linking images: {ex.Message}"
                };
            }
        }

        public async Task<CondoMember> GetCondoMemberByEmailAsync(string email)
        {
            var condoMember = await GetAll(_context).Include(c => c.Units)
                                            .ThenInclude(u => u.Condominium)
                                            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            
            if (condoMember == null)
            {
                return null;
            }

            return condoMember; 
        }


        public async Task<CondoMember?> GetByIdWithIncludeAsync(int id, DataContextCondos context)
        {
            return await context.CondoMembers.Include(c => c.Units).ThenInclude(u => u.Condominium).FirstOrDefaultAsync(c => c.Id == id);

        }

        public async Task<bool> AssociateFinancialAccountAsync(string? email, int? financialAccountId)
        {
            var member = GetAll(_context).FirstOrDefault(c => c.Email == email);
            if (member == null)
            {
                return false;
            }
                

            member.FinancialAccountId = financialAccountId ?? 0;

            _context.CondoMembers.Update(member);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CondoMember>> GetCondoMembersByEmailsAsync(List<string> emails)
        {
            return await _context.CondoMembers
                .Where(cm => emails.Contains(cm.Email))
                .ToListAsync();
        }



        public async Task<bool> ExistByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return await _context.CondoMembers.AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }
    }
}
