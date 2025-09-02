using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //TODO: [Authorize(Roles = "CompanyAdmin")]
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    public class FeesController : Controller
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserHelper _userHelper;

        public FeesController(IFeeRepository feeRepository,IConverterHelper converterHelper,IUserHelper userHelper)
        {
            _feeRepository = feeRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }
        
        [HttpGet("GetFees")]
        public async Task<IEnumerable<FeeDto>> GetFees()
        {
            var fees = await _feeRepository.GetAllFees();

            var feesDtos = fees.Select(f => _converterHelper.ToFeeDto(f)).ToList();

            return feesDtos;
        }


        [HttpPost("UpdateFees")]
        public async Task<IActionResult> UpdateFees([FromBody]IEnumerable<FeeDto> updatedFees)
        {
            try
            {
                await _feeRepository.UpdateFees(updatedFees);

                return Ok(new Response { IsSuccess = true, Message = "Fees updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating Fees.");
            }
        }
    }
}
