using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CondoMembersController : ControllerBase
    {
        private readonly DataContextCondos _context;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IUnitRepository _unitRepository;

        public CondoMembersController(DataContextCondos context, ICondoMemberRepository condoMemberRepository, IConverterHelper converterHelper, IUserHelper userHelper,IUnitRepository unitRepository)
        {
            _context = context;
            _condoMemberRepository = condoMemberRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _unitRepository = unitRepository;
        }

        // GET: api/CondoMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CondoMemberDto>>> GetCondoMembers()
        {

            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var condoMembers = await _condoMemberRepository.GetAll(_context).Where(c => c.CompanyId == user.CompanyId).Include(c => c.Units).ThenInclude(u => u.Condominium).ToListAsync();

            if (condoMembers == null)
            {
                return NotFound();
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

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var condoMember = await _condoMemberRepository.GetByIdWithIncludeAsync(id, _context);
            if (condoMember == null)
            {
                return NotFound();
            }

            if(condoMember.CompanyId != user.CompanyId)
            {
                return BadRequest("You do not have access to this condo member.");
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
                return BadRequest();
            }

            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var condoMember = await _condoMemberRepository.GetAll(_context).Include(c => c.Units).ThenInclude(u => u.Condominium)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            if (condoMember == null)
            {
                return null;
            }

            if (condoMember.CompanyId != user.CompanyId)
            {
                return BadRequest("You do not have access to this condo member.");
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

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var members = await _condoMemberRepository.GetAll(_context)
               .Include(m => m.Units)
                   .ThenInclude(u => u.Condominium) 
               .Where(m => m.Units.Any(u => u.CondominiumId == condoId))
               .ToListAsync();

            var memberDtos = members
                .Select(m => _converterHelper.ToCondoMemberDto(m))
                .ToList();

            if (memberDtos.Any(m => m.CompanyId != user.CompanyId))
            {
                return BadRequest("You do not have access to these condo members.");
            }

            return Ok(memberDtos);
        }



        //// POST: api/CondoMembers/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditCondoMember(int id, [FromBody] CondoMemberDto condoMemberDto)
        {

            var userEmail = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (id != condoMemberDto.Id)
            {
                return BadRequest();
            }

            var exists = await _condoMemberRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return NotFound();
            }

            if (condoMemberDto.CompanyId != user.CompanyId)
            {
                return BadRequest("You do not have access to edit this condo member.");
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
                return BadRequest("Request body is null.");
            }


            try
            {
                var condoMember = _converterHelper.ToCondoMember(condoMemberDto);

                if (condoMember == null)
                {
                    return BadRequest("Conversion failed. Invalid data.");
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

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var member = await _condoMemberRepository.GetByIdWithIncludeAsync(memberId, _context);
            if (member == null)
            {
                return NotFound();
            }

            if(member.CompanyId != user.CompanyId)
            {
                return BadRequest("You do not have access to edit this condo member.");
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

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var member = await _condoMemberRepository.GetByIdWithIncludeAsync(memberId, _context);

            if (member == null)
            {
                return NotFound();
            }

            if (member.CompanyId != user.CompanyId)
            {
                return BadRequest("You do not have access to edit this condo member.");
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

            var user2 = await _userHelper.GetUserByEmailWithCompanyAsync(userEmail);

            if (user2 == null)
            {
                return BadRequest("User not found.");
            }


            var condoMember = await _condoMemberRepository.GetByIdAsync(id, _context);
            if (condoMember == null)
            {
                return NotFound();
            }

            if(condoMember.CompanyId != user2.CompanyId)
            {
                return BadRequest("You do not have access to delete this condo member.");
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


    }
}
