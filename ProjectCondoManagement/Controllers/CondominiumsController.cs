using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectCondoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CondominiumsController : ControllerBase
    {
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly DataContextCondos _context;
        private readonly IConverterHelper _converterHelper;

        public CondominiumsController(ICondominiumRepository condominiumRepository,DataContextCondos context,IConverterHelper converterHelper)
        {
            _condominiumRepository = condominiumRepository;
            _context = context;
            _converterHelper = converterHelper;
        }

        // GET: api/Condominiums
        [HttpGet("GetCondominiums")]
        public async Task<ActionResult<IEnumerable<CondominiumDto>>> GetCondominiums()
        {
            var condominiums = await _condominiumRepository.GetAll(_context).ToListAsync();

            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c)).ToList();

            return Ok(condominiumsDtos);
        }

        // GET api/Condominiums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CondominiumDto>> Get(int id)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(id,_context);

            if(condominium == null)
            {
                return NotFound();
            }

            var condominiumDto = _converterHelper.ToCondominiumDto(condominium);

            return Ok(condominiumDto);
        }

        // POST api/<CondominiumsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CondominiumsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CondominiumsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
