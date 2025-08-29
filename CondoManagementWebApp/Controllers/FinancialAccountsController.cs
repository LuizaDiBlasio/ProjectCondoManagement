using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: FinancialAccountsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FinancialAccountDto financialAccountDto)
        {
            if (id != financialAccountDto.Id)
            {
                return NotFound();
            }

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
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _flashMessage.Danger("Error updating account");
                }

            return View(financialAccountDto);

        }



        // GET: FinancialAccountsController/Deposit/5
        public async Task<IActionResult> Deposit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
                    DepositValue = 0, // Initialize to 0 or any default value
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
        public async Task<IActionResult> Deposit(int? id, DepositViewModel model)
        {
            if (id != model.OwnerId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
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

                financialAccountDto.Balance += model.DepositValue;

                var result = await _apiCallService.PostAsync<FinancialAccountDto, Response<object>>($"api/FinancialAccounts/Edit/{id}", financialAccountDto);
                if (!result.IsSuccess)
                {
                    _flashMessage.Danger("Error making deposit!");
                    return View(model);
                }

                _flashMessage.Confirmation("Deposit successfull.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error making deposit!");
            }

            return View(model);

        }


    }
}
