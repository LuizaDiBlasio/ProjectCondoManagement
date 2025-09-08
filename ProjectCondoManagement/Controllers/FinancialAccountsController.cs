using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FinancialAccountsController : Controller
    {
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextFinances _context;

        public FinancialAccountsController(IFinancialAccountRepository financialAccountRepository, IConverterHelper converterHelper, DataContextFinances context)
        {
            _financialAccountRepository = financialAccountRepository;
            _converterHelper = converterHelper;
            _context = context;
        }

        //public IFinancialAccountRepository FinancialAccount { get; }
        //public IConverterHelper ConverterHelper { get; }

        // GET api/FinancialAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialAccountDto>> GetUsersFinancialAccount(int id)
        {
            var financialAccount = await _financialAccountRepository.GetByIdAsync(id, _context);

            if (financialAccount == null)
            {
                return NotFound();
            }

            var financialAccountDto = _converterHelper.ToFinancialAccountDto(financialAccount, false);
            if (financialAccountDto == null)
            {
                return NotFound();
            }

            return financialAccountDto;
        }


        // POST: api/FinancialAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditFinancialAccount(int id, [FromBody] FinancialAccountDto financialAccountDto)
        {
            if (id != financialAccountDto.Id)
            {
                return BadRequest();
            }

            var exists = await _financialAccountRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return NotFound();
            }


            var financialAccount = _converterHelper.ToFinancialAccount(financialAccountDto, false);

            try
            {
                await _financialAccountRepository.UpdateAsync(financialAccount, _context);

                return Ok(new Response<object> { IsSuccess = true, Message = "Account updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the Account.");
            }

        }



    }
}
