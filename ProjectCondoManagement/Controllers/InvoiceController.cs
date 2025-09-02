using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
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

        public InvoiceController(IInvoiceRepository invoiceRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper)
        {
            _invoiceRepository = invoiceRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
        }

        [HttpGet("InvoiceDetails/{id}")]
        public async Task<InvoiceDto> InvoiceDetails(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id, _dataContextFinances);

            var invoiceDto = _converterHelper.ToInvoiceDto(invoice, false);

            return invoiceDto;
        }
    }
}
