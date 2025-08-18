using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using System.Collections.Generic;

namespace CondoManagementWebApp.Controllers
{
    public class FinancesController : Controller
    {
        private readonly IApiCallService _apiCallService;

        public FinancesController(IApiCallService apiCallService)
        {
            _apiCallService = apiCallService;
        }

        [HttpPost]
        [Route("Finances/SaveFees")]
        public async Task<IActionResult> SaveFees([FromBody]IEnumerable<Fee> changedFees)
        {
            try
            {
                List<Fee> updatedFees = new List<Fee>();

                var apiFees = await _apiCallService.GetAsync<IEnumerable<Fee>>("api/Finances/GetFees");

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

                var result = await _apiCallService.PostAsync<IEnumerable<Fee>,Response>("api/Finances/UpdateFees",updatedFees);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }


}

