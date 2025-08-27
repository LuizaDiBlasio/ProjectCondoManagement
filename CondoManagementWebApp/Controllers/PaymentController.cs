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


        public PaymentController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper   )
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }


        // GET: PaymentController
        public async Task<ActionResult<List<PaymentDto>>> IndexCondominiumPayments()
        {
            try
            {
                //achar o condominio do condoManager
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<PaymentDto>());
                }

                var condoPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("GetCondoPayments", $"{condoManagerCondo.Id}");

                return View(condoPayments);

            }
            catch
            {
                return View("Error");
            }
        }


        // GET: PaymentController
        public async Task<ActionResult<List<PaymentDto>>> IndexAllCondoMembersPayments()
        {
            try
            {
                //achar o condominio do condoManager
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View(new List<PaymentDto>());
                }

                var allCondoMemeberPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("GetAllCondoMemberPayments", $"{condoManagerCondo.Id}");

                return View(allCondoMemeberPayments);
            }
            catch
            {
                return View("Error");
            }
        }


        // GET: PaymentController
        public async Task<ActionResult<List<PaymentDto>>> IndexCondoMemberPayments()
        {
            try
            {
                var condoMemberPayments = await _apiCallService.GetByQueryAsync<List<PaymentDto>>("GetCondoMemberPayments", this.User.Identity.Name);

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

        // GET: PaymentController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //puxar a list de expenses
            var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"GetPaymentDetails/{id}");
            if(paymentDto == null)
            {
                _flashMessage.Danger("An error has occurred: payment not found");
                return View(new PaymentDto());  
            }



            return View(paymentDto);
        }

        // GET: PaymentController/CreateRecurringPayment
        public async Task<ActionResult> CreateRecurringPayment() 
        {
            var model = new CreateRecurringPaymentViewModel();

            var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);
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

            return View(model);
        }

        // GET: PaymentController/CreateOneTimePayment
        public async Task<ActionResult> CreateOneTimePayment()
        {
            var model = new CreateOneTimePaymentViewModel();

            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            if (expenseTypeList.Any())
            {
                model.ExpenseTypesList = expenseTypeList;
            }

            return View(model);
        }

        // POST: PaymentController/RequestCreateOneTimePaymentCondo
        [HttpPost]
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
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View("CreateOneTimePayment", new CreateOneTimePaymentViewModel());
                }

                model.CondominiumId = condoManagerCondo.Id;

              
                    //criar expense 
                    var expenseDto = new ExpenseDto()
                    {
                        Amount = model.ExpenseAmount,
                        Detail = model.ExpenseDetail,
                        ExpenseTypeDto = model.ExpenseTypeDto,
                    };

                model.OneTimeExpense = expenseDto;

                //CONVERTER PARA PAYMENT DTO
                var paymentDto = _converterHelper.FromOneTimeToPaymentDto(model);

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
        [HttpPost] 
        public async Task<ActionResult> RequestCreateRecurringPayment(CreateRecurringPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to issue one time payment");
                return View("CreateOneTimePayment", model);
            }

            try
            {
                //PREENCHER MODEL (propriedades fora da view)
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condoManagerCondo == null)
                {
                    _flashMessage.Danger("You are not managing any condominiums currently");
                    return View("CreateRecurringPayment", new CreateRecurringPaymentViewModel());
                }

                model.CondominiumId = condoManagerCondo.Id;

                //CONVERTER PARA PAYMENT DTO
                var paymentDto = _converterHelper.FromRecurringToPaymentDto(model);

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
        [HttpGet]
        public async Task<ActionResult> MakePayment()
        {
            var model = new MakePaymentViewModel();

            var paymentMethodList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Payment/GetPaymentMethodsList");

            if (paymentMethodList.Any())
            {
                model.PaymentMethods = paymentMethodList;
            }

            model.SelectedPaymentMethodId = 0;

            return View(model);
        }


        //RequestMakePayment


        // POST: PaymentController/Edit/5
        [HttpPost]
        public async Task<ActionResult> RequestMakePayment(MakePaymentViewModel model) //ID HIDDEN
        {
            try
            {
                //buscar metodo de pagamento
                var paymentDto = await _apiCallService.GetAsync<PaymentDto>($"api/Payment/GetPaymentDetails/{model.Id}");

                var paymentMethod = await _apiCallService.GetAsync<string>($"api/Payment/GetSelectedPaymentMethod/{model.SelectedPaymentMethodId}");

                if (paymentMethod == string.Empty)
                {
                    _flashMessage.Danger("Unable to select payment method");
                    return View("MakePayment", model);
                }

                paymentDto.PaymentMethod = paymentMethod;
                paymentDto.IsPaid = true;

                //criar transaction
                paymentDto.TransactionDto = new TransactionDto() //o id vai ser zero por enquanto 
                {
                    PaymentId = paymentDto.Id,
                    DateAndTime = DateTime.Now,
                    PayerAccountId = paymentDto.PayerFinancialAccountId,
                    BeneficiaryAccountId = model.BeneficiaryAccountId
                };

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestDelete(int id)
        {
            try
            {
                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("GetCondoManagerCondominiumDto", this.User.Identity.Name);

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
