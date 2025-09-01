using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;
        private readonly IPaymentHelper _paymentHelper; 



        public PaymentController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper, IPaymentHelper paymentHelper )
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
            _paymentHelper = paymentHelper;
        }


        // GET: PaymentController
        [HttpGet("IndexCondominiumPayments")]
        public async Task<ActionResult<List<PaymentDto>>> IndexCondominiumPayments()
        {
            try
            {
                //achar o condominio do condoManager
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<PaymentDto>());
                }

                var condoPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("api/Payment/GetCondoPayments", $"{condoManagerCondo.Id}");

                return View(condoPayments);

            }
            catch
            {
                return View("Error");
            }
        }


        // GET: PaymentController
        [HttpGet("IndexAllCondoMembersPayments")]
        public async Task<ActionResult<List<PaymentDto>>> IndexAllCondoMembersPayments()
        {
            try
            {
                //achar o condominio do condoManager
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<PaymentDto>());
                }

                var allCondoMemeberPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("api/Payment/GetAllCondoMemberPayments", $"{condoManagerCondo.Id}");

                return View(allCondoMemeberPayments);
            }
            catch
            {
                return View("Error");
            }
        }


        // GET: PaymentController
        [HttpGet("IndexCondoMemberPayments")]
        public async Task<ActionResult<List<PaymentDto>>> IndexCondoMemberPayments()
        {
            try
            {
                var condoMemberPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("api/Condominiums/GetCondoMemberPayments", this.User.Identity.Name);

                if (condoMemberPayments == null)
                {
                    return View(new List<PaymentDto>());
                }

                return View(condoMemberPayments);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet("Details/{id}")]
        // GET: PaymentController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //puxar a list de expenses
            var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{id}");
            if(paymentDto == null)
            {
                _flashMessage.Danger("An error has occurred: payment not found");
                return View(new PaymentDto());  
            }



            return View(paymentDto);
        }


        [HttpGet("CreateRecurrentPayment")]
        // GET: PaymentController/CreateRecurringPayment
        public async Task<ActionResult> CreateRecurrentPayment() 
        {
            var model = new CreateRecurringPaymentViewModel();

            var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
            if (condoManagerCondo == null)
            {
                _flashMessage.Danger("You are not managing any condominiums currently");
                return View(new CreateRecurringPaymentViewModel());
            }

            var expensesList = await _apiCallService.GetAsync<List<SelectListItem>>($"api/Expense/GetExpensesList/{condoManagerCondo.Id}");

            if (expensesList.Any())
            {
                model.ExpensesToSelect = expensesList;
            }

            model.CondominiumId = condoManagerCondo.Id;

            return View(model);
        }

        [HttpGet("CreateOneTimePayment")]
        // GET: PaymentController/CreateOneTimePayment
        public async Task<ActionResult> CreateOneTimePayment()
        {
            var model = new CreateOneTimePaymentViewModel();

            var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
            if (condoManagerCondo == null)
            {
                _flashMessage.Danger("You are not managing any condominiums currently");
                return View(new CreateOneTimePaymentViewModel());
            }

            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            if (expenseTypeList.Any())
            {
                model.ExpenseTypesList = expenseTypeList;
            }

            model.CondominiumId = condoManagerCondo.Id;

            return View(model);
        }

        // POST: PaymentController/RequestCreateOneTimePaymentCondo
        [HttpPost("RequestCreateOneTimePayment")]
        public async Task<ActionResult> RequestCreateOneTimePayment(CreateOneTimePaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to issue one time payment");
                return View("CreateOneTimePayment", model); 
            }

            try
            {
                //PREENCHER MODEL (propriedades fora da view)
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
                }

                //model.CondominiumId = condoManagerCondo.Id;

                var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

                var expenseType = expenseTypeList.FirstOrDefault(e => e.Value == model.ExpenseTypeValue.ToString());
                if(expenseType == null)
                {
                    _flashMessage.Danger("Unable to select this type of expense");
                    return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
                }

                //criar expense 
                var expenseDto = new ExpenseDto()
                    {
                        Amount = model.ExpenseAmount,
                        Detail = model.ExpenseDetail,
                        ExpenseTypeDto = new EnumDto() { Value = model.ExpenseTypeValue, Name = expenseType.Text},
                        CondominiumId = model.CondominiumId,    
                    };


                //CONVERTER PARA PAYMENT DTO
                var paymentDto = _converterHelper.FromOneTimeToPaymentDto(model);

                //atribur propriedades fora do view model

                paymentDto.IssueDate = DateTime.Now;

                paymentDto.OneTimeExpenseDto = expenseDto; 

                var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/CreateOneTimePayment", paymentDto);

                if (apiCall.IsSuccess)
                {
                    if(condoManagerCondo.FinancialAccountId == paymentDto.PayerFinancialAccountId)
                    {
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }

                _flashMessage.Danger("Unable to issue payment");
                return View("CreateOneTimePayment", model);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: PaymentController/RequestCreateRecurringPayment
        [HttpPost("RequestCreateRecurrentPayment")] 
        public async Task<ActionResult> RequestCreateRecurrentPayment(CreateRecurringPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to issue one time payment");
                return View("CreateOneTimePayment", model);
            }

            try
            {

                //PREENCHER MODEL (propriedades fora da view)
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View("CreateRecurringPayment", new CreateRecurringPaymentViewModel());
                }

                model.CondominiumId = condoManagerCondo.Id;

                //CONVERTER PARA PAYMENT DTO
                var paymentDto = _converterHelper.FromRecurringToPaymentDto(model);

                //atribur propriedade fora do view model
                paymentDto.IssueDate = DateTime.Now;

                var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/CreateRecurringPayment", paymentDto);

                if (apiCall.IsSuccess)
                {
                    if (condoManagerCondo.FinancialAccountId == paymentDto.PayerFinancialAccountId)
                    {
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }

                _flashMessage.Danger("Unable to issue payment");
                return View("CreateRecurringPayment", model);
            }
            catch
            {
                return View("Error");
            }
        }


        // GET: PaymentController/Edit/5
        [HttpGet("MakePayment/{id}")]
        public async Task<ActionResult> MakePayment(int id)
        {
            var model = new MakePaymentViewModel();

            var paymentMethodList = _paymentHelper.GetPaymentMethodsList();

            if (paymentMethodList.Any())
            {
                model.PaymentMethods = paymentMethodList;
            }

            model.SelectedPaymentMethodId = 0;

            //descobrir o role do user

            string userRole = string.Empty;

            if (User.IsInRole("CondoManager"))
            {
                userRole = "CondoManager";
            }

            if (User.IsInRole("CondoMember"))
            {
                userRole = "CondoMember";
            }


            model.BeneficiaryTypeList = _paymentHelper.GetBeneficiaryTypesList(userRole);

            //atribuir id do pagamento
            model.Id = id;   

            return View(model);
        }


        //RequestMakePayment


        // POST: PaymentController/Edit/5
        [HttpPost("RequestMakePayment")]
        public async Task<ActionResult> RequestMakePayment(MakePaymentViewModel model) //ID HIDDEN
        {
            //validação hard code dos campos das opções de pagamento
            if (model.SelectedPaymentMethodId == 2) // Cartão de Crédito
            {
                if (string.IsNullOrEmpty(model.CreditCardNumber) || string.IsNullOrEmpty(model.Cvv))
                {
                    ModelState.AddModelError("CreditCardNumber", "Credit card number and CVV are required.");
                    // Ou adicione a validação em campos específicos
                }
            }
            else if (model.SelectedPaymentMethodId == 1) // MbWay
            {
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number is required.");
                }
            }
            else if(model.SelectedPaymentMethodId == 3)//omah wallet
            {
                if(model.PayerFinancialAccountId == 0)
                {
                    ModelState.AddModelError("OmahWalletNumber", "Omah wallet number is required");
                }

            }

            //validação manual para account id ou external bank account
            if(model.SelectedBeneficiaryId != 3)
            {
                if (model.BeneficiaryAccountId == null)
                {
                    ModelState.AddModelError("BeneficiaryAccountId", "Beneficiary Account Id is required");
                }

            }
            else
            {
                if(model.ExternalRecipientBankAccount == null)
                {
                    ModelState.AddModelError("ExternalRecipientBankAccount", "Recipient's bank account is required");
                }
            }


            if (!ModelState.IsValid)
            {
                return View("MakePayment", model);
            }

            try
            {

                //buscar metodo de pagamento
                var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{model.Id}");

                var paymentMethod = _paymentHelper.GetSelectedPaymentMethod(model.SelectedPaymentMethodId);

                if (paymentMethod == string.Empty)
                {
                    _flashMessage.Danger("Unable to select payment method");
                    return View("MakePayment", model);
                }

                paymentDto.PaymentMethod = paymentMethod;

                //caso pagamento seja feito com conta Omah, certificar que conta esteja ativa
                if(paymentMethod == "Omah Wallet")
                {
                    var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.PayerFinancialAccountId}");

                    if (!financialAccountDto.IsActive)
                    {
                        _flashMessage.Danger("Payment failed, your Omah Wallet is inactive");
                        return View("MakePayment", model);
                    }
                }

                //caso seja um beneficiario externo , não atribuir a conta Omah e atribuir a conta de banco externa
                if (model.SelectedBeneficiaryId == 3)
                {
                    model.BeneficiaryAccountId = null;   
                }
                else //caso seja pagamento interno ver se a account do beneficiary está válida
                {
                    var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{model.BeneficiaryAccountId}");

                    if (!financialAccountDto.IsActive)
                    {
                        _flashMessage.Danger("Payment failed, recipient's Omah Wallet is inactive");
                        return View("MakePayment", model);
                    }
                }

                    //criar transaction
                    paymentDto.TransactionDto = new TransactionDto() //o id vai ser zero por enquanto 
                    {
                        PaymentId = paymentDto.Id,
                        DateAndTime = DateTime.Now,
                        PayerAccountId = paymentDto.PayerFinancialAccountId,
                        BeneficiaryAccountId = model.BeneficiaryAccountId,
                        ExternalRecipientBankAccount = model.ExternalRecipientBankAccount,
                    };

                //subtrair de account caso method payment seja omahWallet

                if(model.SelectedPaymentMethodId == 3)
                {
                    var deductDto = new CashFlowDto()
                    {
                        TotalAmount = paymentDto.TotalAmount,
                        AccountId = paymentDto.PayerFinancialAccountId,
                    }; 

                    var deductAccount = await _apiCallService.PostAsync<CashFlowDto, Response<object>>("api/Payment/DeductPayment", deductDto);

                    if(!deductAccount.IsSuccess)
                    {
                        _flashMessage.Danger(deductAccount.Message);
                        return View("MakePayment", model);
                    }
                }

                //adicionar ao account do beneficiario
                if (model.SelectedBeneficiaryId != 3) // se não for externo
                {
                    var incomeDto = new CashFlowDto()
                    {
                        TotalAmount = paymentDto.TotalAmount,
                        AccountId = paymentDto.TransactionDto.BeneficiaryAccountId.Value,
                    };

                    if (model.BeneficiaryAccountId != 0)
                    {
                        var incomePay = await _apiCallService.PostAsync<CashFlowDto, Response<object>>("api/Payment/IncomePayment", incomeDto);

                        if (!incomePay.IsSuccess)
                        {
                            _flashMessage.Danger(incomePay.Message);
                            return View("MakePayment", model);
                        }
                    }
                }
               

                //Fazer requisição para fazer fazer pagamento

                var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/MakePayment", paymentDto); 

                if(apiCall.IsSuccess)
                {
                    _flashMessage.Confirmation(apiCall.Message);
                    return View("MakePayment", model);
                }

                _flashMessage.Confirmation(apiCall.Message); 
                return View("MakePayment", model);
            }
            catch
            {
                return View("Error");
            }
        }


        // POST: PaymentController/Delete/5
        [HttpPost("RequestDelete")]
        public async Task<ActionResult> RequestDelete(int id)
        {
            try
            {
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);

                var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{id}");

                var apiCall = await _apiCallService.DeleteAsync($"api/Payment/Delete/{id}");

                if(apiCall.IsSuccessStatusCode)
                {
                    if (condoManagerCondo.FinancialAccountId == paymentDto.PayerFinancialAccountId)
                    {
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }

                return Json(new { success = false, message = "An HTTP error occurred. Please try again later." });
            }
            catch
            {
                return Json(new { success = false, message = "An HTTP error occurred. Please try again later." });
            }
        }
    }
}
