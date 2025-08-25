using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Data.Repositories.Users;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly DataContextUsers _contextUsers;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextCondos _contextCondos;
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly DataContextFinances _contextFinances;
        private readonly IUserHelper _userHelper;

        public CompanyController(ICompanyRepository companyRepository, DataContextUsers contextUsers, IConverterHelper converterHelper, 
            DataContextCondos contextCondos, IFinancialAccountRepository financialAccountRepository, DataContextFinances dataContextFinances,
            IUserHelper userHelper)
        {
            _companyRepository = companyRepository;
            _contextUsers = contextUsers;
            _converterHelper = converterHelper;
            _contextCondos = contextCondos;
            _financialAccountRepository = financialAccountRepository;
            _contextFinances = dataContextFinances;
            _userHelper = userHelper;
        }

        /// <summary>
         /// Retrieves a list of all companies.
         /// </summary>
         /// <returns>A collection of CompanyDto objects representing all companies.</returns>
        //GET: CompanyController
        [HttpGet("GetCompanies")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
        {
           var companies = _companyRepository.GetAll(_contextUsers).ToList();

            var companiesDtos = companies.Select(c => _converterHelper.ToCompanyDto(c)).ToList();

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
            var company = await _companyRepository.GetCompanyWithcCondosAndAdmin(id, _contextUsers);

            if (company == null)
            {
                return null;
            }

            var companyDto = _converterHelper.ToCompanyDto(company);

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
                return BadRequest(new Response<object> { IsSuccess = false, Message = "System error" });
            }


            try
            {
                if (companyDto.CompanyAdminId != null)
                {
                    var companyAdminUserDto = _converterHelper.ToUserDto(await _userHelper.GetUserByIdAsync(companyDto.CompanyAdminId));

                    if (companyAdminUserDto == null)
                    {
                        return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to assing company admin, user not found" });
                    }

                    //atribuir admin à company dto

                    companyDto.CompanyAdmin = companyAdminUserDto;  
                }

                //converter para company

                var company = _converterHelper.ToCompany(companyDto, true);

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
                    InitialDeposit = 0 // depósito inicial vai ser sempre 0
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

                //buscar entidades selecionadas com base nos ids selecionados
                var selectedAdminAdnCondosDto = await _companyRepository.SelectedAdminAndCondos(companyDto);

                companyDto.CondominiumDtos = selectedAdminAdnCondosDto.SelectedCondos;
                companyDto.CompanyAdmin = selectedAdminAdnCondosDto.SelectedAdmin;

                var company = _converterHelper.ToCompany(companyDto, false);

                if (company == null)
                {
                    return BadRequest(new Response<object> { IsSuccess = false, Message = "Unable to edit company, record not found." });
                }

                await _companyRepository.UpdateAsync(company, _contextUsers);

                return Ok(new Response<object> { IsSuccess = true, Message = "Company details updated successfully!" });
            }
            catch (Exception)
            {
                return BadRequest(new Response<object> { IsSuccess = false, Message = "Unable to edit company due to internal error" });
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
            var company = await _companyRepository.GetCompanyWithcCondosAndAdmin(id, _contextUsers);

            if (company == null)
            {
                return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to delete company, record not found." });
            }

            try
            {
                
                if (!company.Condominiums.Any() && company.CompanyAdmin == null)
                {
                    await _companyRepository.DeleteAsync(company, _contextUsers);
                    return Ok();
                }

                return Conflict(new Response<object> { IsSuccess = false, Message = "Unable to delete company due to conflict: company still associated with users or condominiuns." });
                
                  
            }
            catch
            {
                return BadRequest();
            }

        }


        [HttpGet("LoadAdminsAndCondos")]
        public async Task<IActionResult> LoadAdminsAndCondos()
        {
            var condosList = await _companyRepository.GetCondosSelectListAsync(_contextCondos);

            var adminsList = await _companyRepository.GetCompanyAdminsSelectListAsync();

            return Ok(new AdminsAndCondosDto { Admins = adminsList, Condos = condosList});
        }

    }
}
