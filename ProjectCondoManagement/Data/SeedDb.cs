using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProjectCondoManagement.Data
{
    public class SeedDb
    {
        private readonly DataContextCondos _contextCondos;

        private readonly DataContextUsers _contextUsers;

        private readonly DataContextFinances _contextFinances;

        private readonly IUserHelper _userHelper;

        private readonly ICondominiumRepository _condominiumRepository;



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



            //__________________________________________________________________________________________________________________

            //TODO: TESTE DE CONDOMINIO APAGAR.
            var user2 = await _userHelper.GetUserByEmailAsync("fredericoaugusto@gmail.com"); //ver se user já existe 

            if (user2 == null) // caso não encontre o utilizador 
            {
                var company = await _contextUsers.Companies.FirstOrDefaultAsync(c => c.Name == "Frederico Augusto Lda");
                if (company == null) // se não existir a empresa, cria uma nova
                {
                    company = new Company
                    {
                        Name = "Frederico Augusto Lda",
                        Address = "Rua Laranja, 123",
                        PhoneNumber = "123456789",
                        Email = "fredericoaugusto@gmail.com",
                        TaxIdDocument = "123456789",
                    };
                }
                user2 = new User // cria utilizador admin
                {
                    FullName = "Frederico Augusto",
                    Email = "fredericoaugusto@gmail.com",
                    UserName = "fredericoaugusto@gmail.com",
                    PhoneNumber = "12345678",
                    Address = "Rua Laranja",
                    BirthDate = new DateTime(1997, 05, 10),
                    Company = company,                    

                    IsActive = true,
                    EmailConfirmed = true
                };

                var activation2FA = await _userHelper.EnableTwoFactorAuthenticationAsync(user2, true);


                var result = await _userHelper.AddUserAsync(user2, "123456"); //criar utilizador, mandar utilizador e password

                if (result != IdentityResult.Success) //se o resultado não for bem sucedido (usa propriedade da classe Identity) 
                {
                    throw new InvalidOperationException("Coud not create the user in seeder"); //pára o programa
                }



                await _userHelper.CheckRoleAsync("CondoManager");
                await _userHelper.AddUserToRoleAsync(user2, "CondoManager"); //adiciona role ao user
            }

            //TODO CRIAÇÃO DE CONDOMINIO  Teste APAGAR
            var condo = await _contextCondos.Condominiums.FirstOrDefaultAsync(c => c.CondoName == "Condomínio Laranje" && c.CompanyId == 2);

            var condominium = await _condominiumRepository.GetByIdAsync(2, _contextCondos);
            if (condo == null)
            {
                condo = new Condominium
                {
                    CondoName = "Condomínio Laranje",
                    Address = "Avenida José Mourinho, 1",
                    CompanyId = 2
                };

                await _contextCondos.Condominiums.AddAsync(condo);
            }

            var unit = await _contextCondos.Units.FirstOrDefaultAsync(u => u.Id == 1);

            if (condominium == null)
            {
                var condominium1 = new Condominium
                {
                    CondoName = "PazuVilla",
                    Company = null,
                    Address = "Pazulandia, 5",
                    ManagerUserId = null,
                    ManagerUser = null,
                    CondoMembers = null,
                    Units = null,
                    Documents = null,
                    Meetings = null,
                    Occurrences = null,
                    CompanyId = null
                };
                var create = await _contextCondos.Condominiums.AddAsync(condominium1);
                await _contextCondos.SaveChangesAsync();
            }
            if (unit != null)
            {
                var condoMember = await _contextCondos.CondoMembers
                    .FirstOrDefaultAsync(cm => cm.Email == "condomember@yopmail.com");

                if (condoMember == null)
                {
                    condoMember = new CondoMember
                    {
                        FullName = "Carlos Mendes",
                        BirthDate = new DateTime(1985, 02, 15),
                        PhoneNumber = "987654321",
                        Address = "Rua Amarela, 456",
                        Email = "condomember@yopmail.com",
                        IdDocument = "AB123456",
                        TaxIdNumber = "987654321",
                        MeetingsAttended = null,
                        ImageUrl = null,
                        Units = new List<Unit> { unit }  // aqui fazemos a relação N:N
                    };

                    await _contextCondos.CondoMembers.AddAsync(condoMember);
                    await _contextCondos.SaveChangesAsync();
                }
            }





            // ***************************************************************************************************************************************************************************

            //Criar CondoManager para NikitinhaVila 
            var userNikitinha = await _userHelper.GetUserByEmailAsync("nikitinha@yopmail.com");

            if (userNikitinha == null)
            {
                userNikitinha = new User // cria utilizador admin
                {
                    FullName = "Nikitinha",
                    Email = "nikitinha@yopmail.com",
                    UserName = "nikitinha@yopmail.com",
                    PhoneNumber = "12345678",
                    Address = "Rua Laranja",
                    BirthDate = new DateTime(1997, 05, 10),


                    IsActive = true,
                    EmailConfirmed = true
                };

                var activate2FA = await _userHelper.EnableTwoFactorAuthenticationAsync(userNikitinha, true);

                var result2 = await _userHelper.AddUserAsync(userNikitinha, "123456"); //criar utilizador, mandar utilizador e password

                if (result2 != IdentityResult.Success) //se o resultado não for bem sucedido (usa propriedade da classe Identity) 
                {
                    throw new InvalidOperationException("Coud not create the user in seeder"); //pára o programa
                }
                await _userHelper.CheckRoleAsync("CondoManager");
                await _userHelper.AddUserToRoleAsync(userNikitinha, "CondoManager"); //adiciona role ao user
            }


            var condominium2 = await _condominiumRepository.GetByIdAsync(2025, _contextCondos);

            if (condominium2 == null)
            {

                condominium2 = new Condominium
                {
                    CondoName = "NikitinhaVilla",
                    Company = null,
                    Address = "Nikilandia, 5",
                    CondoMembers = null,
                    ManagerUser = userNikitinha,
                    ManagerUserId = userNikitinha.Id,
                    Units = null,
                    Documents = null,
                    Meetings = null,
                    Occurrences = null,
                    CompanyId = null,
                };
                var create2 = await _contextCondos.Condominiums.AddAsync(condominium2);
                await _contextCondos.SaveChangesAsync();

            }

            var condominium3 = await _condominiumRepository.GetByIdAsync(2026, _contextCondos);

            if (condominium3 == null)
            {

                condominium3 = new Condominium
                {
                    CondoName = "FeliniVilla",
                    Company = null,
                    Address = "Felinilandia, 5",
                    CondoMembers = null,
                    ManagerUser = null,
                    ManagerUserId = null,
                    Units = null,
                    Documents = null,
                    Meetings = null,
                    Occurrences = null,
                    CompanyId = null,
                };
                var create3 = await _contextCondos.Condominiums.AddAsync(condominium3);
                await _contextCondos.SaveChangesAsync();


                var condominium4 = await _condominiumRepository.GetByIdAsync(2027, _contextCondos);

                if (condominium4 == null)
                {

                    condominium4 = new Condominium
                    {
                        CondoName = "BobotiVilla",
                        Company = null,
                        Address = "Bobotilandia, 5",
                        CondoMembers = null,
                        ManagerUser = null,
                        ManagerUserId = null,
                        Units = null,
                        Documents = null,
                        Meetings = null,
                        Occurrences = null,
                        CompanyId = null
                    };
                    var create4 = await _contextCondos.Condominiums.AddAsync(condominium4);
                    await _contextCondos.SaveChangesAsync();

                }

                var condominium5 = await _condominiumRepository.GetByIdAsync(2028, _contextCondos);

                if (condominium5 == null)
                {

                    condominium5 = new Condominium
                    {
                        CondoName = "GudetamaVilla",
                        Company = null,
                        Address = "Gudetamalandia, 5",
                        CondoMembers = null,
                        ManagerUser = null,
                        ManagerUserId = null,
                        Units = null,
                        Documents = null,
                        Meetings = null,
                        Occurrences = null,
                        CompanyId = null
                    };
                    var create5 = await _contextCondos.Condominiums.AddAsync(condominium4);
                    await _contextCondos.SaveChangesAsync();

                }

                //______________________________________________________________________________________________________________________________



                await _userHelper.CheckRoleAsync("Admin"); //verificar se já existe um role de admin, se não existir cria

                var user = await _userHelper.GetUserByEmailAsync("luizabandeira90@gmail.com"); //ver se user já existe 

                if (user == null) // caso não encontre o utilizador 
                {
                    user = new User // cria utilizador admin
                    {
                        FullName = "Luiza Bandeira",
                        Email = "luizabandeira90@gmail.com",
                        UserName = "luizabandeira90@gmail.com",
                        PhoneNumber = "12345678",
                        Address = "Fonte da Saudade",
                        BirthDate = new DateTime(1990, 04, 06),
                        IsActive = true,
                        EmailConfirmed = true,
                        FinancialAccountId = null
                    };

                    var result = await _userHelper.AddUserAsync(user, "123456"); //criar utilizador, mandar utilizador e password

                    if (result != IdentityResult.Success) //se o resultado não for bem sucedido (usa propriedade da classe Identity) 
                    {
                        throw new InvalidOperationException("Coud not create the user in seeder"); //pára o programa
                    }

                    var activation2FA = await _userHelper.EnableTwoFactorAuthenticationAsync(user, true); // habilitar 2fa

                    await _userHelper.AddUserToRoleAsync(user, "SysAdmin"); //adiciona role ao user
                }

                var isInRole = await _userHelper.IsUserInRoleAsync(user, "SysAdmin"); //verifica se role foi designado para user existente

                if (!isInRole) //se não estiver no role, colocar
                {
                    await _userHelper.AddUserToRoleAsync(user, "SysAdmin"); //adiciona role ao user
                }
            }
        }
    }
}
