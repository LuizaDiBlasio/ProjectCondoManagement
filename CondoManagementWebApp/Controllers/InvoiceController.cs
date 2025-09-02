using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;
        
        

        public InvoiceController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }

        //GET Index
        //TODO fazer index
        //my invoices
        //condo invoices
        //condoMembers invoices

        //GET Invoice/InvoiceDetails/5
        [HttpGet("InvoiceDetails/{id}")]
        public async Task<ActionResult> InvoiceDetails(int id)
        {
            try
            {
                var model = new InvoiceDetailsViewModel();

                var invoice = await _apiCallService.GetAsync<InvoiceDto>($"api/Invoice/InvoiceDetails/{id}");

                if (invoice == null)
                {
                    return View(model);
                }

                //buscar payment
                var payment = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{invoice.PaymentId}");

                model.Payment = payment;


                //buscar payer account
                if(payment.PayerFinancialAccountId != 0)
                {
                    var payerFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{payment.PayerFinancialAccountId}");
                    model.PayerFinancialAccountDto = payerFinancialAccount;
                }
            

                //buscar beneficiary account
                var transaction = await _apiCallService.GetAsync<TransactionDto>($"api/Transaction/{payment.TransactionId}");

                if(transaction != null)
                {
                    model.Payment.TransactionDto = transaction;

                    model.PaymentDate = transaction.DateAndTime;

                    var beneficiaryFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{transaction.BeneficiaryAccountId}");
                    model.BeneficiaryFinancialAccountDto = beneficiaryFinancialAccount;
                }
                

                return View(model);

            }
            catch
            {
                return View("Error");
            }
        }

    }
}
