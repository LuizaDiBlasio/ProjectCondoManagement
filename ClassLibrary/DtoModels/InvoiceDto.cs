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

        public int? PayerAccountId { get; set; }

        public FinancialAccountDto? PayerFinancialAccountDto { get; set; }

        public int? BeneficiaryAccountId { get; set; }

        public FinancialAccountDto? BeneficiaryFinancialAccountDto { get; set; }

        public int PaymentId { get; set; }


        public PaymentDto Payment { get; set; }

    }
 }
