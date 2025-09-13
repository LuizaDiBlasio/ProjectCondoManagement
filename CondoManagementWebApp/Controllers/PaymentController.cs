using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
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
        public async Task<ActionResult<List<CondominiumWithPaymentsDto>>> IndexCondominiumPayments()
        {
            try
            {
                //achar o condominio do condoManager
                var condosWithPayments = await _apiCallService.GetAsync<List<CondominiumWithPaymentsDto>>($"api/Condominiums/GetCondoManagerCondominiumsWithPayments/{this.User.Identity.Name}") ;
                if (condosWithPayments == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<PaymentDto>());
                }                            


                return View(condosWithPayments);

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
                var allCondoMemeberPayments = await _apiCallService.GetAsync<List<CondominiumWithPaymentsDto>>("api/Payment/GetAllCondoMembersPayments");

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
                var condoMemberPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("api/Payment/GetCondoMemberPayments", this.User.Identity.Name);

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
            var model = new PaymentDetailsViewModel();

            //puxar a list de expenses
            var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{id}");
            if(paymentDto == null)
            {
                _flashMessage.Danger("An error has occurred: payment not found");
                return View(new PaymentDto());  
            }

            model.PaymentDto = paymentDto;  

            var condoFinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.CondominiumId}");   
            
            model.CondominiumFinancialAccountId = condoFinancialAccount.Id;

            return View(model);
        }


        [HttpGet("CreateOneTimePayment")]
        public async Task<ActionResult> CreateOneTimePayment()
        {
            var model = await BuildCreatePaymentViewModel();

            if (model.CondosToSelect == null || !model.CondosToSelect.Any())
                _flashMessage.Danger("You are not managing any condominiums currently");

            return View(model);
        }

        //[HttpGet("CreateOneTimePayment")]
        //// GET: PaymentController/CreateOneTimePayment
        //public async Task<ActionResult> CreateOneTimePayment()
        //{
        //    var model = new CreateOneTimePaymentViewModel();

        //    var condoManagerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");
        //    if (condoManagerCondos != null && !condoManagerCondos.Any())
        //    {
        //        _flashMessage.Danger("You are not managing any condominiums currently");
        //        return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
        //    }

        //    model.CondoMembers = new SelectList(new List<CondoMemberDto>(), "Id", "FullName");

        //    var condosToSelectList = _converterHelper.ToCondosSelectList(condoManagerCondos);
        //    if (condosToSelectList.Any())
        //    {
        //        model.CondosToSelect = condosToSelectList;  
        //    }

        //    var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

        //    if (expenseTypeList.Any())
        //    {
        //        model.ExpenseTypesList = expenseTypeList;
        //    }

        //    //descobrir o role do user

        //    string userRole = string.Empty;

        //    if (User.IsInRole("CondoManager"))
        //    {
        //        userRole = "CondoManager";
        //    }

        //    if (User.IsInRole("CondoMember"))
        //    {
        //        userRole = "CondoMember";
        //    }

        //    model.BeneficiaryTypeList = _paymentHelper.GetBeneficiaryTypesList(userRole);



        //    return View(model);
        //}


        [HttpGet]
        public async Task<IActionResult> GetMembersByCondo(int condoId)
        {

            try
            {
                var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{condoId}");
                if (condo == null || condo.CondoMembers == null)
                {
                    return Json(new List<object>());
                }

                condo.CondoMembers = await _apiCallService.GetAsync<List<CondoMemberDto>>($"api/CondoMembers/ByCondo/{condoId}");

                var members = condo.CondoMembers.Select(m => new
                {
                    id = m.Id,
                    name = m.FullName,
                });

                return Json(members);
            }

            catch (Exception)
            {
                var emptyMember = new List<object>
                {
                    new { id = 0, name = "No members found", financialAccountId = (int?)null }
                };

                return Json(new SelectList(emptyMember, "id", "name"));
            }

            
        }


        // POST: PaymentController/RequestCreateOneTimePaymentCondo
        [HttpPost("RequestCreateOneTimePayment")]
        public async Task<ActionResult> RequestCreateOneTimePayment(CreateOneTimePaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to issue one time payment");
                return View("CreateOneTimePayment", await BuildCreatePaymentViewModel(model));
            }

            if (model.SelectedBeneficiaryId == 3)
            {
                if (string.IsNullOrWhiteSpace(model.Recipient) || string.IsNullOrWhiteSpace(model.ExternalRecipientBankAccount))
                {
                    _flashMessage.Danger("Please enter recipient name and bank account.");
                    return View("CreateOneTimePayment", await BuildCreatePaymentViewModel(model));
                }
            }

            try
            {
                // Se for empresa, preenche automaticamente
                if (model.SelectedBeneficiaryId == 2)
                {
                    var company = await _apiCallService.GetAsync<CompanyDto>("api/Company/GetCompanyByUser");

                    model.BeneficiaryAccountId = company.FinancialAccountId;

                    if (string.IsNullOrWhiteSpace(model.Recipient))
                    {
                        model.Recipient = company.Name;
                    }
                }

                // PREENCHER MODEL (propriedades fora da view)
                var condoManagerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");
                if (condoManagerCondos != null && !condoManagerCondos.Any())
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View("CreateOneTimePayment", await BuildCreatePaymentViewModel());
                }

                var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

                var expenseType = expenseTypeList.FirstOrDefault(e => e.Value == model.ExpenseTypeValue.ToString());
                if (expenseType == null)
                {
                    _flashMessage.Danger("Unable to select this type of expense");
                    return View("CreateOneTimePayment", await BuildCreatePaymentViewModel(model));
                }

                // Criar expense 
                var expenseDto = new ExpenseDto()
                {
                    Amount = model.ExpenseAmount,
                    Detail = model.ExpenseDetail,
                    ExpenseTypeDto = new EnumDto() { Value = model.ExpenseTypeValue.Value, Name = expenseType.Text },
                    CondominiumId = model.CondominiumId.Value,
                };

                int? payerFinancialAccountId = null;
                string? payerName = null;

                if (model.SelectedPayerType == "Condominium")
                {
                    var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{model.CondominiumId}");
                    payerFinancialAccountId = condo?.FinancialAccountId;
                    payerName = condo?.CondoName;
                }
                else if (model.SelectedPayerType == "Member" && model.SelectedCondoMemberId.HasValue)
                {
                    var member = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{model.SelectedCondoMemberId.Value}");
                    payerFinancialAccountId = member?.FinancialAccountId;
                    payerName = member?.FullName;

                    var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{model.CondominiumId}");

                    model.BeneficiaryAccountId = condo.FinancialAccountId;
                    model.Recipient = condo.CondoName;
                }

                if (!payerFinancialAccountId.HasValue)
                {
                    _flashMessage.Danger("Unable to resolve financial account.");
                    return View("CreateOneTimePayment", await BuildCreatePaymentViewModel(model));
                }

                model.PayerFinancialAccountId = payerFinancialAccountId;

                // CONVERTER PARA PAYMENT DTO
                var paymentDto = _converterHelper.FromOneTimeToPaymentDto(model);

                // atribuir propriedades fora do view model
                paymentDto.IssueDate = DateTime.Now;
                paymentDto.Amount = model.ExpenseAmount;
                paymentDto.Payer = payerName;
                paymentDto.ExpenseType = expenseType.Text;
                paymentDto.OneTimeExpenseDto = expenseDto;

                if (model.SelectedBeneficiaryId == 2)
                {
                    paymentDto.BeneficiaryAccountId = model.BeneficiaryAccountId.Value;
                }
                else
                {
                    paymentDto.ExternalRecipientBankAccount = model.ExternalRecipientBankAccount;
                }

                paymentDto.SelectedBeneficiaryId = model.SelectedBeneficiaryId;

                var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/CreateOneTimePayment", paymentDto);

                if (apiCall.IsSuccess && condoManagerCondos != null)
                {
                    var isCondoPayment = condoManagerCondos.Any(c => c.FinancialAccountId == paymentDto.PayerFinancialAccountId);

                    if (isCondoPayment)
                    {
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }

                _flashMessage.Danger("Unable to issue payment");
                return View("CreateOneTimePayment", await BuildCreatePaymentViewModel(model));
            }
            catch
            {
                return View("Error");
            }
        }

        private async Task<CreateOneTimePaymentViewModel> BuildCreatePaymentViewModel(CreateOneTimePaymentViewModel model = null)
        {
            model ??= new CreateOneTimePaymentViewModel();

            model.CondosToSelect = await _paymentHelper.GetCondosToSelectListAsync();
            model.ExpenseTypesList = await _paymentHelper.GetExpenseTypesAsync();
            model.BeneficiaryTypeList = _paymentHelper.GetBeneficiaryTypesList(
                User.IsInRole("CondoManager") ? "CondoManager" :
                User.IsInRole("CondoMember") ? "CondoMember" : "");

            return model;
        }


        //// POST: PaymentController/RequestCreateOneTimePaymentCondo
        //[HttpPost("RequestCreateOneTimePayment")]
        //public async Task<ActionResult> RequestCreateOneTimePayment(CreateOneTimePaymentViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _flashMessage.Danger("Unable to issue one time payment");
        //        return View("CreateOneTimePayment", model); 
        //    }

        //    if (model.SelectedBeneficiaryId == 3)
        //    {
        //        if (string.IsNullOrWhiteSpace(model.Recipient) || string.IsNullOrWhiteSpace(model.ExternalRecipientBankAccount))
        //        {
        //            _flashMessage.Danger("Please enter recipient name and bank account.");
        //            return View("CreateOneTimePayment", model);
        //        }
        //    }
        //    try
        //    {


        //        // Se for empresa, preenche automaticamente
        //        if (model.SelectedBeneficiaryId == 2)
        //        {
        //            var company = await _apiCallService.GetAsync<CompanyDto>("api/Company/GetCompanyByUser");

        //            model.BeneficiaryAccountId = company.FinancialAccountId;

        //            if (string.IsNullOrWhiteSpace(model.Recipient))
        //            {
        //                model.Recipient = company.Name;
        //            }

        //        }


        //            //PREENCHER MODEL (propriedades fora da view)
        //            var condoManagerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");
        //        if (condoManagerCondos != null && !condoManagerCondos.Any())
        //        {
        //            _flashMessage.Danger("You are not managing any condominiums currently");
        //            return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
        //        }

        //        var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

        //        var expenseType = expenseTypeList.FirstOrDefault(e => e.Value == model.ExpenseTypeValue.ToString());
        //        if(expenseType == null)
        //        {
        //            _flashMessage.Danger("Unable to select this type of expense");
        //            return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
        //        }

        //        //criar expense 
        //        var expenseDto = new ExpenseDto()
        //        {
        //            Amount = model.ExpenseAmount,
        //            Detail = model.ExpenseDetail,
        //            ExpenseTypeDto = new EnumDto() { Value = model.ExpenseTypeValue.Value, Name = expenseType.Text },
        //            CondominiumId = model.CondominiumId.Value,
        //        };


        //        int? payerFinancialAccountId = null;
        //        string? payerName = null;

        //        if (model.SelectedPayerType == "Condominium")
        //        {
        //            var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{model.CondominiumId}");
        //            payerFinancialAccountId = condo?.FinancialAccountId;
        //            payerName = condo?.CondoName;



        //        }
        //        else if (model.SelectedPayerType == "Member" && model.SelectedCondoMemberId.HasValue)
        //        {


        //            var member = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{model.SelectedCondoMemberId.Value}");
        //            payerFinancialAccountId = member?.FinancialAccountId;
        //            payerName = member?.FullName;

        //            var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{model.CondominiumId}");

        //            model.BeneficiaryAccountId = condo.FinancialAccountId;
        //            model.Recipient = condo.CondoName;

        //        }

        //        if (!payerFinancialAccountId.HasValue)
        //        {
        //            _flashMessage.Danger("Unable to resolve financial account.");
        //            return View("CreateOneTimePayment", model);
        //        }


        //        model.PayerFinancialAccountId = payerFinancialAccountId;


        //        //CONVERTER PARA PAYMENT DTO
        //        var paymentDto = _converterHelper.FromOneTimeToPaymentDto(model);

        //        //atribur propriedades fora do view model

        //        paymentDto.IssueDate = DateTime.Now;
        //        paymentDto.Amount = model.ExpenseAmount;
        //        paymentDto.Payer = payerName;
        //        paymentDto.ExpenseType = expenseType.Text;

        //        paymentDto.OneTimeExpenseDto = expenseDto;
        //        if (model.SelectedBeneficiaryId == 2)
        //        {
        //            paymentDto.BeneficiaryAccountId = model.BeneficiaryAccountId.Value;

        //        }
        //        else
        //        {
        //          paymentDto.ExternalRecipientBankAccount = model.ExternalRecipientBankAccount;
        //        }

        //            paymentDto.SelectedBeneficiaryId = model.SelectedBeneficiaryId;

        //        var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/CreateOneTimePayment", paymentDto);

        //        if (apiCall.IsSuccess && condoManagerCondos != null)
        //        {
        //            var isCondoPayment = condoManagerCondos.Any(c => c.FinancialAccountId == paymentDto.PayerFinancialAccountId);

        //            if (isCondoPayment)
        //            {
        //                return RedirectToAction(nameof(IndexCondominiumPayments));
        //            }
        //            else
        //            {
        //                return RedirectToAction(nameof(IndexAllCondoMembersPayments));
        //            }

        //        }

        //        _flashMessage.Danger("Unable to issue payment");
        //        return View("CreateOneTimePayment", model);
        //    }
        //    catch
        //    {
        //        return View("Error");
        //    }
        //}


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

            model.Payment = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{id}");

            model.SelectedPaymentMethodId = 0;

                      

            //atribuir id do pagamento
            model.Id = id;   

            return View(model);
        }


        //RequestMakePayment


        // POST: PaymentController/Edit/5
        [HttpPost("RequestMakePayment")]
        public async Task<ActionResult> RequestMakePayment(MakePaymentViewModel model) 
        {

            var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{model.Id}");

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
                if (this.User.IsInRole("CondoMember"))
                {
                    var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={this.User.Identity?.Name}");


                    model.PayerFinancialAccountId = user.FinancialAccountId;
                }
                else if (this.User.IsInRole("CondoManager"))
                {
                                        
                    var condo = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{paymentDto.CondominiumId}");

                    model.PayerFinancialAccountId = condo.FinancialAccountId;
                }

                if (model.PayerFinancialAccountId == 0)
                {
                    ModelState.AddModelError("OmahWalletNumber", "Omah wallet number is required");
                }

            }
            else if (model.SelectedPaymentMethodId == 0)
            {
                ModelState.AddModelError("SelectedPaymentMethodId", "You must select a payment method");
            }

            ////validação manual para account id ou external bank account
            //if (model.SelectedBeneficiaryId != 3)
            //{
            //    if (model.BeneficiaryAccountId == null)
            //    {
            //        ModelState.AddModelError("BeneficiaryAccountId", "Beneficiary Account Id is required");
            //    }

            //}
            //else if (model.SelectedBeneficiaryId == 0)
            //{
            //    ModelState.AddModelError("SelectedBeneficiaryId", "You must select a recipient type");
            //}
            //else
            //{
            //    if (model.ExternalRecipientBankAccount == null)
            //    {
            //        ModelState.AddModelError("ExternalRecipientBankAccount", "Recipient's bank account is required");
            //    }
            //}


            if (!ModelState.IsValid)
            {
                return View("MakePayment", model);
            }

            try
            {

                //buscar metodo de pagamento

                var paymentMethod = _paymentHelper.GetSelectedPaymentMethod(model.SelectedPaymentMethodId);

                if (paymentMethod == string.Empty)
                {
                    _flashMessage.Danger("Unable to select payment method");
                    return View("MakePayment", model);
                }

                paymentDto.PaymentMethod = paymentMethod;

                //Caso pagamento seja feito com cartão de crédito
                if (paymentMethod == "Credit card")
                {
                    paymentDto.CreditCard = model.CreditCardNumber;
                }

                //caso pagamento seja feito com mbway
                if(paymentMethod == "MbWay")
                {
                    paymentDto.MbwayNumber = model.PhoneNumber;
                }

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
                if (paymentDto.SelectedBeneficiaryId == 3)
                {
                    paymentDto.BeneficiaryAccountId = null;  
                }
                else //caso seja pagamento interno ver se a account do beneficiary está válida
                {
                    var financialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{paymentDto.BeneficiaryAccountId}");

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
                        RecipientName = paymentDto.Recipient,
                        PayerAccountId = paymentDto.PayerFinancialAccountId,
                        BeneficiaryAccountId = paymentDto.BeneficiaryAccountId,
                        ExternalRecipientBankAccount = paymentDto.ExternalRecipientBankAccount,
                        Amount = paymentDto.TotalAmount
                    };

                //subtrair de account caso method payment seja omahWallet

                if(model.SelectedPaymentMethodId== 3)
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

           
                if (paymentDto.SelectedBeneficiaryId != 3) // se não for externo
                {
            

                    var incomeDto = new CashFlowDto()
                    {
                        TotalAmount = paymentDto.TotalAmount,
                        AccountId = paymentDto.BeneficiaryAccountId.Value,
                    };

                    if (paymentDto.BeneficiaryAccountId != 0)
                    {
                        var incomePay = await _apiCallService.PostAsync<CashFlowDto, Response<object>>("api/Payment/IncomePayment", incomeDto);

                        if (!incomePay.IsSuccess)
                        {
                            _flashMessage.Danger(incomePay.Message);
                            return View("MakePayment", model);
                        }
                    }





                }
               

                //Fazer requisição para fazer pagamento

                var apiCall = await _apiCallService.PostAsync<PaymentDto, Response<object>>("api/Payment/MakePayment", paymentDto); 

                if(apiCall.IsSuccess)
                {
                    var paidPayment = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{paymentDto.Id}");

                    
                    return RedirectToAction("InvoiceDetails", "Invoice", new { id = paidPayment.InvoiceId });
                }
                else
                {
                    _flashMessage.Danger(apiCall.Message);
                    return View("MakePayment", model);
                }
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
            List<CondominiumDto> condoManagerCondos = null;

            PaymentDto paymentDto = null;

            try
            {
                condoManagerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");

              

                paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{id}");

                var isCondoPayment = condoManagerCondos.Any(c => c.FinancialAccountId == paymentDto.PayerFinancialAccountId);

                var apiCall = await _apiCallService.DeleteAsync($"api/Payment/Delete/{id}");

                if(apiCall.IsSuccessStatusCode)
                {
                    if (isCondoPayment)
                    {
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }
                else
                {
                    if (isCondoPayment)
                    {
                        _flashMessage.Danger("Unable to process cancellation due to server error");
                        return RedirectToAction(nameof(IndexCondominiumPayments));
                    }
                    else
                    {
                        _flashMessage.Danger("Unable to process cancellation due to server error");
                        return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                    }
                }
                

            }
            catch(System.Net.Http.HttpRequestException ex)
            {


                if(condoManagerCondos != null && paymentDto != null)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        condoManagerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");
                        var isCondoPayment = condoManagerCondos.Any(c => c.FinancialAccountId == paymentDto.PayerFinancialAccountId);

                        if (isCondoPayment)
                        {
                            _flashMessage.Danger("Payment is already paid, unable to process cancellation");
                            return RedirectToAction(nameof(IndexCondominiumPayments));
                        }
                        else
                        {
                            _flashMessage.Danger("Payment is already paid, unable to process cancellation");
                            return RedirectToAction(nameof(IndexAllCondoMembersPayments));
                        }
                    }
                }

                return View("Error");
            }
        }


    }
}
