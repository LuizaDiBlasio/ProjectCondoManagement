using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using ProjectCondoManagement.Migrations.CondosDb;
using System.Threading.Tasks;
using System.Web.Mvc;
using Twilio.Rest.Api.V2010.Account.Call;

namespace ProjectCondoManagement.Controllers
{

    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : Microsoft.AspNetCore.Mvc.Controller
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IExpenseRepository _expenseRepository;
        private readonly ITransactionRepository _transactionRepository; 
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentController(IPaymentRepository paymentRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper, IUserHelper userHelper,
            ICondominiumRepository condominiumRepository, DataContextCondos dataContextCondos, IExpenseRepository expenseRepository, ITransactionRepository transactionRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository; 
            _dataContextCondos = dataContextCondos;
            _expenseRepository = expenseRepository;
            _transactionRepository = transactionRepository; 
            _invoiceRepository = invoiceRepository;
        }


        // GET: PaymensController
        [Microsoft.AspNetCore.Mvc.HttpPost("GetCondoMemberPayments")]
        public async Task<List<PaymentDto>> GetCondoMemberPayments([FromBody] string userEmail)
        {
            //get condoMember user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return new List<PaymentDto>();
            }

            // get user payments
            var allPayments = _paymentRepository.GetAll(_dataContextFinances).ToList();

            var userPayments = allPayments.Where(p => p.PayerFinancialAccountId == user.FinancialAccountId).ToList();

            //converter
            var userPaymentsDto = userPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

