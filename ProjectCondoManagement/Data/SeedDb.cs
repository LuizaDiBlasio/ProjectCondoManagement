using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System;

namespace ProjectCondoManagement.Data
{
    public class SeedDb
    {
        private readonly DataContextCondos _contextCondos;
        private readonly DataContextUsers _contextUsers;
        private readonly DataContextFinances _contextFinances;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository;

        public SeedDb(
            DataContextCondos contextCondos,
            DataContextFinances dataContextFinances,
            DataContextUsers contextUsers,
            IUserHelper userHelper,
            ICondominiumRepository condominiumRepository)
        {
            _contextCondos = contextCondos;
            _contextFinances = dataContextFinances;
            _contextUsers = contextUsers;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Apply EF Core migrations
                await _contextCondos.Database.MigrateAsync();
                await _contextUsers.Database.MigrateAsync();
                await _contextFinances.Database.MigrateAsync();

                // Create roles
                await _userHelper.CreateRolesAsync();

                // Ensure SysAdmin role exists
                await _userHelper.CheckRoleAsync("SysAdmin");

                // Check if the admin user exists
                var userLuiza = await _userHelper.GetUserByEmailAsync("luizabandeira90@gmail.com");
                if (userLuiza == null)
                {
                    userLuiza = new User
                    {
                        FullName = "Luiza Bandeira",
                        Email = "luizabandeira90@gmail.com",
                        UserName = "luizabandeira90@gmail.com",
                        PhoneNumber = "12345678",
                        Address = "Fonte da Saudade",
                        BirthDate = new DateTime(1990, 04, 06),
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    // Enable 2FA
                    var activation2FA = await _userHelper.EnableTwoFactorAuthenticationAsync(userLuiza, true);

                    // Add user
                    var result = await _userHelper.AddUserAsync(userLuiza, "123456");
                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException("Could not create the user in seeder.");
                    }

                    // Assign role
                    await _userHelper.AddUserToRoleAsync(userLuiza, "SysAdmin");
                }
                else
                {
                    // Ensure user is in SysAdmin role
                    var isInRole = await _userHelper.IsUserInRoleAsync(userLuiza, "SysAdmin");
                    if (!isInRole)
                    {
                        await _userHelper.AddUserToRoleAsync(userLuiza, "SysAdmin");
                    }
                }

                Console.WriteLine("Seeding completed successfully!");
            }
            catch (Exception ex)
            {
                // Log full exception for debugging
                Console.WriteLine("SeedDb exception: " + ex);
                // Optional: rethrow if you want the app to still fail
                throw;
            }
        }
    }
}