using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

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





            // ---------------- USER "FREDERICO" -----------------------------------------------------------------------------------------------------------------------------
            var user2 = await _userHelper.GetUserByEmailAsync("fredericoaugusto@gmail.com");
            if (user2 == null)
            {
                var company = await _contextUsers.Companies.FirstOrDefaultAsync(c => c.Name == "Frederico Augusto Lda");
                if (company == null)
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

                user2 = new User
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

                var result = await _userHelper.AddUserAsync(user2, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.CheckRoleAsync("CondoManager");
                await _userHelper.AddUserToRoleAsync(user2, "CondoManager");
            }

            // ---------------- CONDOMÍNIO LARANJE ----------------
            var condo = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "Condomínio Laranje");

            if (condo == null)
            {
                condo = new Condominium
                {
                    CondoName = "Condomínio Laranje",
                    Address = "Avenida José Mourinho, 1",
                    CompanyId = user2.CompanyId
                };

                await _contextCondos.Condominiums.AddAsync(condo);
                await _contextCondos.SaveChangesAsync();
            }

            // ---------------- CONDOMÍNIO PAZUVILLA ----------------
            var pazuVilla = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "PazuVilla");

            if (pazuVilla == null)
            {
                pazuVilla = new Condominium
                {
                    CondoName = "PazuVilla",
                    Address = "Pazulandia, 5",
                    CompanyId = user2.CompanyId
                };
                await _contextCondos.Condominiums.AddAsync(pazuVilla);
                await _contextCondos.SaveChangesAsync();
            }

            // ---------------- CONDOMEMBER DE TESTE ----------------


            var unit = await _contextCondos.Units.FirstOrDefaultAsync();
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
                        Units = new List<Unit> { unit }
                    };

                    await _contextCondos.CondoMembers.AddAsync(condoMember);
                    await _contextCondos.SaveChangesAsync();
                }
            }

            // ---------------- USER "NIKITINHA" ----------------
            var userNikitinha = await _userHelper.GetUserByEmailAsync("nikitinha@yopmail.com");
            if (userNikitinha == null)
            {
                var company = await _contextUsers.Companies.FirstOrDefaultAsync(c => c.Name == "Frederico Augusto Lda");
                userNikitinha = new User
                {
                    FullName = "Nikitinha",
                    Email = "nikitinha@yopmail.com",
                    UserName = "nikitinha@yopmail.com",
                    PhoneNumber = "12345678",
                    Address = "Rua Laranja",
                    BirthDate = new DateTime(1997, 05, 10),
                    IsActive = true,
                    EmailConfirmed = true,
                    Company = company,
                };

                var result2 = await _userHelper.AddUserAsync(userNikitinha, "123456");
                if (result2 != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.CheckRoleAsync("CondoManager");
                await _userHelper.AddUserToRoleAsync(userNikitinha, "CondoManager");
            }

            // ---------------- CONDOMÍNIO NIKITINHAVILLA ----------------
            var nikitinhaVilla = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "NikitinhaVilla");

            if (nikitinhaVilla == null)
            {
                nikitinhaVilla = new Condominium
                {
                    CondoName = "NikitinhaVilla",
                    Address = "Nikilandia, 5",
                    ManagerUserId = userNikitinha.Id,
                    CompanyId = userNikitinha.CompanyId
                };
                await _contextCondos.Condominiums.AddAsync(nikitinhaVilla);
                await _contextCondos.SaveChangesAsync();
            }

            // ---------------- OUTROS CONDOMÍNIOS ----------------
            var condominium3 = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "FeliniVilla");

            if (condominium3 == null)
            {

                condominium3 = new Condominium
                {
                    CondoName = "FeliniVilla",
                    Address = "Felinilandia, 5",
                    CompanyId = userNikitinha.CompanyId
                };
                var create3 = await _contextCondos.Condominiums.AddAsync(condominium3);
                await _contextCondos.SaveChangesAsync();


                var condominium4 = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "BobotiVilla");

                if (condominium4 == null)
                {

                    condominium4 = new Condominium
                    {
                        CondoName = "BobotiVilla",
                        Address = "Bobotilandia, 5",
                        CompanyId = userNikitinha.CompanyId
                    };
                    var create4 = await _contextCondos.Condominiums.AddAsync(condominium4);
                    await _contextCondos.SaveChangesAsync();

                }

                var condominium5 = await _contextCondos.Condominiums
                .FirstOrDefaultAsync(c => c.CondoName == "GudetamaVilla");

                if (condominium5 == null)
                {

                    condominium5 = new Condominium
                    {
                        CondoName = "GudetamaVilla",
                        Address = "Gudetamalandia, 5",
                        CompanyId = userNikitinha.CompanyId
                    };
                    var create5 = await _contextCondos.Condominiums.AddAsync(condominium5);
                    await _contextCondos.SaveChangesAsync();

                }

                // _______________________TESTE FINANCES___________________________
                //_____________________USER PARA CONDOMEMBER_______________________
                await _userHelper.CheckRoleAsync("CondoMember");

                var userCondoMember = await _userHelper.GetUserByEmailAsync("condomember0@yopmail.com");
                if (userCondoMember == null)
                {
                    userCondoMember = new User
                    {
                        FullName = "userCondoMember",
                        Email = "condomember0@yopmail.com",
                        UserName = "condomember0@yopmail.com",
                        PhoneNumber = "12345678",
                        Address = "Condominio Laranje",
                        BirthDate = new DateTime(1990, 04, 06),
                        IsActive = true,
                        EmailConfirmed = true
                    };


                    var result = await _userHelper.AddUserAsync(userCondoMember, "123456");
                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException("Could not create the user in seeder");
                    }

                    await _userHelper.AddUserToRoleAsync(userCondoMember, "CondoMember");


                    var financialAccount = new FinancialAccount()
                    {
                        Deposit = 200, // depósito inicial vai ser sempre 0
                        Balance = 200
                    };

                    await _contextFinances.FinancialAccounts.AddAsync(financialAccount); //add FinAcc na Bd
                    await _contextFinances.SaveChangesAsync();

                    userCondoMember.FinancialAccountId = financialAccount.Id;

                    await _userHelper.UpdateUserAsync(userCondoMember);
                }
                else
                {
                    var isInRole = await _userHelper.IsUserInRoleAsync(userCondoMember, "CondoMember");
                    if (!isInRole)
                    {
                        await _userHelper.AddUserToRoleAsync(userCondoMember, "CondoMember");
                    }
                }

                //________________CRIAR UNIT DO CONDOlARANJE___________________
                var firstUnit = await _contextCondos.Units.FirstOrDefaultAsync(u => u.Id == 1);
                if (firstUnit == null)
                {
                    firstUnit = new Unit()
                    {
                        CondominiumId = 1,
                        Floor = "1",
                        Door = "1",
                        Bedrooms = 2

                    };
                }
                await _contextCondos.Units.AddAsync(firstUnit);
                await _contextCondos.SaveChangesAsync();


                //criar condoMember 
                var condoMember = await _contextCondos.CondoMembers.FirstOrDefaultAsync(c => c.Id == 1);
                if (condoMember == null)
                {
                    //lista de units do condomember
                    var unitsList = new List<Unit>();
                    unitsList.Add(firstUnit);

                    condoMember = new CondoMember()
                    {
                        FullName = "CondoMember",
                        BirthDate = DateTime.Now,
                        PhoneNumber = "87654321",
                        Email = "condomember0@yopmail.com",
                        IdDocument = "987546898",
                        TaxIdNumber = "9469054784",
                        Units = unitsList
                    };
                }

                await _contextCondos.CondoMembers.AddAsync(condoMember);
                await _contextCondos.SaveChangesAsync();

                //____________________PAYMENT CONDO LARANJE______________________________

                //criar expense
                var expense = await _contextFinances.Expenses.FirstOrDefaultAsync(e=> e.Id == 1);
                if(expense == null)
                {
                    expense = new Expense()
                    {
                        Detail = "Monthly quota",
                        Amount = 100,
                        ExpenseType = Entites.Enums.ExpenseType.Quota,
                        CondominiumId = 1,
                    };

                    await _contextFinances.Expenses.AddAsync(expense);
                    await _contextFinances.SaveChangesAsync();

                    var expenses = new List<Expense>();
                    expenses.Add(expense);


                    //_______________________CRIAR PAYMENT____________________
                    var payment1 = await _contextFinances.Payments.FirstOrDefaultAsync(p => p.Id == 1);

                    if (payment1 == null)
                    {
                        payment1 = new Payment()
                        {
                            IssueDate = DateTime.Now,
                            DueDate = DateTime.Now,
                            PayerFinancialAccountId = userCondoMember.FinancialAccountId.Value,
                            CondominiumId = 1,
                            Expenses = expenses,
                        };

                        await _contextFinances.Payments.AddAsync(payment1);
                        await _contextFinances.SaveChangesAsync();
                    }
                }

               



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
}
