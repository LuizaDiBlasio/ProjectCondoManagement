using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Users;

namespace ProjectCondoManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly DataContextUsers _contextUsers;


        public CompanyController(ICompanyRepository companyRepository, DataContextUsers contextUsers    )
        {
            _companyRepository = companyRepository;
            _contextUsers = contextUsers;
        }

        // GET: CompanyController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: CompanyController/Details/5
        [HttpGet("Details/{id}")]
        public async Task<Company?> Details(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id, _contextUsers);

            if (company == null)
            {
                return null;
            }

            return company; 
        }

        // GET: CompanyController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: CompanyController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: CompanyController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: CompanyController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: CompanyController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: CompanyController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
