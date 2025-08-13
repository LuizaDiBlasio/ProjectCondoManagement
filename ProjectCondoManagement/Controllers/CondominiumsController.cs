using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin")]
    public class CondominiumsController : ControllerBase
    {
        private readonly DataContextCondos _context;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public CondominiumsController(DataContextCondos context, ICondominiumRepository condominiumRepository, IConverterHelper converterHelper, IUserHelper userHelper)
        {
            _context = context;
            _condominiumRepository = condominiumRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        // GET: api/Condominiums
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Condominium>>> GetCondominiums()
        {
            return await _condominiumRepository.GetAll(_context).ToListAsync(); ;
        }

        // GET: api/Condominiums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Condominium>> GetCondominium(int id)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(id, _context);

            if (condominium == null)
            {
                return NotFound();
            }

            return condominium;
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


            var condominium = _converterHelper.ToCondominium(condominiumDto);


            try
            {
                await _condominiumRepository.UpdateAsync(condominium, _context);

                return Ok(new Response { IsSuccess = true, Message = "Condominium updated successfully." });
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


                var condominium = _converterHelper.ToCondominium(condominiumDto);

                if (condominium == null)
                {
                    return BadRequest("Conversion failed. Invalid data.");
                }

     
                

                await _condominiumRepository.CreateAsync(condominium, _context);

                return Ok(new Response { IsSuccess = true, Message = "Condominium created successfully." });
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

    }
}
