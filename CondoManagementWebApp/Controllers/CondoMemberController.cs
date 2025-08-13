//using ClassLibrary.DtoModels;
//using CondoManagementWebApp.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace CondoManagementWebApp.Controllers
//{
//    public class CondoMemberController : Controller
//    {
//        private readonly ICondoMemberHelper _condoMemberHelper;
//        private readonly IConverterHelper _converterHelper;
//        private readonly IUserHelper _userHelper;

//        public CondoMemberController(ICondoMemberHelper condoMemberHelper, IConverterHelper converterHelper, IUserHelper userHelper)
//        {
//            _condoMemberHelper = condoMemberHelper;
//            _converterHelper = converterHelper;
//            _userHelper = userHelper;
//        }


//        // GET: CondoMemberController
//        public async Task<ActionResult> Index()
//        {
//            var condoMembers = await  _condoMemberHelper.GetAllAsync("api/CondoMembers");

//            //var condoMembers = await _apiServiceCallAccount.GetAsync<IEnumerable<CondoMember>>("api/CondoMembers");

//            return View(condoMembers);
//        } 

//        // GET: CondoMemberController/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {

//            if (id == null)
//            {
//                return NotFound();
//            }

//            var condoMember = await _condoMemberHelper.GetByIdAsync($"api/CondoMembers/{id.Value}");

//            //var condoMembers = await _apiServiceCallAccount.GetAsync<CondoMember>("api/CondoMembers/{id.Value}");

//            return View(condoMember);
//        }

//        // GET: CondoMemberController/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: CondoMemberController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create(CondoMemberDto condoMemberDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(condoMemberDto);
//            }

//            try
//            {
//                var registerUserDto = _converterHelper.ToRegisterDto(condoMemberDto);

//                registerUserDto.SelectedRole = "CondoMember"; 

//                var success2 = await _userHelper.CreateAsync("api/Account/AssociateUser", registerUserDto);

//                //var apiCall = await _apiCallServiceAccount.PostAsync<RegisterUserDto, Response>("api/Account/AssociateUser", registerDto);

//                if (!success2)
//                {
//                    ModelState.AddModelError("", "Failed to create user. Please try again.");
//                    return View(condoMemberDto);
//                }
 

//                var success = await _condoMemberHelper.CreateAsync("api/CondoMembers", condoMemberDto);



//                if (!success)
//                {
//                    ModelState.AddModelError("", "Failed to create condo member. Please try again.");
//                    return View(condoMemberDto);
//                }

                

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
//                return View(condoMemberDto);
//            }

//        }

//        // GET: CondoMemberController/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {

//            if (id == null)
//            {
//                return  NotFound();
//            }

//            var condoMember = await _condoMemberHelper.GetByIdAsync($"api/CondoMembers/{id.Value}");
//            if (condoMember == null)
//            {
//                return NotFound();
//            }


//            return View(condoMember);
//        }

//        // POST: CondoMemberController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, CondoMemberDto condoMemberDto)
//        {

//            if (id != condoMemberDto.Id)
//            {
//                return NotFound();
//            }

//            if (!ModelState.IsValid)
//            {
//                return View(condoMemberDto);
//            }

//            try
//            {

//               var success = await _condoMemberHelper.EditAsync($"api/CondoMembers/Edit/{id}", condoMemberDto);
//               if (!success)
//               {
//                   ModelState.AddModelError("", "Failed to edit condo member. Please try again.");
//                   return View(condoMemberDto);
//               }

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
//                return View(condoMemberDto);
//            }
//        }

//        // GET: CondoMemberController/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var condoMember = await _condoMemberHelper.GetByIdAsync($"api/CondoMembers/{id.Value}"); ;
//            if (condoMember == null)
//            {
//                return NotFound();
//            }


//            return View(condoMember);
//        }

//        // POST: CondoMemberController/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            try
//            {                            

//                var condoMember = await _condoMemberHelper.GetByIdAsync($"api/CondoMembers/{id.Value}");
//                if (condoMember == null)
//                {
//                    return NotFound();
//                }

//                var success = await _condoMemberHelper.DeleteAsync($"api/CondoMembers/{id.Value}");
//                if (!success)
//                {
//                    ModelState.AddModelError("", "Failed to delete condo member. Please try again.");
//                    return View(condoMember);
//                }

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
//                return View(id);
//            }
//        }
//    }
//}
