using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class ExpenseDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Detail { get; set; }

        public int CondominiumId { get; set; }


        [Display(Name = "Type")]
        public EnumDto ExpenseTypeDto { get; set; }

    }
}
