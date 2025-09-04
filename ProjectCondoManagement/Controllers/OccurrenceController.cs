using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OccurrenceController : Controller
    {
        private readonly IOccurenceRepository _occurrenceRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IConverterHelper _converterHelper;
        private readonly ICondoMemberRepository _condoMemberRepository;

        public OccurrenceController(IOccurenceRepository occurrenceRepository, DataContextCondos dataContextCondos, IConverterHelper converterHelper, ICondoMemberRepository condoMemberRepository)
        {
            _occurrenceRepository = occurrenceRepository;
            _dataContextCondos = dataContextCondos;
            _converterHelper = converterHelper;
            _condoMemberRepository = condoMemberRepository;
        }


        // GET: OccurrenceController
        [HttpGet("GetAllCondoOccurrences/{id}")]
        public async Task<ActionResult<List<OccurrenceDto>>> GetAllCondoOccurrences(int id)
        {
            var condoOccurrences = _occurrenceRepository.GetAll(_dataContextCondos).Where(o => o.CondominiumId == id). ToList();

            var occurrencesDto = condoOccurrences.Select(o => _converterHelper.ToOccurrenceDto(o)).ToList() ?? new List<OccurrenceDto>();

            return Ok(occurrencesDto);    
        }


        [HttpGet("GetOccurrenceWithUnits/{id}")]
        // GET: OccurrenceController/Details/5
        public async Task<OccurrenceDto?> GetOccurrenceWithUnits(int id)
        {
            var occurrence = await _occurrenceRepository.GetOccurrenceWithUnits(id);
            if(occurrence == null)
            {
                return null;
            }

            var occurrenceDto = _converterHelper.ToOccurrenceDto(occurrence, false);

            return occurrenceDto;   
        }

        // POST : GetSelectedUnits
        [HttpPost("GetSelectedUnits")]
        public async Task<ActionResult<List<UnitDto>>> GetSelectedUnits([FromBody] List<int> selectedUnitsIds)
        {
            var selectedUnits = await _occurrenceRepository.GetSelectedUnits(selectedUnitsIds);

            var selectedUnitsDtos = selectedUnits.Select(u => _converterHelper.ToUnitDto(u)).ToList() ?? new List<UnitDto>();

            return selectedUnitsDtos; 
        }


        // POST: OccurrenceController/Create
        [HttpPost("CreateOccurrence")]
        public async Task<ActionResult> CreateOccurrence([FromBody] OccurrenceDto occurrenceDto)
        {
            try
            {
                var occurrence = _converterHelper.ToOccurrence(occurrenceDto, true);

                if (occurrence.Units != null && occurrence.Units.Any())
                {
                    _dataContextCondos.Units.AttachRange(occurrence.Units); //avisar ao ef é que uma lista de entidades que já existem
                }

                await _occurrenceRepository.CreateAsync(occurrence, _dataContextCondos);

                return Ok(new Response<object>() { IsSuccess = true} );
            }
            catch
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Unable to register occurrence due to server error" });
            }
        }


        // POST: OccurrenceController/Edit/5
        [HttpPost("EditOccurrence")]
        public async Task<ActionResult> EditOccurrence([FromBody] OccurrenceDto occurrenceDto)
        {
            try
            {
                // buscar occurrence
                var occurrenceToUpdate = await _dataContextCondos.Occurences
                    .Include(o => o.Units)
                    .FirstOrDefaultAsync(o => o.Id == occurrenceDto.Id);

                if (occurrenceToUpdate == null)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message="Unable to edit, occurrence not found"}); 
                }

                // Atualizar as propriedades 
                occurrenceToUpdate.Subject = occurrenceDto.Subject;
                occurrenceToUpdate.Details = occurrenceDto.Details;
                occurrenceToUpdate.DateAndTime = occurrenceDto.DateAndTime;
                occurrenceToUpdate.IsResolved = occurrenceDto.IsResolved;
                occurrenceToUpdate.ResolutionDate = occurrenceDto.ResolutionDate;
                occurrenceToUpdate.CondominiumId = occurrenceDto.CondominiumId;

                // buscar lista de selecionados
                var selectedUnitIds = occurrenceDto.UnitDtos.Select(u => u.Id).ToList();

                // Remover units que foram desmarcadas
                var unitsToRemove = occurrenceToUpdate.Units
                    .Where(u => !selectedUnitIds.Contains(u.Id))
                    .ToList();

                foreach (var unit in unitsToRemove)
                {
                    occurrenceToUpdate.Units.Remove(unit);
                }

                // Adicionar novas units
                var existingUnitIds = occurrenceToUpdate.Units.Select(u => u.Id);
                var newUnitIds = selectedUnitIds.Except(existingUnitIds).ToList();

                if (newUnitIds.Any())
                {
                    var unitsToAdd = await _dataContextCondos.Units
                        .Where(u => newUnitIds.Contains(u.Id))
                        .ToListAsync();

                    foreach (var unit in unitsToAdd)
                    {
                        occurrenceToUpdate.Units.Add(unit);
                    }
                }

               
                await _dataContextCondos.SaveChangesAsync();

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return Ok(new Response<object>() { IsSuccess = false, Message = "Unable to update occurrence due to server error" });
            }
        }


        [HttpGet("GetCondoMemberOccurrences/{email}")]
        public async Task<ActionResult<List<OccurrenceDto>>> GetCondoMemberOccurrences(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }

            var condoMember = await _condoMemberRepository.GetAll(_dataContextCondos)
                                                          .Include(c => c.Units)
                                                          .ThenInclude(u =>u.Condominium)
                                                          .Include(c => c.Units)
                                                          .ThenInclude(u => u.Occurrences)
                                                          .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            if (condoMember == null)
            {
                return null;
            }

            var occurrences = condoMember.Units.SelectMany(u => u.Occurrences).ToList();

            return occurrences.Select(o => _converterHelper.ToOccurrenceDto(o, false)).ToList() ?? new List<OccurrenceDto>();
        }


    }
}
