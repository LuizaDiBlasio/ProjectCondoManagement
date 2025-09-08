using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CondoManagementWebApp.Helpers
{
    public class PaymentHelper : IPaymentHelper 
    {
        public List<SelectListItem> GetPaymentMethodsList()
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem{Value = "1", Text = "MbWay"},
                new SelectListItem{Value = "2", Text = "Credit card"},
                new SelectListItem{Value = "3", Text = "Omah Wallet"}
            };
            return selectList;

        }


        public string GetSelectedPaymentMethod(int id)
        {
            var paymentMethodList = GetPaymentMethodsList();

            var selectedPaymentMethod = paymentMethodList.FirstOrDefault(pm => pm.Value == id.ToString());

            if (selectedPaymentMethod != null)
            {
                return selectedPaymentMethod.Text.ToString();
            }

            return string.Empty;
        }

        public List<SelectListItem> GetBeneficiaryTypesList(string userRole)
        {
           var selectList = new List<SelectListItem>();

            if (userRole == "CondoMember")
            {
                selectList.Add(new SelectListItem { Value = "1", Text = "Condominium" });
                selectList.Add(new SelectListItem { Value = "3", Text = "External recipient" });
            }

            if (userRole == "CondoManager")
            {
                selectList.Add(new SelectListItem { Value = "2", Text = "Company" });
                selectList.Add(new SelectListItem { Value = "3", Text = "External recipient" });

            }

            
            return selectList;

        }

        public string GetBeneficiaryType(int id, string userRole)
        {
            var beneficiaryTypesList = GetBeneficiaryTypesList(userRole);

            var selectedBeneficiaryType = beneficiaryTypesList.FirstOrDefault(pm => pm.Value == id.ToString());

            if (selectedBeneficiaryType != null)
            {
                return selectedBeneficiaryType.Text.ToString();
            }

            return string.Empty;
        }

    }
}
