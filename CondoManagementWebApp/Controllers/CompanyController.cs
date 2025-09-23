using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using System.Net;
using Vereyon.Web;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace CondoManagementWebApp.Controllers
{
    public class CompanyController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

        public CompanyController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }

        // GET: Company/IndexCompanies
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> IndexCompanies()
        {
            try
            {
                var companies = await _apiCallService.GetAsync<IEnumerable<CompanyDto>>("api/Company/GetCompanies");

                var model = new IndexCompaniesViewModel()
                {
                    Companies = companies
                };

                return View(model);
            }
            catch (Exception)
            {
                return View("Error500");
            }
        }

        // GET: Company/Details/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(int id)
        {
            try
            {

                var model = new CompanyDetailsViewModel();

                var companyDto = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{id}");

                if (companyDto == null)
                {
                    return View("NotFound");
                }

                model.CompanyDto = companyDto;

                if (companyDto.CompanyAdminId != null)
                {
                    var companyAdmin = await _apiCallService.GetAsync<UserDto>($"api/Company/GetCompanyAdmin/{companyDto.CompanyAdminId}");

                    model.CompanyAdmin = companyAdmin;
                }

                if (companyDto.CondominiumDtos != null)
                {
                    model.CondominiumDtos = companyDto.CondominiumDtos;
                }

                return View(model);
            }
            catch
            {
                return View("Error500");
            }

        }


        // GET: Company/Create
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Create()
        {
                var model = new CreateEditCompanyViewModel();
               
                return View(model);
            
        }


        // POST: Company/RequestCreateCompany
        [Microsoft.AspNetCore.Mvc.HttpPost("RequestCreateCompany")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestCreateCompany(CreateEditCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to add company due to unexpected error");
                return View("Create", model);
            }
            try
            {
                var companyDto = _converterHelper.ToCompanyDto(model);

                var apiCall = await _apiCallService.PostAsync<CompanyDto, Response<object>>("api/Company/PostCompany", companyDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexCompanies));
                }

                _flashMessage.Danger(apiCall.Message);

                return View("Create", model);

            }
            catch
            {
                return View("Error500");
            }
        }




        // GET: Company/Edit/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id)
        {
            try
            {
                var companyDto = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{id}");

                if (companyDto == null)
                {
                    return View("NotFound");
                }

                var model = _converterHelper.ToCreateEditCompanyViewModel(companyDto);

                return View(model);
            }
            catch
            {
                return View("Error500");
            }
        }


        // POST: Company/RequestEdit/5
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestEdit(CreateEditCompanyViewModel model)
        {

            //TODO: o reload das selectLists nao esta funcionar , veem vazias


            if (!ModelState.IsValid)
            {
                
                return View("Edit", model);
            }

            var companyDto = _converterHelper.ToCompanyDto(model);

            try
            {
                var apiCall = await _apiCallService.PostAsync<CompanyDto, Response<object>>($"api/Company/EditCompany", companyDto);

                if (!apiCall.IsSuccess)
                {
                    _flashMessage.Danger(apiCall.Message);

                    return View("Edit", model);
                }



                _flashMessage.Confirmation(apiCall.Message);

                return RedirectToAction(nameof(IndexCompanies));
            }
            catch
            {
                return View("Edit", model);
            }
        }



        // POST: Company/RequestDelete/5
        [Microsoft.AspNetCore.Mvc.HttpDelete("Company/RequestDelete/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestDelete(int id)
        {
            try
            {
                var apiCall = await _apiCallService.DeleteAsync($"api/Company/DeleteCompany/{id}");

                if (apiCall.IsSuccessStatusCode)
                {
                    // Retorna um JSON para o AJAX indicar sucesso
                    return Json(new { success = true });
                }

                var responseBody = await apiCall.Content.ReadFromJsonAsync<Response<object>>();

                // Retorna o JSON com a mensagem de erro da API
                return Json(new { success = false, message = responseBody?.Message ?? "Unable to delete company due to error" });

            }
            catch (HttpRequestException ex) //buscar o tipo de exceção para ver se tem conflito
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    return Json(new { success = false, message = "Unable to delete company due to conflict, company associated with users or condos" });
                }
                else
                {

                    return Json(new { success = false, message = "An HTTP error occurred. Please try again later." });
                }
            }
            catch (Exception)
            {
                // Para outros tipos de exceções inesperadas
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
