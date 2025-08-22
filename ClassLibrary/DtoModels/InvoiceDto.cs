using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class InvoiceDto
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public int CondominiumId { get; set; }

        public int AccountId { get; set; }

        public FinancialAccountDto FinancialAccountDto { get; set; }

        public string UserEmail { get; set; }

        public PaymentDto Payment { get; set; }
    }
}
