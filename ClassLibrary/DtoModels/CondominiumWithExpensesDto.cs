using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CondominiumWithExpensesDto
    {
        public int CondominiumId { get; set; }  

        public string CondoName { get; set; }

        public List<ExpenseDto> ExpensesDto { get; set; } 
    }
}
