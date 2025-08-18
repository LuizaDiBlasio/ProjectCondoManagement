using ClassLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Controllers
{    
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //TODO: [Authorize(Roles = "CompanyAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FinancesController : Controller
    {
        private readonly DataContextFinances _context;
        private readonly IFeeRepository _feeRepository;

        public FinancesController(DataContextFinances context,IFeeRepository feeRepository)
        {
            _context = context;
            _feeRepository = feeRepository;
        }
        
        [HttpGet("GetFees")]
        public async Task<IEnumerable<Fee>> GetFees()
        {
            var fees = await _feeRepository.GetAll(_context).ToListAsync();
            return fees;
        }


        [HttpPost("UpdateFees")]
        public async Task<IActionResult> UpdateFees([FromBody]IEnumerable<Fee> updatedFees)
        {
            try
            {
                foreach (var fee in updatedFees)
                {
                    await _feeRepository.UpdateAsync(fee,_context);
                }

                return Ok(new Response { IsSuccess = true, Message = "Fees updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating Fees.");
            }
        }
    }
}
