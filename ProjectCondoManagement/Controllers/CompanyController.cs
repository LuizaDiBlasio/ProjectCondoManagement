using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Data.Repositories.Users;
using ProjectCondoManagement.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProjectCondoManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly DataContextUsers _contextUsers;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextCondos _contextCondos;
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly DataContextFinances _contextFinances;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository; 

        public CompanyController(ICompanyRepository companyRepository, DataContextUsers contextUsers, IConverterHelper converterHelper, 
            DataContextCondos contextCondos, IFinancialAccountRepository financialAccountRepository, DataContextFinances dataContextFinances,
            IUserHelper userHelper, ICondominiumRepository condominiumRepository)
        {
            _companyRepository = companyRepository;
            _contextUsers = contextUsers;
            _converterHelper = converterHelper;
            _contextCondos = contextCondos;
            _financialAccountRepository = financialAccountRepository;
            _contextFinances = dataContextFinances;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
        }

        /// <summary>
         /// Retrieves a list of all companies.
         /// </summary>
         /// <returns>A collection of CompanyDto objects representing all companies.</returns>
        //GET: CompanyController
        [HttpGet("GetCompanies")]
        public async Task<ActionResult<List<CompanyDto>>> GetCompanies()
        {
           var companies = _companyRepository.GetAll(_contextUsers).Include(c => c.Users).ToList();

            var companiesDtos = companies.Select(c => _converterHelper.ToCompanyDto(c, true)).ToList();

            return companiesDtos;
        }


        /// <summary>
         /// Retrieves the details of a specific company by its ID.
         /// </summary>
         /// <param name="id">The unique identifier of the company.</param>
         /// <returns>A CompanyDto object if the company is found; otherwise, returns null.</returns>
        // GET: CompanyController/GetCompany/5
        [HttpGet("GetCompany/{id}")]
        public async Task<CompanyDto?> GetCompany(int id)
        {
            var company = await _companyRepository.GetCompanyWithCondosAndUsers(id);

            if (company == null)
            {
                return null;
            }

            var companyDto = _converterHelper.ToCompanyDto(company, true);

            return companyDto;
        }


        /// <summary>
         /// Creates a new company based on the provided data.
         /// </summary>
         /// <param name="companyDto">The data transfer object containing the company details.</param>
         /// <returns>
         /// An IActionResult indicating the result of the operation.
         /// Returns Ok on success, BadRequest for invalid input, or a 500 Internal Server Error on an unexpected exception.
         /// </returns>
        // POST: CompanyController/PostCompany
        [HttpPost("PostCompany")]
        public async Task<IActionResult> PostCompany([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest(new   Response<object> { IsSuccess = false, Message = "System error" });
            }


            try
            {
               
                //converter para company

                var company = _converterHelper.ToCompany(companyDto, true, true);

                if (company == null)
                {
                    return BadRequest(new Response<object> { IsSuccess = false, Message = "Conversion failed. Invalid data." });
                }

                if (await _companyRepository.ExistingCompany(company))
                {
                    return Conflict(new Response<object> { IsSuccess = false, Message ="Company already registered"});
                }


                //criar financial account 

                var financialAccount = new FinancialAccount()
                {
                    OwnerName = company.Name,
                    Balance = 0 // depósito inicial vai ser sempre 0
                };

                await _financialAccountRepository.CreateAsync(financialAccount, _contextFinances);


                //Atribuir financial account

                company.FinancialAccountId = financialAccount.Id;

                await _companyRepository.CreateAsync(company, _contextUsers);

                return Ok(new Response<object> { IsSuccess = true});
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {errorMessage}");
            }
        }


        /// <summary>
        /// Updates a company based on the provided data.
        /// </summary>
        /// <param name="companyDto">The data transfer object containing the company details.</param>
        /// <returns> Returns Ok on success, BadRequest for invalid input, or a 500 Internal Server Error on an unexpected exception.</returns>
        [HttpPost("EditCompany")]
        public async Task<IActionResult> EditCompany([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request body is null.");
            }


            try
            {
                var company = await _companyRepository.GetByIdAsync(companyDto.Id, _contextUsers);

                if (company == null)
                {
                    return Ok(new Response<object> { IsSuccess = false, Message = "Unable to edit company, record not found." });
                }

                //atualizar propriedades
                company.Name = companyDto.Name; 
                company.Email = companyDto.Email;
                company.PhoneNumber = companyDto.PhoneNumber;
                company.TaxIdDocument = companyDto.TaxIdDocument;
                company.Address = companyDto.Address;   

                await _companyRepository.UpdateAsync(company, _contextUsers);

                return Ok(new Response<object> { IsSuccess = true, Message = "Company details updated successfully!" });
            }
            catch (Exception)
            {
                return Ok(new Response<object> { IsSuccess = false, Message = "Unable to edit company due to internal error" });
            }
        }


      

        /// <summary>
         /// Deletes a company by its unique identifier.
         /// </summary>
         /// <param name="id">The unique identifier of the company to delete.</param>
         /// <returns>
         /// An IActionResult indicating the result of the operation.
         /// Returns Ok on successful deletion, NotFound if the company does not exist, or BadRequest on an error.
         /// </returns>
        // DELETE: CompanyController/DeleteCompany/5
        [HttpDelete("DeleteCompany/{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyWithCondosAndUsers(id);

                if (company == null)
                {
                    return NotFound();
                }

                if (!company.Condominiums.Any())
                {
                    //desfazer relação com admin
                    var admin = await _userHelper.GetUserByIdAsync(company.CompanyAdminId);
                    if (admin != null)
                    {
                        company.CompanyAdminId = null;
                    }

                    await _companyRepository.DeleteAsync(company, _contextUsers);
                    return Ok();
                }

                return Conflict();
                
                  
            }
            catch
            {
                return BadRequest();
            }

        }
  

        //Metodos auxiliares

        [HttpGet("LoadAdmins")]
        public async Task<IActionResult> LoadAdmins()
        {
            var adminsList = await _companyRepository.GetCompanyAdminsSelectListAsync();

            return Ok(adminsList);
        }


        [HttpGet("LoadAdminsToEdit/{id}")]
        public async Task<IActionResult> LoadAdminsToEdit(int id)
        {
            var adminsList = await _companyRepository.GetCompanyAdminsSelectListToEdit(id);

            return Ok(adminsList);
        }




        [HttpGet("GetCompanyAdmin/{id}")]
        public async Task<IActionResult> GetCompanyAdmin(string id)
        {
            var companyAdminUser = await _userHelper.GetUserByIdAsync(id);

            if (companyAdminUser == null)
            {
                return Ok(null);  
            }

            var companyAdminUserDto = _converterHelper.ToUserDto(companyAdminUser, true);

            return Ok(companyAdminUserDto);
        }

        [HttpGet("GetCompanyByUser")]
        public async Task<ActionResult<CompanyDto?>> GetCompanyByUser([FromQuery] bool includeCondominiums = false)
        {
            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(this.User.Identity.Name);

         
            var companiesDto = user.Companies?.Select(c => _converterHelper.ToCompanyDto(c, false)).ToList();

            if (includeCondominiums && companiesDto != null)
            {
                foreach (var companyDto in companiesDto)
                {
                    if (companyDto != null)
                    {
                        companyDto.CondominiumDtos = await _condominiumRepository.GetCondominiumsByCompanyIdAsync(companyDto.Id);
                    }
                }
            }

            return Ok(companiesDto);
        }


        [HttpPost("GetCompanyCondominiums")]

        public async Task<IActionResult> GetCompanyCondominiums([FromBody] CompanyDto companyDto)
        {
            var companyCondominiums = await _condominiumRepository.GetCompanyCondominiums(companyDto.SelectedCondominiumIds);

            var companyCondominiumsDtos = companyCondominiums?.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList() ?? new List<CondominiumDto>();


            return Ok(companyCondominiumsDtos); 
        }
    }
}
