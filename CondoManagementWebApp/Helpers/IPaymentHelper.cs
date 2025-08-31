using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoManagementWebApp.Helpers
{
    public interface IPaymentHelper
    {
        List<SelectListItem> GetPaymentMethodsList();


        string GetSelectedPaymentMethod(int id);

        List<SelectListItem> GetBeneficiaryTypesList(string userRole);

        string GetBeneficiaryType(int id, string userRole);

    }
}
