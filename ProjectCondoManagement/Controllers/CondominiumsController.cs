using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using ProjectCondoManagement.Migrations.CondosDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  
    public class CondominiumsController : ControllerBase
    {
        private readonly DataContextCondos _context;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumHelper _condominiumHelper;
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public CondominiumsController(DataContextCondos context,
                                      ICondominiumRepository condominiumRepository,
                                      IConverterHelper converterHelper,
                                      IUserHelper userHelper,
                                      ICondominiumHelper condominiumHelper,
                                      IFinancialAccountRepository financialAccounRepository,
                                      IPaymentRepository paymentRepository,
                                      DataContextFinances dataContextFinances,
                                      ICondoMemberRepository condoMemberRepository,
                                      IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _condominiumRepository = condominiumRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumHelper = condominiumHelper;
            _financialAccountRepository = financialAccounRepository;
            _paymentRepository = paymentRepository;
            _dataContextFinances = dataContextFinances;
            _condoMemberRepository = condoMemberRepository;
            _invoiceRepository = invoiceRepository;
        }

        // GET: api/Condominiums
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CondominiumDto>>> GetCondominiums()
        {

            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }



            var condominiums = await _condominiumRepository.GetAll(_context).Where(c => c.CompanyId == user.CompanyId).ToListAsync();
            if (condominiums == null)
            {
                return new List<CondominiumDto>();
            }

            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList();

            if (condominiumsDtos == null)
            {
                return new List<CondominiumDto>();
            }

            await _condominiumRepository.LinkManager(condominiumsDtos);
            await _condominiumRepository.LinkFinancialAccount(condominiumsDtos);

            return condominiumsDtos;
        }

        

        // GET: api/Condominiums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CondominiumDto>> GetCondominium(int id)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(id, _context);

            if (condominium == null)
            {
                return NotFound();
            }

            var condominiumDto = _converterHelper.ToCondominiumDto(condominium, false);
            if (condominiumDto == null)
            {
                return NotFound();
            }

            var result = await _condominiumHelper.LinkCompanyToCondominiumAsync(condominiumDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            List<CondominiumDto> condominiumsDtos = new List<CondominiumDto>();
            condominiumsDtos.Add(condominiumDto);
            await _condominiumRepository.LinkManager(condominiumsDtos);

            return condominiumDto;
        }


        // POST: api/CondoMembers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditCondominium(int id, [FromBody] CondominiumDto condominiumDto)
        {
            if (id != condominiumDto.Id)
            {
                return BadRequest();
            }

            var exists = await _condominiumRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return NotFound();
            }


            var condominium = _converterHelper.ToCondominium(condominiumDto, false);

            try
            {
                await _condominiumRepository.UpdateAsync(condominium, _context);

                return Ok(new Response<object> { IsSuccess = true, Message = "Condominium updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the condominium.");
            }

        }





        // POST: api/Condominiums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostCondominium([FromBody] CondominiumDto condominiumDto)
        {
            if (condominiumDto == null)
            {
                return BadRequest("Request body is null.");
            }

            try
            {

                var email = this.User.Identity?.Name;

                var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                condominiumDto.CompanyId = user.Company.Id;

                if (user.Company == null)
                {
                    return BadRequest("User does not belong to a company.");
                }


                var condominium = _converterHelper.ToCondominium(condominiumDto, true);

                if (condominium == null)
                {
                    return BadRequest("Conversion failed. Invalid data.");
                }


                //Atribuir financial account
                var financialAccount = await _financialAccountRepository.CreateFinancialAccountAsync(condominium.CondoName);

                condominium.FinancialAccountId = financialAccount.Id;


                await _condominiumRepository.CreateAsync(condominium, _context);

                return Ok(new Response<object> { IsSuccess = true, Message = "Condominium created successfully." });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/Condominiums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCondominium(int id)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(id, _context);
            if (condominium == null)
            {
                return NotFound();
            }

            try
            {
                await _condominiumRepository.DeleteAsync(condominium, _context);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
            return NoContent();
        }



        [HttpGet("GetCondoManagerCondominiumsWithPayments/{email}")]
        public async Task<ActionResult<List<CondominiumWithPaymentsDto>>> GetCondoManagerCondominiumsWithPayments(string email)
        {
            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            var condominiums = await _condominiumRepository.GetAll(_context).Where(c => c.ManagerUserId == user.Id).ToListAsync();
            if (condominiums == null)
            {
                return new List<CondominiumWithPaymentsDto>();
            }


            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList();

            if (condominiumsDtos == null)
            {
                return new List<CondominiumWithPaymentsDto>();
            }

            // FinancialAccountIds de todos os condomínios
            var condominiumFinancialAccountIds = condominiums.Select(c => c.FinancialAccountId)
                                                             .Where(id => id.HasValue)
                                                             .Select(id => id.Value)
                                                             .ToList();

            // Buscar todos os pagamentos onde  Fin. acc Id do pagante == Fin. acc Id Condo.
            var allPayments = await _paymentRepository.GetAll(_dataContextFinances)
                .Where(p => condominiumFinancialAccountIds.Contains(p.PayerFinancialAccountId))
                .Include(p => p.Transaction)
                .Include(p => p.Expenses)
                .ToListAsync();

            // Converter 
            var allPaymentsDto = allPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList();

            //Lista a ser populada
            var condosWithPaymentsDtoList = new List<CondominiumWithPaymentsDto>();

            foreach (var condo in condominiums)
            {
                // Buscar pagamentos para o condomínio atual, pelo FinancialAccountId
                var condoPaymentsDto = allPaymentsDto
                    .Where(p => p.PayerFinancialAccountId == condo.FinancialAccountId)
                    .ToList();

                
                var condominiumWithPaymentsDto = new CondominiumWithPaymentsDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    CondoPayments = condoPaymentsDto
                };

                condosWithPaymentsDtoList.Add(condominiumWithPaymentsDto);
            }

            return condosWithPaymentsDtoList;

        }

        // GET: api/Condominiums/ByManager
        [HttpGet("ByManager")]
        public async Task<ActionResult<IEnumerable<CondominiumDto>>> GetManagerCondos()
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            var condominiums = await _condominiumRepository.GetAll(_context).Where(c => c.ManagerUserId == user.Id).ToListAsync();
            if (condominiums == null)
            {
                return new List<CondominiumDto>();
            }



            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c)).ToList();

            if (condominiumsDtos == null)
            {
                return new List<CondominiumDto>();
            }

            await _condominiumRepository.LinkManager(condominiumsDtos);
            await _condominiumRepository.LinkFinancialAccount(condominiumsDtos);

            return condominiumsDtos;
        }




        [HttpGet("ByCondoMember")]
        public async Task<ActionResult<IEnumerable<CondominiumDto>>> GetCondoMemberCondos()
        {
            var email = this.User.Identity?.Name;

            var condoMember = await _condoMemberRepository.GetCondoMemberByEmailAsync(email);

            var condominiums = condoMember.Units.Select(u => u.Condominium).ToList();
            if (condominiums == null)
            {
                return new List<CondominiumDto>();
            }

            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList();


            return condominiumsDtos;
        }

    }
}