            return userPaymentsDto;
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("GetCondoPayments")]
        public async Task<List<PaymentDto>> GetCondoPayments([FromBody] string condoId)
        {
            // get condominium 
            var condominium = await _condominiumRepository.GetByIdAsync(int.Parse(condoId), _dataContextCondos);
            if(condominium == null)
            {
                return new List<PaymentDto>();  
            }


            var allPayments = _paymentRepository.GetAll(_dataContextFinances).ToList();

            if (condoId != null)
            {
                var condoPayments = allPayments.Where(p => p.PayerFinancialAccountId == condominium.FinancialAccountId).ToList();

                //converter

                var condoPaymentsDto = condoPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

                return condoPaymentsDto;
            }

            return new List<PaymentDto>();
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("GetAllCondoMemberPayments")]
        public async Task<List<PaymentDto>> GetAllCondoMemberPayments([FromBody] string condoId)
        {
            var condo = await _condominiumRepository.GetByIdAsync(int.Parse(condoId), _dataContextCondos);

            if(condo == null)
            {
                return new List<PaymentDto>();
            }

            var usersCondoMembers = new List<User>();

            if(condo.CondoMembers != null)
            {
                //converter condoMembers para users
                foreach (var condoMember in condo.CondoMembers)
                {
                    var user = await _userHelper.GetUserByEmailAsync(condoMember.Email);
                    usersCondoMembers.Add(user);
                }

                var allPayments = _paymentRepository.GetAll(_dataContextFinances).ToList();

                var allCondomemberPayments = new List<Payment>();

                allCondomemberPayments = allPayments
                                .Where(payment => usersCondoMembers
                                .Any(user => user.FinancialAccountId == payment.PayerFinancialAccountId))
                                .ToList();

                //converter
                var allCondomemberPaymentsDto = allCondomemberPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

                return allCondomemberPaymentsDto;
            }

            return new List<PaymentDto>();
        }


        // GET: PaymensController/PaymentDetails/5

        [Microsoft.AspNetCore.Mvc.HttpGet("GetPaymentDetails/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(int id)
        {
            var paymentWithExpenses = await _paymentRepository.GetPaymentWithExpenses(id);

            if (paymentWithExpenses != null)
            {
                var paymentDto = _converterHelper.ToPaymentDto(paymentWithExpenses, false);

                return Ok(paymentWithExpenses);
            }

            return NotFound(new PaymentDto());

        }

        [Microsoft.AspNetCore.Mvc.HttpPost("CreateOneTimePayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateOneTimePayment([FromBody] PaymentDto paymentDto)
        {
            try
            {
                //converter para payment
                var payment = _converterHelper.ToPayment(paymentDto, true);

                //criar expense

                if(payment.OneTimeExpense != null)
                {
                    var oneTimeExpense = new Expense()
                    {
                        Id = 0,
                        Amount = payment.OneTimeExpense.Amount,
                        ExpenseType = payment.OneTimeExpense.ExpenseType,
                        Detail = payment.OneTimeExpense.Detail,
                    };

                    await _expenseRepository.CreateAsync(oneTimeExpense, _dataContextFinances);

                    payment.Expenses.Add(oneTimeExpense);
                }

               //criar payment

                await _paymentRepository.CreateAsync(payment, _dataContextFinances);

                return Ok( new Response () { IsSuccess = true});
            }
            catch
            {
                return BadRequest(new Response () { IsSuccess = false, Message = "Unable to issue payment due to serve error" });
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("CreateRecurringPayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateRecurringPayment([FromBody] PaymentDto paymentDto)
        {
            try
            {
                //converter para payment
                var payment = _converterHelper.ToPayment(paymentDto, true);


                //criar payment

                await _paymentRepository.CreateAsync(payment, _dataContextFinances);

                return Ok(new Response() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable to issue payment due to serve error" });
            }
        }





        //// POST: PaymensController/MakePayment 
        [Microsoft.AspNetCore.Mvc.HttpPost("MakePayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> MakePayment([FromBody] PaymentDto paymentDto)
        {
            try
            {
                //criar transaction

                var transaction = _converterHelper.ToTransaction(paymentDto.TransactionDto, true);

                await _transactionRepository.CreateAsync(transaction, _dataContextFinances);

                //criar invoice

                var invoice = new Invoice()
                {
                    PaymentDate = transaction.DateAndTime,
                    CondominiumId = paymentDto.CondominiumId,
                    PayerAccountId = transaction.PayerAccountId,
                    BeneficiaryAccountId = transaction.BeneficiaryAccountId,
                    PaymentId = paymentDto.Id,
                };

                await _invoiceRepository.CreateAsync(invoice, _dataContextFinances);    
                
                paymentDto.InvoiceId = invoice.Id;
                paymentDto.TransactionId = transaction.Id;

                //fazer update do pagamento

                var payment = _converterHelper.ToPayment(paymentDto, false);

                payment.Transaction = transaction;
                payment.Invoice = invoice;

                await _paymentRepository.UpdateAsync(payment, _dataContextFinances);

                return Ok(new Response() { IsSuccess = true, Message = "Payment successful" });
               
            }
            catch
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable to make payment due to server error" });
            }
        }


        // POST: PaymensController/Delete/5 CANCEL PAYMENT
        [Microsoft.AspNetCore.Mvc.HttpPost("Delete/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentWithExpenses(id);
                if (payment == null)
                {
                    return NotFound();
                }

                if (payment.IsPaid == false && payment.Transaction == null && payment.Invoice == null)
                {
                    await _paymentRepository.DeleteAsync(payment, _dataContextFinances);
                    return Ok();
                }

               return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        //Metodos auxiliares

        [Microsoft.AspNetCore.Mvc.HttpGet("GetPaymentMethodsList")] 
        public List<SelectListItem> GetPaymentMethodsList()
        {
            return _paymentRepository.GetPaymentMethodsList();
        
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("GetSelectedPaymentMethod/{id}")]
        public string GetPaymentMethod(int id)
        {
            var paymentMethodList = _paymentRepository.GetPaymentMethodsList();

            var selectedPaymentMethod = paymentMethodList.FirstOrDefault(pm => pm.Value == id.ToString());

            if(selectedPaymentMethod != null)
            {
                return selectedPaymentMethod.Text.ToString();
            }

            return string.Empty;    
            
        }
    }
}
