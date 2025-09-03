using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
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

        public InvoiceController(IInvoiceRepository invoiceRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper, IPaymentRepository paymentRepository, ICondominiumRepository condominiumRepository,
            DataContextCondos dataContextCondos, IUserHelper userHelper)
        {
            _invoiceRepository = invoiceRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _paymentRepository = paymentRepository;
            _condominiumRepository = condominiumRepository;
            _dataContextCondos = dataContextCondos;
            _userHelper = userHelper;
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


        [HttpGet("GetCondominiumInvoices/{id}")]
        public async Task<List<InvoiceDto>> GetCondominiumInvoices(int id)
        {
            // get condominium 
            var condominium = await _condominiumRepository.GetByIdAsync(id, _dataContextCondos);
            if (condominium == null)
            {
                return new List<InvoiceDto>();
            }

            var condoInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                   .Where(i => i.CondominiumId == id && i.PayerAccountId == condominium.FinancialAccountId)
                                                      .ToList();

                //converter

                var condoInvoicesDto = condoInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

                return condoInvoicesDto;
            
        }


        [HttpPost("GetCondoMemberInvoices")]
        public async Task<List<InvoiceDto>> GetCondoMemberInvoices([FromBody] string userEmail)
        {

            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return new List<InvoiceDto>();
            }

            // get user payments
            var userInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                .Where(i => i.PayerAccountId == user.FinancialAccountId)
                                                .ToList();


            //converter
            var userInvoicesDto = userInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

            return userInvoicesDto;
        }



        [HttpGet("GetAllCondoMembersInvoices/{id}")]
        public async Task<List<InvoiceDto>> GetAllCondoMembersInvoices(int id)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(id, _dataContextCondos);
            if (condominium == null)
            {
                return new List<InvoiceDto>();
            }

            var condoMembersInvoices = _invoiceRepository.GetAll(_dataContextFinances)
                                                   .Where(i => i.CondominiumId == id && i.PayerAccountId != condominium.FinancialAccountId)
                                                      .ToList();

            //converter

            var condoMembersInvoicesDto = condoMembersInvoices.Select(p => _converterHelper.ToInvoiceDto(p, false)).ToList() ?? new List<InvoiceDto>();

            return condoMembersInvoicesDto;
        }
    }
}
