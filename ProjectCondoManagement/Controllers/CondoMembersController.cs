using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Linq;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CondoMembersController : ControllerBase
    {
        private readonly DataContextCondos _context;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IUnitRepository _unitRepository;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly DataContextCondos _dataContextCondos;

        public CondoMembersController(DataContextCondos context, ICondoMemberRepository condoMemberRepository, IConverterHelper converterHelper, IUserHelper userHelper,IUnitRepository unitRepository,
                                        ICondominiumRepository condominiumRepository, DataContextCondos dataContextCondos)
        {
            _context = context;
            _condoMemberRepository = condoMemberRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _unitRepository = unitRepository;
            _condominiumRepository = condominiumRepository;
            _dataContextCondos = dataContextCondos;
        }

        // GET: api/CondoMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CondoMemberDto>>> GetCondoMembers()
        {

            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            if (user == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }


            var userCompanyIds = user.Companies.Select(c => c.Id).ToList(); //busca ids das companies do user

            var condoMemberUsers = await _userHelper.GetUsersWithCompanyByRoleAsync("CondoMember"); //busca users que são condoMembers
             
            var condoMemberUsersCompaniesIds = condoMemberUsers.SelectMany(u => u.Companies).Select(c => c.Id).ToList(); //busca os ids das companies dos condoMembers

            var condoMembersUsersFromCompany = condoMemberUsers.Where(condoMember => condoMember.Companies.Any(company => userCompanyIds.Contains(company.Id))).ToList(); //selecionar somente se pertencerem à mesma company

            var condoMemberEmails = condoMembersUsersFromCompany.Select(u => u.Email).ToList();//selecionar emails

            var condoMembers = await _condoMemberRepository.GetCondoMembersByEmailsAsync(condoMemberEmails); //buscar condomembers por emails
                                                                    
            if (!condoMembers.Any())
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }

            await _condoMemberRepository.LinkImages(condoMembers); // Link images to condo members

            var condoMembersDtos = condoMembers.Select(c => _converterHelper.ToCondoMemberDto(c)).ToList();

            return condoMembersDtos;

        }

        // GET: api/CondoMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CondoMemberDto>> GetCondoMember(int id)
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            if (user == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "You do not have access to this condo member." });
            }

            var condoMember = await _condoMemberRepository.GetByIdWithIncludeAsync(id, _context);
            if (condoMember == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }

            var userCompanyIds = user.Companies.Select(c => c.Id).ToList();

            var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(condoMember.Email);

            var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

            if (!userCompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso nãp haja match de empresa
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "You do not have access to this condo member." });
            }

            var condoMembers = new List<CondoMember> { condoMember };// Create a list with the single condo member for linking images
            await _condoMemberRepository.LinkImages(condoMembers); // Link images to the single condo member

            var condoMemberDto = _converterHelper.ToCondoMemberDto(condoMember);

           

            return condoMemberDto;
        }

        // GET: api/CondoMembers/ByEmail/{email}
        [HttpGet("ByEmail/{email}")]
        public async Task<ActionResult<CondoMemberDto>> GetCondoMemberByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return NotFound();
            }

            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }


            var condoMember = await _condoMemberRepository.GetAll(_context).Include(c => c.Units).ThenInclude(u => u.Condominium)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());


            if (condoMember == null)
            {
                return NotFound();
            }

            if (!this.User.IsInRole("SysAdmin"))
            {
                var userCompanyIds = user.Companies.Select(c => c.Id).ToList();

                var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(condoMember.Email);

                var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

                if (!userCompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso não haja match de empresa
                {
                    return Conflict();
                }
            }
           
            var condoMembers = new List<CondoMember> { condoMember };// Create a list with the single condo member for linking images
            await _condoMemberRepository.LinkImages(condoMembers); // Link images to the single condo member

            

            var condoMemberDto = _converterHelper.ToCondoMemberDto(condoMember);

            return condoMemberDto;
        }

      


        [HttpGet("ByCondo/{condoId}")]
        public async Task<ActionResult<IEnumerable<CondoMemberDto>>> GetMembersByCondo(int condoId)
        {
            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }

            var members = await _condoMemberRepository.GetAll(_context)
               .Include(m => m.Units)
                   .ThenInclude(u => u.Condominium) 
               .Where(m => m.Units.Any(u => u.CondominiumId == condoId))
               .ToListAsync();

            var memberDtos = members
                .Select(m => _converterHelper.ToCondoMemberDto(m))
                .ToList();

            var userCompanyIds = user.Companies.Select(c => c.Id).ToList(); //busca ids das companies do user

            var condoMemberUsers = await _userHelper.GetUsersWithCompanyByRoleAsync("CondoMember"); //busca users que são condoMembers

            var condoMemberUsersCompaniesIds = condoMemberUsers.SelectMany(u => u.Companies).Select(c => c.Id).ToList(); //busca os ids das companies dos condoMembers

            var condoMembersUsersFromCompany = condoMemberUsers.Where(condoMember => condoMember.Companies.Any(company => userCompanyIds.Contains(company.Id))).ToList(); //selecionar somente se pertencerem à mesma company

            var condoMemberEmails = condoMembersUsersFromCompany.Select(u => u.Email).ToList();//selecionar emails

            var condoMembers = await _condoMemberRepository.GetCondoMembersByEmailsAsync(condoMemberEmails); //buscar condomembers por emails

            if (!condoMembers.Any())
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "There are no members under your management" });
            }

            await _condoMemberRepository.LinkImages(condoMembers); // Link images to condo members

            var condoMembersDtos = condoMembers.Select(c => _converterHelper.ToCondoMemberDto(c)).ToList();

            return condoMembersDtos;
    
        }



        //// POST: api/CondoMembers/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditCondoMember(int id, [FromBody] CondoMemberDto condoMemberDto)
        {

            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }

            if (id != condoMemberDto.Id)
            {
                return BadRequest();
            }

            var exists = await _condoMemberRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }


            var userCompanyIds = user.Companies.Select(c => c.Id).ToList();

            var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(condoMemberDto.Email);

            var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

            if (!userCompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso não haja match de empresa
            {
                return Ok(new Response<object>() {IsSuccess = false, Message = "Unable to update member, you do not have access to this user" });
            }

            var condoMember = _converterHelper.ToCondoMember(condoMemberDto);


            try
            {
                await _condoMemberRepository.UpdateAsync(condoMember, _context);

                return Ok(new Response<CondoMember> { IsSuccess = true, Message = "Condo member updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the condo member.");
            }

        }

        //// POST: api/CondoMembers
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostCondoMember([FromBody] CondoMemberDto condoMemberDto)
        {

            if (condoMemberDto == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }


            try
            {
                var condoMember = _converterHelper.ToCondoMember(condoMemberDto);

                if (condoMember == null)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "System error" });
                }

                await _condoMemberRepository.CreateAsync(condoMember, _context);

                return Ok(new Response<CondoMember> { IsSuccess = true, Message = "Condo member created successfully." });
            }

            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {errorMessage}");
            }

        }


        // POST api/CondoMembers/5/Units/10
        [HttpPost("{memberId}/Units/{unitId}")]
        public async Task<IActionResult> AddUnitToMember(int memberId, int unitId)
        {

            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var member = await _condoMemberRepository.GetByIdWithIncludeAsync(memberId, _context);
            if (member == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }

            var userCompanyIds = user.Companies.Select(c => c.Id).ToList();

            var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(member.Email);

            var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

            if (!userCompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso nãp haja match de empresa
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "You do not have access to these members." });
            }



            var unit = await _unitRepository.GetByIdAsync(unitId, _context);

            if (unit == null)
            {
                return NotFound();
            }


            if (member.Units.Any(u => u.Id == unitId))
            {
                return Ok(new { message = "Member already assigned to this unit." });
            }


            member.Units.Add(unit);
            await _condoMemberRepository.UpdateAsync(member, _context);

            return Ok(new { message = "Unit associated successfully" });
        }


        [HttpDelete("{memberId}/Units/{unitId}")]
        public async Task<IActionResult> RemoveMemberFromUnit(int memberId, int unitId)
        {
            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found." });
            }


            var member = await _condoMemberRepository.GetByIdWithIncludeAsync(memberId, _context);

            if (member == null)
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Member not found" });
            }

            var userCompanyIds = user.Companies.Select(c => c.Id).ToList();

            var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(member.Email);

            var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

            if (!userCompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso nãp haja match de empresa
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "You do not have access to these members." });
            }

            var unitToRemove = member.Units.FirstOrDefault(u => u.Id == unitId);
            if (unitToRemove == null)
            {
                return Ok(new { message = "Member was not assigned to this unit." });
            }

            member.Units.Remove(unitToRemove);
            await _condoMemberRepository.UpdateAsync(member, _context);

            return Ok(new { message = "Unit removed from member successfully." });
        }



        // DELETE: api/CondoMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCondoMemberAndDeactivateUser(int id)
        {
            var userEmail = this.User.Identity?.Name;

            var user2 = await _userHelper.GetUserByEmailWithCompaniesAsync(userEmail);

            if (user2 == null)
            {
                return BadRequest("User not found.");
            }


            var condoMember = await _condoMemberRepository.GetByIdAsync(id, _context);
            if (condoMember == null)
            {
                return NotFound();
            }


            var user2CompanyIds = user2.Companies.Select(c => c.Id).ToList();

            var condoMemberUser = await _userHelper.GetUserByEmailWithCompaniesAsync(condoMember.Email);

            var condoMemberCompanyIds = condoMemberUser.Companies.Select(c => c.Id);

            if (!user2CompanyIds.Any(id => condoMemberCompanyIds.Contains(id))) //caso nãp haja match de empresa
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "You do not have access to this member." });
            }


            var user = await _userHelper.GetUserByEmailAsync(condoMember.Email);
            if (user == null)
            {
                return NotFound();
            }

            await _condoMemberRepository.DeleteAsync(condoMember, _context);

            var result = await _userHelper.DeactivateUserAsync(user);
            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deactivating the user.{result.Message}");
            }

            return Ok(new Response<CondoMember> { IsSuccess = true, Message = "Condo member deleted and user deactivated successfully." });
        }


        [HttpGet("ByManager")]
        public async Task<ActionResult<List<CondoMemberDto>>> GetMembersByManager()
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            var condominiums = _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.ManagerUserId == user.Id).ToList();
            if (condominiums == null)
            {
                return new List<CondoMemberDto>();
            }

            var condominiumIds = condominiums.Select(c => c.Id).ToList();

            //buscar  members que contenham o id do condominio iguais aos da lista de ids

            var members = new List<CondoMember>();

            foreach (var id in condominiumIds)
            {
               members = await _condoMemberRepository.GetAll(_context)
              .Include(m => m.Units)
                  .ThenInclude(u => u.Condominium)
              .Where(m => m.Units.Any(u => u.CondominiumId == id))
              .ToListAsync();
            }

            var membersDto = members.Select(m => _converterHelper.ToCondoMemberDto(m)).ToList();

            return membersDto;
        }

    }
}
