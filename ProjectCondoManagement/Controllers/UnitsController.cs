using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitRepository _unitRepository;
        private readonly DataContextCondos _context;
        private readonly IConverterHelper _converterHelper;
        private readonly ICondominiumRepository _condominiumRepository;

        public UnitsController(IUnitRepository unitRepository, DataContextCondos context, IConverterHelper converterHelper, ICondominiumRepository condominiumRepository)
        {
            _unitRepository = unitRepository;
            _context = context;
            _converterHelper = converterHelper;
            _condominiumRepository = condominiumRepository;
        }

        // GET: api/<UnitsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
        {
            var units = await _unitRepository.GetAll(_context).Include(u => u.Condominium).ToListAsync();
            if (units == null)
            {
                return new List<UnitDto>();
            }

            var unitsDtos = units.Select(c => _converterHelper.ToUnitDto(c)).ToList();

            if (unitsDtos == null)
            {
                return new List<UnitDto>();
            }


            return unitsDtos;
        }

        [HttpGet("condo/{condoId}")]
        public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnitsFromCondo(int CondoId)
        {
            var units = await _unitRepository.GetAll(_context).Where(u => u.CondominiumId == CondoId).Include(u => u.Condominium).ToListAsync();
            if (units == null)
            {
                return new List<UnitDto>();
            }

            var unitsDtos = units.Select(c => _converterHelper.ToUnitDto(c)).ToList();

            if (unitsDtos == null)
            {
                return new List<UnitDto>();
            }


            return unitsDtos;
        }

 


        // GET api/<UnitsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitDto>> GetUnit(int id)
        {
            var unit = await _unitRepository.GetByIdWithIncludeAsync(id, _context);

            if (unit == null)
            {
                return NotFound();
            }

            var unitDto = _converterHelper.ToUnitDto(unit);
            if (unitDto == null)
            {
                return NotFound();
            }


            return unitDto;
        }

        // POST api/<UnitsController>
        [HttpPost]
        public async Task<IActionResult> PostUnit([FromBody] UnitDto unitDto)
        {
            if (unitDto == null)
            {
                return BadRequest("Request body is null.");
            }

            try
            {

                var unit = _converterHelper.ToUnit(unitDto, true);

                if (unit == null)
                {
                    return BadRequest("Conversion failed. Invalid data.");
                }




                await _unitRepository.CreateAsync(unit, _context);

                return Ok(new Response<Unit> { IsSuccess = true, Message = "Unit created successfully.", Results = unit });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }

        //POST: api/Units/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditUnits(int id, [FromBody] UnitDto unitDto)
        {
            if (id != unitDto.Id)
            {
                return BadRequest();
            }

            var exists = await _unitRepository.ExistAsync(id, _context);

            if (!exists)
            {
                return NotFound();
            }


            var unit = _converterHelper.ToUnit(unitDto, false);


            try
            {
                await _unitRepository.UpdateAsync(unit, _context);

                return Ok(new Response<Unit> { IsSuccess = true, Message = "Unit updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the condominium.");
            }

        }

        // DELETE api/<UnitsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var unit = await _unitRepository.GetByIdAsync(id, _context);
            if (unit == null)
            {
                return NotFound();
            }

            try
            {
                await _unitRepository.DeleteAsync(unit, _context);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        //GetCondoUnitsList
        [HttpGet("GetCondoUnitsList/{id}")]
        public async Task<ActionResult<List<SelectListItem>>> GetCondoUnitsList(int id)
        {
            var condoUnits = await _unitRepository.GetCondoUnitsList(id);

            return Ok(condoUnits);  
        }




    }
}
