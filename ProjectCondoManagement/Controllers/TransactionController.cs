using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Finances;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextFinances _context;
        private readonly IUserHelper _userHelper;

        public TransactionController(ITransactionRepository transactionRepository, IConverterHelper converterHelper, DataContextFinances dataContextFinances, IUserHelper userHelper)
        {
            _transactionRepository = transactionRepository;
            _converterHelper = converterHelper;
            _context = dataContextFinances;
            _userHelper = userHelper;
        }


        // GET api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, _context);

            if (transaction == null)
            {
                return NotFound();
            }

            var transactionDto = _converterHelper.ToTransactionDto(transaction, false);
            if (transaction == null)
            if (transactionDto == null)
            {
                return NotFound();
            }

            return transactionDto;
        }

        [HttpPost("GetTransactions")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions([FromBody] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            var transactions = await _transactionRepository.GetTransactions(user);

            var transactionsDtos = transactions.Select(t => _converterHelper.ToTransactionDto(t, false)).ToList();

            return transactionsDtos;
        }



        // POST: api/Condominiums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest("Request body is null.");
            }

            try
            {

                var transaction = _converterHelper.ToTransaction(transactionDto, true);

                if (transaction == null)
                {
                    return BadRequest("Conversion failed. Invalid data.");
                }


                await _transactionRepository.CreateAsync(transaction, _context);

                return Ok(new Response<object> { IsSuccess = true, Message = "Transaction created successfully." });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }




        // GET: api/Transaction/ByFinancialAccount/5
        [HttpGet("ByFinancialAccount/{id}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByFinancialAccountId(int id)
        {
            var transactions = await _transactionRepository.GetByFinancialAccountIdAsync(id);


            var transactionDtos = transactions.Select(t => _converterHelper.ToTransactionDto(t, false)).ToList();

            return transactionDtos;
        }




    }
}

