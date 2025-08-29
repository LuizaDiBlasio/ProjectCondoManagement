using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
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



        // GET: FinancialAccountsController/Edit/5
        public async Task<ActionResult> Deposit(int? id)
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
                _flashMessage.Danger("Error retrieving account to deposit.");
                return NotFound();
            }

        }





    }
}
