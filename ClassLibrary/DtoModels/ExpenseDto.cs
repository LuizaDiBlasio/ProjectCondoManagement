using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
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

        public CondominiumDto CondominiumDto { get; set; }  

        public EnumDto ExpenseTypeDto { get; set; }
    }
}
