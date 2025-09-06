using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Web.Mvc;

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
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public PaymentController(IPaymentRepository paymentRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper, IUserHelper userHelper,
            ICondominiumRepository condominiumRepository, DataContextCondos dataContextCondos, IExpenseRepository expenseRepository, ITransactionRepository transactionRepository, IInvoiceRepository invoiceRepository,
            IFinancialAccountRepository financialAccountRepository)
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
            _financialAccountRepository = financialAccountRepository;
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
            var allPayments = _paymentRepository.GetAll(_dataContextFinances)
                                                .Include(p => p.Expenses)
                                                .Include(p =>p.Transaction)
                                                .ToList();

            var userPayments = allPayments
                                .Where(p => p.PayerFinancialAccountId == user.FinancialAccountId)
                                .ToList();

            //converter
            var userPaymentsDto = userPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

            return userPaymentsDto;
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("GetCondoPayments")]
        public async Task<List<PaymentDto>> GetCondoPayments([FromBody] string condoId)
        {
            // get condominium 
            var condominium = await _condominiumRepository.GetByIdAsync(int.Parse(condoId), _dataContextCondos);
            if (condominium == null)
            {
                return new List<PaymentDto>();
            }


            if (condoId != null)
            {
                var condoPayments = _paymentRepository.GetAll(_dataContextFinances)
                                                      .Where(p => p.PayerFinancialAccountId == condominium.FinancialAccountId)
                                                      .Include(p => p.Expenses)
                                                      .Include(p => p.Transaction)
                                                      .ToList();

                //converter

                var condoPaymentsDto = condoPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

                return condoPaymentsDto;
            }

            return new List<PaymentDto>();
        }


        [Microsoft.AspNetCore.Mvc.HttpGet("GetAllCondoMembersPayments")]
        public async Task<List<CondominiumWithPaymentsDto>> GetAllCondoMemberPayments()
        {
            var email = this.User.Identity?.Name;
            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            // buscar todos os condomínios do manager
            var condominiums = await _condominiumRepository.GetAll(_dataContextCondos)
                .Where(c => c.ManagerUserId == user.Id)
                .ToListAsync();

            if (condominiums == null || !condominiums.Any())
            {
                return new List<CondominiumWithPaymentsDto>();
            }

            // buscar os emails de todos os membros de todos os condomínios
            var allCondoMemberEmails = new List<string>();
            foreach (var condo in condominiums)
            {
                var condoMembers = await _condominiumRepository.GetCondoCondomembers(condo.Id);
                allCondoMemberEmails.AddRange(condoMembers.Select(cm => cm.Email));
            }

            // Buscar os Users desses emails
            var allCondoMemberUsers = await _userHelper.GetUsersByEmailsAsync(allCondoMemberEmails);

            // Extrair os FinancialAccountIds de todos os users
            var memberFinancialAccountIds = allCondoMemberUsers
                .Where(u => u.FinancialAccountId.HasValue)
                .Select(u => u.FinancialAccountId.Value)
                .Distinct()
                .ToList();

            if (!memberFinancialAccountIds.Any())
            {
                return new List<CondominiumWithPaymentsDto>();
            }

            // Buscar todos os pagamentos de todos os membros 
            var allMemberPayments = await _paymentRepository.GetAll(_dataContextFinances)
                .Where(p => memberFinancialAccountIds.Contains(p.PayerFinancialAccountId))
                .Include(p => p.Expenses)
                .Include(p => p.Transaction)
                .ToListAsync();

            // Converter 
            var allMemberPaymentsDto = allMemberPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList();

            // instanciar lista
            var condosWithPaymentsDtoList = new List<CondominiumWithPaymentsDto>();

            foreach (var condo in condominiums)
            {
                // Encontrar os pagamentos para o condomínio atual, usando o pelo condo id
                var condoPaymentsDto = allMemberPaymentsDto
                    .Where(p => p.CondominiumId == condo.Id)
                    .ToList();

            
                var condominiumWithPaymentsDto = new CondominiumWithPaymentsDto
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    CondoPayments = condoPaymentsDto
                };
                condosWithPaymentsDtoList.Add(condominiumWithPaymentsDto);
            }

            return condosWithPaymentsDtoList;
        }


        // GET: PaymensController/PaymentDetails/5

        [Microsoft.AspNetCore.Mvc.HttpGet("GetPaymentDetails/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(int id)
        {
            var paymentWithExpenses = await _paymentRepository.GetPaymentWithExpenses(id);

            if (paymentWithExpenses != null)
            {
                var paymentDto = _converterHelper.ToPaymentDto(paymentWithExpenses, false);

                return Ok(paymentDto);
            }

            return NotFound(new PaymentDto());

        }

        [Microsoft.AspNetCore.Mvc.HttpGet("GetPaymentWithTransacAndExp/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> GetPaymentWithTransacAndExp(int id)
        {
            var payment = await _paymentRepository.GetPaymentWithExpensesAndTransaction(id);

            if (payment != null)
            {
                var paymentDto = _converterHelper.ToPaymentDto(payment, false);

                return Ok(paymentDto);
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

                //criar payment

                if(payment.OneTimeExpense != null)
                {

                    payment.Expenses.Add(payment.OneTimeExpense);
                }

                await _paymentRepository.CreateAsync(payment, _dataContextFinances);


                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to issue payment due to serve error" });
            }
        }



        //// POST: PaymensController/MakePayment 
        [Microsoft.AspNetCore.Mvc.HttpPost("MakePayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> MakePayment([FromBody] PaymentDto paymentDto)
        {

            try
            {
                //buscar payment
                var payment = await _paymentRepository.GetByIdAsync(paymentDto.Id, _dataContextFinances);

                if (payment == null)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "Payment not found." });
                }

                if (payment.IsPaid)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "Payment has already been made." });
                }

                payment.PaymentMethod = paymentDto.PaymentMethod;
                payment.CreditCard = paymentDto.CreditCard;
                payment.MbwayNumber = paymentDto.MbwayNumber;

                //converter para transaction
                var transaction = _converterHelper.ToTransaction(paymentDto.TransactionDto, true);

                //associar
                payment.Transaction = transaction;

                //criar invoice
                var invoice = new Invoice()
                {
                    PaymentDate = transaction.DateAndTime,
                    CondominiumId = payment.CondominiumId,
                    PayerAccountId = transaction.PayerAccountId,
                    BeneficiaryAccountId = transaction.BeneficiaryAccountId
                };


                //associar
                payment.Invoice = invoice;

                // marcar o pagamento como pago
                payment.IsPaid = true;


                //salvar alterações
                await _paymentRepository.UpdateAsync(payment, _dataContextFinances);

                
                //buscar payment e atribuir ids
                var updatedPayment = await _paymentRepository.GetByIdAsync(payment.Id, _dataContextFinances);

                updatedPayment.TransactionId = updatedPayment.Transaction.Id;
                updatedPayment.InvoiceId = updatedPayment.Invoice.Id;

                //nova atualização
                await _paymentRepository.UpdateAsync(updatedPayment, _dataContextFinances);


                return Ok(new Response<object>() { IsSuccess = true, Message = "Payment successful" });

            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to make payment due to server error" });
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("DeductPayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DeductPayment([FromBody] CashFlowDto deductDto)
        {
            try
            {
                var financialAccount = await _financialAccountRepository.GetByIdAsync(deductDto.AccountId, _dataContextFinances);

                // deduzir pagamento

                if (financialAccount.Balance < deductDto.TotalAmount)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "Insufficient balance" });
                }

                financialAccount.Balance = financialAccount.Balance - deductDto.TotalAmount;

                // update da financial account
                await _financialAccountRepository.UpdateAsync(financialAccount, _dataContextFinances);

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to process payment" });
            }

        }

        [Microsoft.AspNetCore.Mvc.HttpPost("IncomePayment")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> IncomePayment([FromBody] CashFlowDto incomeDto)
        {
            try
            {
                var financialAccount = await _financialAccountRepository.GetByIdAsync(incomeDto.AccountId, _dataContextFinances);

                // deduzir pagamento
                financialAccount.Balance = financialAccount.Balance + incomeDto.TotalAmount;

                // update da financial account
                await _financialAccountRepository.UpdateAsync(financialAccount, _dataContextFinances);

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to process with payment" });
            }

        }




        // POST: PaymensController/Delete/5 CANCEL PAYMENT
        [Microsoft.AspNetCore.Mvc.HttpDelete("Delete/{id}")]
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
                else
                {
                    return Conflict();
                }
                
            }
            catch
            {
                return BadRequest();
            }
        }

        //Metodos auxiliares

        
    }
}
