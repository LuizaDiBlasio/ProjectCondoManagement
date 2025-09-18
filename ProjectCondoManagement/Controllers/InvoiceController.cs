using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IUserHelper _userHelper;
        private readonly ICondoMemberRepository _condoMemberRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper, IPaymentRepository paymentRepository, ICondominiumRepository condominiumRepository,
            DataContextCondos dataContextCondos, IUserHelper userHelper, ICondoMemberRepository condoMemberRepository)
        {
            _invoiceRepository = invoiceRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _paymentRepository = paymentRepository;
            _condominiumRepository = condominiumRepository;
            _dataContextCondos = dataContextCondos;
            _userHelper = userHelper;
            _condoMemberRepository = condoMemberRepository;
        }

        [HttpGet("InvoiceDetails/{id}")]
        public async Task<InvoiceDto?> InvoiceDetails(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id, _dataContextFinances);

            if (invoice == null)
            {
                return null;
            }

            var invoiceDto = _converterHelper.ToInvoiceDto(invoice, false);

            return invoiceDto;
        }




        [HttpGet("GetCondominiumsWithInvoices")]
        public async Task<List<CondominiumWithInvoicesDto>> GetCondominiumsInvoices(int id)
        {
            var managerCondos = await GetManagerCondos();

            if (managerCondos == null)
            {
                return new List<CondominiumWithInvoicesDto>();
            }

            var condosWithInvoicesDto = new List<CondominiumWithInvoicesDto>();

            foreach (var condo in managerCondos)
            {
                var condoInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                  .Where(i => i.PayerAccountId == condo.FinancialAccountId)
                                                     .ToList();

                var condoInvoicesDto = condoInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

                var condoWithInvoicesDto = new CondominiumWithInvoicesDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    InvoicesDto = condoInvoicesDto
                };

                condosWithInvoicesDto.Add(condoWithInvoicesDto);
            }

            return condosWithInvoicesDto;

        }

        [HttpGet("GetMemberCondominiumsWithInvoices")]
        public async Task<List<CondominiumWithInvoicesDto>> GetMemberCondominiumsInvoices(int id)
        {
            var email = this.User.Identity?.Name;

            var condoMember = await _condoMemberRepository.GetCondoMemberByEmailAsync(email);

            var user = await _userHelper.GetUserByEmailAsync(email);

            var condominiums = condoMember.Units.Select(u => u.Condominium).DistinctBy(c => c.Id).ToList();
            if (condominiums == null)
            {
                return new List<CondominiumWithInvoicesDto>();
            }

            var condominiumsDtos = condominiums.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList();

            var condosWithInvoicesDto = new List<CondominiumWithInvoicesDto>();

            foreach (var condo in condominiumsDtos)
            {
                var condoInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                  .Where(i => i.PayerAccountId == user.FinancialAccountId)
                                                     .ToList();

                var condoInvoicesDto = condoInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

                var condoWithInvoicesDto = new CondominiumWithInvoicesDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    InvoicesDto = condoInvoicesDto
                };

                condosWithInvoicesDto.Add(condoWithInvoicesDto);
            }

            return condosWithInvoicesDto;

        }


        [HttpGet("GetAllCondoMembersInvoices")]
        public async Task<List<CondominiumWithInvoicesDto>> GetAllCondoMembersInvoices()
        {
            var managerCondos = await GetManagerCondos();


            var condosWithInvoicesDto = new List<CondominiumWithInvoicesDto>();

            foreach (var condo in managerCondos)
            {
                var condoMembersInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                  .Where(i => i.PayerAccountId != condo.FinancialAccountId)
                                                     .ToList();

                var condoMembersInvoicesDto = condoMembersInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

                var condoWithInvoicesDto = new CondominiumWithInvoicesDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    InvoicesDto = condoMembersInvoicesDto
                };

                condosWithInvoicesDto.Add(condoWithInvoicesDto);
            }
        
            return condosWithInvoicesDto;
        }

        //Método auxiliar
        public async Task<List<Condominium>> GetManagerCondos()
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            var condominiums = await _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.ManagerUserId == user.Id).ToListAsync();
            if (condominiums == null)
            {
                return new List<Condominium>();
            }

            return condominiums;
        }
    }
}
