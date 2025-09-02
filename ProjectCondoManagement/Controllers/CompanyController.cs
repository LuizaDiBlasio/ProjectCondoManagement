using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Users;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextUsers _context;

        public CompanyController(ICompanyRepository companyRepository,DataContextUsers context,IConverterHelper converterHelper)
        {
            _companyRepository = companyRepository;
            _converterHelper = converterHelper;
        }

        // GET api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> Get(int id)
        {
            var company = await  _companyRepository.GetByIdAsync(id,_context);

            if (company == null)
            {
                return NotFound();
            }

            var companyDto = _converterHelper.ToCompanyDto(company);

            return Ok(companyDto);
        }
    }
}
