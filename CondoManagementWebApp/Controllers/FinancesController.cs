using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using System.Collections.Generic;

namespace CondoManagementWebApp.Controllers
{
    public class FinancesController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFeeHelper _feeHelper;

        public FinancesController(IApiCallService apiCallService,IFeeHelper feeHelper)
        {
            _apiCallService = apiCallService;
            _feeHelper = feeHelper;
        }

        public async Task<IActionResult> FinancialAccount()
        {
            if (this.User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new FinancialAccountViewModel
            {
                User = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUser", $"{this.User.Identity.Name}"),
                Transactions = await _apiCallService.GetByQueryAsync<IQueryable<TransactionDto>>($"api/Transactions/GetTransactions",$"{this.User.Identity.Name}"),
            };

            if(model.User.CompanyId != null)
            {
                model.Company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/{model.User.CompanyId.Value}");
            }

            return View(model);
        } 

        [HttpPost]
        [Route("Finances/SaveFees")]
        public async Task<IActionResult> SaveFees([FromBody]IEnumerable<FeeDto> changedFees)
        {
            try
            {
                List<FeeDto> updatedFees = new List<FeeDto>();

                var apiFees = await _feeHelper.GetFeesAsync();

                foreach (var fee in changedFees)
                {
                    var updatedFee = apiFees
                    .SingleOrDefault(f => f.Id == fee.Id);

                    if (updatedFee != null)
                    {
                        updatedFee.Value = fee.Value;
                        updatedFees.Add(updatedFee);
                    }

                }

                var result = await _apiCallService.PostAsync<IEnumerable<FeeDto>,Response>("api/Fees/UpdateFees",updatedFees);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }


}

