using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class IndexCompaniesViewModel
    {
        public IEnumerable<CompanyDto> Companies { get; set; }

        public IndexCompaniesViewModel()
        {
            Companies = new List<CompanyDto>();        
        }
    }
}
