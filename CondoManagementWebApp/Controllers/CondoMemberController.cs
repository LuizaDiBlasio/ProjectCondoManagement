using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class CondoMemberController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public CondoMemberController(IApiCallService apiCallService, IConverterHelper converterHelper, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }


        // GET: CondoMemberController
        public async Task<ActionResult> Index()
        {
            try
            {
                var condoMembers = await _apiCallService.GetAsync<IEnumerable<CondoMemberDto>>("api/CondoMembers")?? new List<CondoMemberDto>();
                return View(condoMembers);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching condo members.");
                return View(new List<CondoMemberDto>());
            }


        }


        public async Task<ActionResult> MembersFromCondo (int condoId, string? condoName)
        {
            if(condoName != null)
            {
                ViewBag.CondoName = condoName;
            }

            try
            {
                var condoMembers = await _apiCallService.GetAsync<IEnumerable<CondoMemberDto>>($"api/CondoMembers/ByCondo/{condoId}") ?? new List<CondoMemberDto>();
                return View(condoMembers);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching condo members for the specified condominium.");
                return View(new List<CondoMemberDto>());
            }
        }


        public async Task<ActionResult<CondoMemberDashboardViewModel>> Dashboard()
        {
            var email = User.Identity?.Name;

            var model = new CondoMemberDashboardViewModel();

            var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{email}");
            var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");

            model.CondoMemberDto = condoMember;

            model.UnitDtos = condoMember?.Units?.ToList() ?? new List<UnitDto>();

            var distinctCondos = model.UnitDtos
            .Where(u => u.CondominiumDto != null)
            .Select(u => u.CondominiumDto)
            .DistinctBy(c => c.Id)   
            .ToList();

            model.OccurrenceDtos = distinctCondos
                .Where(c => c.Occurrences != null)                
                .SelectMany(c => c.Occurrences.Where(o => o.IsResolved == false))
                .ToList();


            model.FinancialAccountDto = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{user.FinancialAccountId}");

            var unfilteredPayments = await _apiCallService.GetAsync<List<PaymentDto>>($"api/Payment/GetPaymentsByFinancialAccount?financialAccountId={model.FinancialAccountDto.Id}") ?? new List<PaymentDto>();

            model.PaymentsDtos = unfilteredPayments.Where(p => p.IsPaid == false).ToList();

            model.MeetingsDtos = distinctCondos.SelectMany(c => c.Meetings ?? new List<MeetingDto>()).ToList();

            model.MessageDtos = await _apiCallService.GetAsync<List<MessageDto>>($"api/Message/Received/{email}") ?? new List<MessageDto>();

          

            return View(model);

        }



        // GET: CondoMemberController/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return View("Error404");
            }

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");

                return View(condoMember);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching the condo member.");
                return View(new CondoMemberDto());
            }
        }

        // GET: CondoMemberController/Create
        public async Task<ActionResult> Create()
        {
            var email = User.Identity?.Name;
            var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");


            return View();
        }

        // POST: CondoMemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CondoMemberDto condoMemberDto)
        {

            var email = User.Identity?.Name;
            var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");

            if (!ModelState.IsValid)
            {
                return View(condoMemberDto);
            }

            if (condoMemberDto.BirthDate > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Birth date cannot be in the future.");


                return View(condoMemberDto);
            }
            

            try
            {

                bool checkEmailExists = await _apiCallService.GetAsync<bool>($"api/CondoMembers/Exists?email={condoMemberDto.Email}");
                if (checkEmailExists)
                {
                    ModelState.AddModelError("Email", "Email already in use!");

                    return View(condoMemberDto);
                }

                var result = await _apiCallService.PostAsync<CondoMemberDto, Response<object>>("api/CondoMembers", condoMemberDto);
                if (!result.IsSuccess)
                {
                    _flashMessage.Danger(result.Message);

                    return View(condoMemberDto);
                }

                if (result.IsSuccess)
                {
                    var registerUserDto = _converterHelper.ToRegisterDto(condoMemberDto);

                    registerUserDto.SelectedRole = "CondoMember";

                    if(registerUserDto.Companies == null)
                    {
                        registerUserDto.Companies = new List<CompanyDto>();
                    }

                    registerUserDto.Companies.Add(user.CompaniesDto.FirstOrDefault());                    

                    var result2 = await _apiCallService.PostAsync<RegisterUserDto, Response<object>>("api/Account/AssociateUser", registerUserDto);
                    

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                var createdMemberExists = await _apiCallService.GetAsync<bool>($"api/CondoMembers/Exists?email={condoMemberDto.Email}");
                if (!createdMemberExists)
                {
                    return NotFound("Condo member not found after creation.");
                }
                var createdMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{condoMemberDto.Email}");

                await _apiCallService.DeleteAsync($"api/CondoMembers/{createdMember.Id}");

                _flashMessage.Danger($"An error occurred while creating the condo member.");

                return View(condoMemberDto);
            }


            _flashMessage.Danger($"An error occurred while creating the condo member.");

            return View(condoMemberDto);

        }



        // GET: CondoMemberController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return View("Error404");
            }

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                return View(condoMember);
            }
            catch (Exception)
            {

               _flashMessage.Danger($"An error occurred while fetching the condo member for editing.");
               return View(new CondoMemberDto());
            }

  
        }

        // POST: CondoMemberController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CondoMemberDto condoMemberDto)
        {

            if (id != condoMemberDto.Id)
            {
                return View("Error404");
            }

            if (!ModelState.IsValid)
            {
                return View(condoMemberDto);
            }

            var result = await _apiCallService.PostAsync<CondoMemberDto, Response<object>>($"api/CondoMembers/Edit/{id}", condoMemberDto);
            if (result == null)
            {
                _flashMessage.Danger($"An error occurred while updating the condo member.");
                return View(condoMemberDto);
            }
            if (!result.IsSuccess)
            {
                _flashMessage.Danger($"An error occurred while updating the condo member.");
                return View(condoMemberDto);
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: CondoMemberController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                if (condoMember == null)
                {
                    return View("Error404");
                }

                return View(condoMember);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching the condo member for deletion.");
                return RedirectToAction(nameof(Index));
            }            
        }

        // POST: CondoMemberController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return View("Error404");
            }

            try
            {

                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                if (condoMember == null)
                {
                    return View("Error404");
                }

                var result = await _apiCallService.DeleteAsync($"api/CondoMembers/{id.Value}");
                if (!result.IsSuccessStatusCode)
                {
                    _flashMessage.Danger("Failed to delete condo member. Please try again.");
                    return View(condoMember);
                }


                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                _flashMessage.Danger("An unexpected error occurred while deleting the condo member.");
                return RedirectToAction(nameof(Index));
            }
        }





     


    }
}