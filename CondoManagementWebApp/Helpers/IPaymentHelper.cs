using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Vereyon.Web;

namespace CondoManagementWebApp.Helpers
{
    public interface IPaymentHelper
    {
        List<SelectListItem> GetPaymentMethodsList();


        string GetSelectedPaymentMethod(int id);

        List<SelectListItem> GetBeneficiaryTypesList(string userRole);

        string GetBeneficiaryType(int id, string userRole);


        Task<SelectList> GetCondosToSelectListAsync();

        Task<List<SelectListItem>> GetExpenseTypesAsync();

    }
}
