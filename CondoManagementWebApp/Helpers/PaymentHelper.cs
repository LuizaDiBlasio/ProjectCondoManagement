using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;
using Vereyon.Web;

namespace CondoManagementWebApp.Helpers
{
    public class PaymentHelper : IPaymentHelper 
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;

        public PaymentHelper(IApiCallService apiCallService, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
        }


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



        public async Task<SelectList> GetCondosToSelectListAsync()
        {
            var condos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");

            var condoSelectListItems = _converterHelper.ToCondosSelectList(condos);

            var condoSelectList = new SelectList(condoSelectListItems, "Value", "Text");

            return condoSelectList;
        }

        public async Task<List<SelectListItem>> GetExpenseTypesAsync()
        {
            return await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
        }


     

    }
}
