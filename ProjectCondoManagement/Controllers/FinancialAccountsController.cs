using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

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

        public FinancialAccountsController(IFinancialAccountRepository financialAccountRepository, IConverterHelper converterHelper, DataContextFinances dataContextFinances)
        {
            _financialAccountRepository = financialAccountRepository;
            _converterHelper = converterHelper;
            _context = dataContextFinances;
        }

        public IFinancialAccountRepository FinancialAccount { get; }
        public IConverterHelper ConverterHelper { get; }

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
    }
}
