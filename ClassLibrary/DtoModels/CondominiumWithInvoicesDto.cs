using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CondominiumWithInvoicesDto
    {
        public int CondominiumId { get; set; }  

        public string CondoName { get; set; }   

        public List<InvoiceDto> InvoicesDto { get; set; }
    }
}
