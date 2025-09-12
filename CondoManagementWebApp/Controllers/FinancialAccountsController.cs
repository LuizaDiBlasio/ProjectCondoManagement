using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Helpers;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class FinancialAccountsController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;

        public FinancialAccountsController(IApiCallService apiCallService, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
        }


        // GET: FinancialAccountsController/Details/5
        public async Task<ActionResult> Details(int? id, string? email)
        {

            if (User.IsInRole("CompanyAdmin"))
            {
                var user = await _apiCallService.PostAsync<string, UserDto>("api/Account/GetUserByEmail", email);

                var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{user.CompanyId}");

                id = company.FinancialAccountId;

            }



            if (User.IsInRole("CondoMember"))
            {
                var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");

                id = user.FinancialAccountId;
            }

            if (id == null)
            {
                return NotFound();
            }


            if (email == null)
            {
                return NotFound();
            }

            ViewBag.Email = email;

                     

            try
            {
                var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{id.Value}");
                if (financialAccountDto == null)
                {
                    return NotFound();
                }

                return View(financialAccountDto);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching account");
            }

            return NotFound();
        }


        
        // GET: FinancialAccountsController/Edit/5
        public async Task<ActionResult> Edit(int? id, string? email, int? returnId, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (email == null)
            {
                return NotFound();
            }

            if (returnUrl == null)
            {
                return NotFound();
            }

            if (returnId != null)
            {
                ViewBag.ReturnId = returnId;
            }

            ViewBag.Email = email;
            ViewBag.ReturnUrl = returnUrl;

            try
            {
                var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{id.Value}");
                if (financialAccountDto == null)
                {
                    return NotFound();
                }

                return View(financialAccountDto);

            }
            catch (Exception)
            {
                _flashMessage.Danger("Error retrieving account for editing.");
                return RedirectToAction(nameof(CondoAccounts), new { email });
            }

        }

        // POST: FinancialAccountsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? email, FinancialAccountDto financialAccountDto)
        {
            if (id != financialAccountDto.Id)
            {
                return NotFound();
            }

            if (email == null)
            {
                return NotFound();
            }
         

            ViewBag.Email = email;

            if (!ModelState.IsValid)
            {
                return View(financialAccountDto);
            }

            if(financialAccountDto.BankName != null && financialAccountDto.AssociatedBankAccount != null && financialAccountDto.CardNumber != null)
            {
                financialAccountDto.IsActive = true;
            }
            else
            {
                financialAccountDto.IsActive = false;
            }

                try
                {
                    var result = await _apiCallService.PostAsync<FinancialAccountDto, Response<object>>($"api/FinancialAccounts/Edit/{id}", financialAccountDto);
                    if (!result.IsSuccess)
                    {
                        _flashMessage.Danger("Error updating account!");
                        return View(financialAccountDto);
                    }

                    _flashMessage.Confirmation("Financial details updated.");
                    if (User.IsInRole("CondoMember") || User.IsInRole("CompanyAdmin"))
                    {
                        return RedirectToAction(nameof(Details), new { email });
                    }
                    return RedirectToAction(nameof(CondoAccounts), new { email });
                }
                catch (Exception)
                {
                    _flashMessage.Danger("Error updating account");
                     return View(financialAccountDto);
            }

            

        }



        // GET: FinancialAccountsController/Deposit/5
        [HttpGet]
        public async Task<IActionResult> Deposit(int? id, string? email)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (email == null)
            {
                return NotFound();
            }

            


            ViewBag.Email = email;

            try
            {
                var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{id.Value}");
                if (financialAccountDto == null)
                {
                    return NotFound();
                }

                var model = new DepositViewModel
                {
                    OwnerId = financialAccountDto.Id,
                    DepositValue = 0, 
                };


                return View(model);

            }
            catch (Exception)
            {
                _flashMessage.Danger("Error retrieving account to deposit.");
                return NotFound();
            }

        }


        // POST: FinancialAccountsController/Deposit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int? id, string? email, DepositViewModel model)
        {
            if (id != model.OwnerId)
            {
                return NotFound();
            }

            if (email == null)
            {
                return NotFound();
            }

            ViewBag.Email = email;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.SelectedPaymentMethodId == 0)
            {
                _flashMessage.Danger("Choose a payment method.");
                return View(model);
            }

            var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{model.OwnerId}");

            if (model.DepositValue <= 0)
            {
                ModelState.AddModelError("DepositValue", "Deposit amount must be greater than 0.");
                return View(model);
            }

            //validação hard code dos campos das opções de pagamento
            if (model.SelectedPaymentMethodId == 2) // Cartão de Crédito
            {
                if (string.IsNullOrEmpty(model.CreditCardNumber) || string.IsNullOrEmpty(model.Cvv))
                {
                    ModelState.AddModelError("CreditCardNumber", "Credit card number and CVV are required.");
                    return View(model);                    
                }
            }
            else if (model.SelectedPaymentMethodId == 1) // MbWay
            {
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number is required.");
                    return View(model);
                }
            }
            else if (model.SelectedPaymentMethodId == 3)//AssociatedBankAccount
            {
                if (financialAccountDto.IsActive == false)
                {
                    ModelState.AddModelError("AssociatedBankAccount", "No Account associated");
                    return View(model);
                }

                model.CreditCardNumber = financialAccountDto.CardNumber;

            }


            try
            {

                var transaction = new TransactionDto
                {
                    DateAndTime = DateTime.Now,
                    BeneficiaryAccountId = financialAccountDto.Id,
                    ExternalRecipientBankAccount = $"{financialAccountDto.OwnerName}'s Wallet",
                    Amount = model.DepositValue
                };

                var resultTransaction = await _apiCallService.PostAsync<TransactionDto, Response<object>>("api/Transaction", transaction);
                if (!resultTransaction.IsSuccess)
                {
                    _flashMessage.Danger("Error creating transaction");
                    return View(model);
                }

                financialAccountDto.Balance += model.DepositValue;

                var result = await _apiCallService.PostAsync<FinancialAccountDto, Response<object>>($"api/FinancialAccounts/Edit/{id}", financialAccountDto);
                if (!result.IsSuccess)
                {
                    _flashMessage.Danger("Error making deposit!");
                    return View(model);
                }


           
                _flashMessage.Confirmation("Deposit successfull.");
                return RedirectToAction(nameof(Details), new { id = id, email = email });
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error making deposit!");
            }

            return View(model);

        }


        // GET: FinancialAccountsController/Withdrawal/5
        [HttpGet]
        public async Task<IActionResult> Withdrawal(int? id, string? email)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (email == null)
            {
                return NotFound();
            }




            ViewBag.Email = email;

            try
            {
                var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{id.Value}");
                if (financialAccountDto == null)
                {
                    return NotFound();
                }

                var model = new DepositViewModel
                {
                    OwnerId = financialAccountDto.Id,
                    DepositValue = 0,
                };


                return View(model);

            }
            catch (Exception)
            {
                _flashMessage.Danger("Error retrieving account to withdrawal.");
                return NotFound();
            }

        }


        // POST: FinancialAccountsController/Withdrawal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdrawal(int? id, string? email, DepositViewModel model)
        {
            if (id != model.OwnerId)
                return NotFound();

            if (email == null)
                return NotFound();

            ViewBag.Email = email;

            if (!ModelState.IsValid)
                return View(model);

            var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{model.OwnerId}");

            if (model.DepositValue <= 0)
            {
                ModelState.AddModelError("DepositValue", "Withdrawal amount must be greater than 0.");
                return View(model);
            }

            if (model.DepositValue > financialAccountDto.Balance)
            {
                ModelState.AddModelError("DepositValue", "Insufficient funds for this withdrawal.");
                return View(model);
            }

            if (financialAccountDto.IsActive == false)
            {
                ModelState.AddModelError("AssociatedBankAccount", "No Account associated");
                return View(model);
            }

            model.SelectedPaymentMethodId = 3;
            model.CreditCardNumber = financialAccountDto.CardNumber;

            try
            {
                var transaction = new TransactionDto
                {
                    DateAndTime = DateTime.Now,
                    PayerAccountId = financialAccountDto.Id,
                    ExternalRecipientBankAccount = financialAccountDto.OwnerName,
                    Amount = model.DepositValue
                };

                var resultTransaction = await _apiCallService.PostAsync<TransactionDto, Response<object>>("api/Transaction", transaction);
                if (!resultTransaction.IsSuccess)
                {
                    _flashMessage.Danger("Error creating transaction");
                    return View(model);
                }

                financialAccountDto.Balance -= model.DepositValue;

                var result = await _apiCallService.PostAsync<FinancialAccountDto, Response<object>>($"api/FinancialAccounts/Edit/{id}", financialAccountDto);
                if (!result.IsSuccess)
                {
                    _flashMessage.Danger("Error making withdrawal!");
                    return View(model);
                }

                _flashMessage.Confirmation("Withdrawal successfull.");
                return RedirectToAction(nameof(Details), new { id = id, email = email });
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error making withdrawal!");
            }

            return View(model);
        }







        public async Task<IActionResult> CondoAccounts(string? email)
        {
            if (email == null)
            {
                return NotFound();
            }

            IEnumerable<FinancialAccountDto> condoAccounts = new List<FinancialAccountDto>();
            
            ViewBag.Email = email;


            try
            {
                var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");

                var condominiums = await _apiCallService.GetAsync<IEnumerable<CondominiumDto>>("api/Condominiums");

                var managerCondos = condominiums.Where(c => c.ManagerUserId == user.Id);

                condoAccounts = managerCondos.Where(c => c.FinancialAccountDto != null).Select(c => c.FinancialAccountDto!);

                return View(condoAccounts);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching condos ");
                return View(condoAccounts);
            }


        }





        // GET: FinancialAccountsController/Transactions/5
        public async Task<IActionResult> Transactions(int? id, string? email)
        {
            if(id == null)
            {
                return NotFound();
            }
            var transactions = new List<TransactionDto>();




            ViewBag.Email = email;
            ViewBag.AccountId = id;

            try
            {
                transactions = await _apiCallService.GetAsync<List<TransactionDto>>($"api/Transaction/ByFinancialAccount/{id}");

                if (transactions == null || !transactions.Any())
                {
                    _flashMessage.Info("No transactions found for this account.");
                    return View(transactions);
                }

                return View(transactions);
            }

            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching transactions");
                return RedirectToAction("Details", "FinancialAccounts", new { id = id, email = email });
            }




        }



    }
}
