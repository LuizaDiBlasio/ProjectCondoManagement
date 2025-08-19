using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin")]
    public class CondoMembersController : ControllerBase
    {
        private readonly DataContextCondos _context;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public CondoMembersController(DataContextCondos context, ICondoMemberRepository condoMemberRepository, IConverterHelper converterHelper, IUserHelper userHelper)
        {
            _context = context;
            _condoMemberRepository = condoMemberRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        // GET: api/CondoMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CondoMemberDto>>> GetCondoMembers()
        {
            var condoMembers = await _condoMemberRepository.GetAll(_context).ToListAsync();

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
            var condoMember = await _condoMemberRepository.GetByIdAsync(id, _context);
            if (condoMember == null)
            {
                return NotFound();
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

            var condoMember = await _condoMemberRepository.GetAll(_context)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            if (condoMember == null)
            {
                return null;
            }

            var condoMemberDto = _converterHelper.ToCondoMemberDto(condoMember);

            return condoMemberDto;
        }



        // POST: api/CondoMembers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditCondoMember(int id, [FromBody]CondoMemberDto condoMemberDto)
        {
            if (id != condoMemberDto.Id)
            {
                return BadRequest();
            }

            var exists = await _condoMemberRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return NotFound();
            }


            var condoMember = _converterHelper.ToCondoMember(condoMemberDto);


            try
            {
                await _condoMemberRepository.UpdateAsync(condoMember, _context);

                return Ok(new Response { IsSuccess = true, Message = "Condo member updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the condo member.");
            }

        }

        // POST: api/CondoMembers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostCondoMember([FromBody]CondoMemberDto condoMemberDto)
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

                return Ok(new Response { IsSuccess = true, Message = "Condo member created successfully." });
            }

            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {errorMessage}");
            }

        }

        // DELETE: api/CondoMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCondoMemberAndDeactivateUser(int id)
        {
            var condoMember = await _condoMemberRepository.GetByIdAsync(id, _context);
            if (condoMember == null)
            {
                return NotFound();
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

            return Ok(new Response { IsSuccess = true, Message = "Condo member deleted and user deactivated successfully." });
        }

     
    }
}
