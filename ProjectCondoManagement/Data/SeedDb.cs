using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data
{
    public class SeedDb
    {
        private readonly DataContextCondos _contextCondos;

        private readonly DataContextUsers _contextUsers;

        private readonly DataContextFinances _contextFinances;

        private readonly IUserHelper _userHelper;

        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public SeedDb(DataContextCondos contextCondos, DataContextFinances dataContextFinances, DataContextUsers contextUsers, IUserHelper userHelper, ICondominiumRepository condominiumRepository) //TODO apagar condo repository
        {
            _contextCondos = contextCondos;
            _contextFinances = dataContextFinances;
            _contextUsers = contextUsers;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
        }

        public async Task SeedAsync() //método para inicializar com admin
        {
            //await _contextCondos.Database.EnsureCreatedAsync();

            //await _contextUsers.Database.EnsureCreatedAsync();

            //await _contextFinances.Database.EnsureCreatedAsync();



            await _contextCondos.Database.MigrateAsync(); // Faz migrações pendentes. Caso não exista BD, cria uma BD aos moldes do _context 

            await _contextUsers.Database.MigrateAsync();

            await _contextFinances.Database.MigrateAsync();

            await _userHelper.CreateRolesAsync();




                //_______________________________________________________________________________________________________________________________________

                // ---------------- USER "LUIZA" SYSADMIN ----------------
                await _userHelper.CheckRoleAsync("SysAdmin");

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

                    var result = await _userHelper.AddUserAsync(userLuiza, "123456");
                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException("Could not create the user in seeder");
                    }

                    await _userHelper.AddUserToRoleAsync(userLuiza, "SysAdmin");
                }
                else
                {
                    var isInRole = await _userHelper.IsUserInRoleAsync(userLuiza, "SysAdmin");
                    if (!isInRole)
                    {
                        await _userHelper.AddUserToRoleAsync(userLuiza, "SysAdmin");
                    }
                }
            }
        }
    }
