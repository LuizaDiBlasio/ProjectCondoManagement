using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public TransactionsController(ITransactionRepository transactionRepository, IUserHelper userHelper,IConverterHelper converterHelper)
        {
            _transactionRepository = transactionRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        [HttpPost("GetTransactions")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions([FromBody] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            var transactions = await _transactionRepository.GetTransactions(user);

            var transactionsDtos = transactions.Select(t => _converterHelper.ToTransactionDto(t, false)).ToList();

            return transactionsDtos;
        }
    }
}
