using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using ProjectCondoManagement.Migrations.CondosDb;
using System.Threading.Tasks;
using Twilio.Rest.Iam.V1;

namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    //[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly IOccurenceRepository _occurrenceRepository;

        public MeetingController(IMeetingRepository meetingRepository, DataContextCondos dataContextCondos, IConverterHelper converterHelper, IUserHelper userHelper, ICondominiumRepository condominiumRepository,
                                    ICondoMemberRepository condoMemberRepository, IOccurenceRepository occurrenceRepository)
        {
            _meetingRepository = meetingRepository;
            _dataContextCondos = dataContextCondos;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
            _condoMemberRepository = condoMemberRepository;
            _occurrenceRepository = occurrenceRepository;
        }


        [HttpGet("GetCondosWithMeetings")]
        public async Task<ActionResult<List<CondominiumWithMeetingsDto>>> GetCondosWithMeetings()
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            var condominiums = _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.ManagerUserId == user.Id).ToList();
            if (condominiums == null)
            {
                return new List<CondominiumWithMeetingsDto>();
            }

            var condominiumIds = condominiums.Select(c => c.Id).ToList();

            //buscar  meetings que contenham o id do condominio iguais aos da lista de ids
            var allCondosMeetings = await _meetingRepository.GetAll(_dataContextCondos)
                                                        .Where(m => condominiumIds
                                                        .Contains(m.CondominiumId))
                                                        .ToListAsync();

            var allCondosMeetingsDto = allCondosMeetings.Select(m => _converterHelper.ToMeetingDto(m)).ToList() ?? new List<MeetingDto>();

            var condosWithMeetingsDto = new List<CondominiumWithMeetingsDto>();

            foreach (var condo in condominiums)
            {
                var condoMeetings = allCondosMeetingsDto.Where(m => m.CondominiumId == condo.Id)
                                                             .ToList();

                var condoWithMeetings = new CondominiumWithMeetingsDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    MeetingsDto = condoMeetings
                };

                condosWithMeetingsDto.Add(condoWithMeetings);
            }
            return Ok(condosWithMeetingsDto);
        }


        [HttpGet ("GetCondoMemberMeetings/{email}")]
        public async Task<ActionResult<List<CondominiumWithMeetingsDto>>> GetCondoMemberMeetings (string email)
        {
            var condoMember = await _condoMemberRepository.GetCondoMemberByEmailAsync(email);

            var user = await _userHelper.GetUserByEmailAsync(email);

            var condominiums = condoMember.Units.Select(u => u.Condominium).DistinctBy(c => c.Id).ToList(); //distinctBy para evitar repetição de condos
            if (condominiums == null)
            {
                return new List<CondominiumWithMeetingsDto>();
            }

            var condominiumIds = condominiums.Select(c => c.Id).ToList();

            //buscar  ocorrencias que contenham o id do condominio iguais aos da lista de ids
            var allCondosMeetings = await _meetingRepository.GetAll(_dataContextCondos)
                                                        .Include(m => m.CondoMembers)
                                                        .Where(m => condominiumIds
                                                        .Contains(m.CondominiumId))
                                                        .ToListAsync();

            var meetingsWithInvitation = allCondosMeetings.Where(c => c.CondoMembers.Any(cm => cm.Id == condoMember.Id));

            var meetingsWithInvitationDto = meetingsWithInvitation.Select(m => _converterHelper.ToMeetingDto(m)).ToList() ?? new List<MeetingDto>();

            var condosWithMeetingsDto = new List<CondominiumWithMeetingsDto>();

            foreach (var condo in condominiums)
            {
                var condoMeetings = meetingsWithInvitationDto.Where(m => m.CondominiumId == condo.Id)
                                                             .ToList();

                var condoWithMeetings = new CondominiumWithMeetingsDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    MeetingsDto = condoMeetings
                };

                condosWithMeetingsDto.Add(condoWithMeetings);
            }
            return Ok(condosWithMeetingsDto);

        }

        // GET: MeetingController/Details/5
        [HttpGet("GetMeetingWithMembersAndOccurrences/{id}")]
        public async Task<ActionResult<MeetingDto?>> GetMeeting(int id)
        {
            var meeting = await _meetingRepository.GetMeetingWithCondomembersAndOccurrences(id);

            if (meeting == null)
            {
                return null;
            }

            var meetingDto = _converterHelper.ToMeetingDto(meeting);
            
            return Ok(meetingDto);
        }


        [HttpPost("GetSelectedCondoMembers")]
        public async Task<ActionResult<List<CondoMemberDto>>> GetSelectedCondoMembers([FromBody] List<int> condoMembersIds)
        {
            var condoMembers = await _condoMemberRepository.GetAll(_dataContextCondos)
                                                       .Where(m => condoMembersIds
                                                       .Contains(m.Id))
                                                       .ToListAsync();

            var condoMembersDto = condoMembers.Select(c => _converterHelper.ToCondoMemberDto(c)).ToList();  

            return Ok(condoMembersDto); 
        }


        [HttpPost("GetSelectedOccurrences")]
        public async Task<ActionResult<List<OccurrenceDto>>> GetSelectedOccurrences([FromBody] List<int> occurrencesIds)
        {
            if(occurrencesIds.Count == 0)
            {
                return Ok(new List<OccurrenceDto>());    
            }

            var occurrences = await _occurrenceRepository.GetAll(_dataContextCondos)
                                                       .Where(o => occurrencesIds
                                                       .Contains(o.Id))
                                                       .ToListAsync();

            var occurrencesDto = occurrences.Select(o => _converterHelper.ToOccurrenceDto(o)).ToList();

            return Ok(occurrencesDto);
        }

       

        // POST: MeetingController/Create
        [HttpPost("CreateMeeting")]
        public async Task<ActionResult> Create([FromBody] MeetingDto meetingDto)
        {
            try
            {
                var meeting = _converterHelper.ToMeeting(meetingDto, true);

                if (meeting.Occurences != null && meeting.Occurences.Any())
                {
                    _dataContextCondos.Occurences.AttachRange(meeting.Occurences); //avisar ao ef é que uma lista de entidades que já existem
                }

                if (meeting.CondoMembers != null && meeting.CondoMembers.Any())
                {
                    _dataContextCondos.CondoMembers.AttachRange(meeting.CondoMembers); //avisar ao ef é que uma lista de entidades que já existem
                }

                await _meetingRepository.CreateAsync(meeting, _dataContextCondos);

                return Ok(new Response<object>() { IsSuccess = true });

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("EditMeeting")]
        public async Task<ActionResult> EditMeeting([FromBody] MeetingDto meetingDto)
        {
            try
            {
                var meetingToUpdate = await _dataContextCondos.Meeting
                    .Include(m => m.Occurences)
                    .Include(m => m.CondoMembers)
                    .FirstOrDefaultAsync(o => o.Id == meetingDto.Id);

                if (meetingToUpdate == null)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "Unable to modify, meeting not found in the system" });
                }

                // Atualizar as propriedades 
                meetingToUpdate.CondominiumId = meetingDto.CondominiumId;   
                meetingToUpdate.DateAndTime = meetingDto.DateAndTime;
                meetingToUpdate.Description = meetingDto.Description;
                meetingToUpdate.IsExtraMeeting = meetingDto.IsExtraMeeting; 
                meetingToUpdate.Title = meetingDto.Title;

                // buscar lista de occurrences selecionados
                var selectedOccurrenceIds = meetingDto.OccurencesDto.Select(o => o.Id).ToList();

                // Remover occurences que foram desmarcadas
                var occurrencesToRemove = meetingToUpdate.Occurences
                    .Where(u => !selectedOccurrenceIds.Contains(u.Id))
                    .ToList();

                foreach (var occurrence in occurrencesToRemove)
                {
                    meetingToUpdate.Occurences.Remove(occurrence);
                }

                // Adicionar novas units
                var existingOccurrenceIds = meetingToUpdate.Occurences.Select(o => o.Id);
                var newOccurrenceIds = selectedOccurrenceIds.Except(existingOccurrenceIds).ToList();

                if (newOccurrenceIds.Any())
                {
                    var occurrencesToAdd = await _dataContextCondos.Occurences
                        .Where(o => newOccurrenceIds.Contains(o.Id))
                        .ToListAsync();

                    foreach (var occurrence in occurrencesToAdd)
                    {
                        meetingToUpdate.Occurences.Add(occurrence);
                    }
                }

                // buscar lista de condomembers selecionados
                var selectedCondomembersIds = meetingDto.CondoMembersDto.Select(o => o.Id).ToList();

                // Remover occurences que foram desmarcadas
                var condomembersToRemove = meetingToUpdate.CondoMembers
                    .Where(c => !selectedCondomembersIds.Contains(c.Id))
                    .ToList();

                foreach (var condomember in condomembersToRemove)
                {
                    meetingToUpdate.CondoMembers.Remove(condomember);
                }

                // Adicionar novas units
                var existingCondomembersIds = meetingToUpdate.CondoMembers.Select(c => c.Id);
                var newCondomembersIds = selectedCondomembersIds.Except(existingCondomembersIds).ToList();

                if (newCondomembersIds.Any())
                {
                    var condomembersToAdd = await _dataContextCondos.CondoMembers
                        .Where(c => newCondomembersIds.Contains(c.Id))
                        .ToListAsync();

                    foreach (var condomember in condomembersToAdd)
                    {
                        meetingToUpdate.CondoMembers.Add(condomember);
                    }
                }


                await _dataContextCondos.SaveChangesAsync();

                return Ok(new Response<object>() { IsSuccess = true });

            }
            catch
            {
                return BadRequest();    
            }
        }

        // DELETE: MeetingController/Delete/5
        [HttpDelete("DeleteMeeting/{id}")]
        public async Task<ActionResult> DeleteMeeting(int id)
        {
            try
            {
                var meeting = await _meetingRepository.GetMeetingWithCondomembersAndOccurrences(id);

                if (meeting == null)
                {
                    return Ok(new Response<object>() { IsSuccess = false, Message = "Delete action failed. Meeting not found in the system." });
                }

                //desfazer relações
                meeting.CondoMembers.Clear();
                meeting.Occurences.Clear();

                await _meetingRepository.DeleteAsync(meeting, _dataContextCondos);

                return Ok(new Response<object>() {IsSuccess = true });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
