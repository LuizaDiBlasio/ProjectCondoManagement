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
                var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentWithTransacAndExp/{invoice.PaymentId}"); //vem sem transaction e sem mbway #

                model.Payment = paymentDto;


                //buscar payer account
                if(paymentDto.PayerFinancialAccountId != 0)
                {
                    var payerFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.PayerFinancialAccountId}");
                    model.PayerFinancialAccountDto = payerFinancialAccount;
                }


                //buscar account do condo 
                var condoFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.CondominiumId}");

                model.CondominiumFinancialAccountId = condoFinancialAccount.Id;



                if (paymentDto.TransactionDto != null)
                {

                    model.PaymentDate = paymentDto.TransactionDto.DateAndTime;

                    if (paymentDto.TransactionDto.BeneficiaryAccountId != null)
                    {
                        var beneficiaryFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.TransactionDto.BeneficiaryAccountId}");//problema aqui

                        model.BeneficiaryFinancialAccountDto = beneficiaryFinancialAccount;
                    }
                    else
                    {
                        model.BeneficiaryFinancialAccountDto = null;
                    }

                }

                return View(model);

            }
            catch
            {
                return View("Error");
            }
        }


        [HttpGet("IndexCondominiumInvoices")]
        public async Task<ActionResult<List<CondominiumWithInvoicesDto>>> IndexCondominiumInvoices()
        {
            try
            {
                //achar o condominio do condoManager
                var managerCondosWithInvoices = await _apiCallService.GetAsync<List<CondominiumWithInvoicesDto>>("api/Invoice/GetCondominiumsWithInvoices");
                if (managerCondosWithInvoices == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<CondominiumWithInvoicesDto>());
                }



                return View(managerCondosWithInvoices);

            }
            catch
            {
                return View("Error");
            }
        }


        [HttpGet("IndexCondoMemberInvoices")]
        public async Task<ActionResult<List<CondominiumWithInvoicesDto>>> IndexCondoMemberInvoices()
        {
            try
            {  
                var condoMemberInvoices = await _apiCallService.GetAsync<List<CondominiumWithInvoicesDto>>("api/Invoice/GetMemberCondominiumsWithInvoices");

                return View(condoMemberInvoices);

            }
            catch
            {
                return View("Error");
            }
        }



        [HttpGet("IndexAllCondoMembersInvoices")]
        public async Task<ActionResult<List<CondominiumWithInvoicesDto>>> IndexAllCondoMembersInvoices()
        {
            try
            {
                var condoMembersInvoices = await _apiCallService.GetAsync<List<CondominiumWithInvoicesDto>>($"api/Invoice/GetAllCondoMembersInvoices");

                return View(condoMembersInvoices);

            }
            catch
            {
                return View("Error");
            }
        }

    }
}
