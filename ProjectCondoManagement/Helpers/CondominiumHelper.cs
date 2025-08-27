using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Users;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Helpers
{
    public class CondominiumHelper : ICondominiumHelper
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly DataContextUsers _dataContextUsers;
        private readonly IConverterHelper _converterHelper;

        public CondominiumHelper(ICompanyRepository companyRepository, DataContextUsers dataContextUsers, IConverterHelper converterHelper)
        {
            _companyRepository = companyRepository;
            _dataContextUsers = dataContextUsers;
            _converterHelper = converterHelper;
        }

        public async Task<Response<object>> LinkCompanyToCondominiumAsync(CondominiumDto condominium)
        {
            var company = await _companyRepository.GetByIdAsync(condominium.CompanyId.Value, _dataContextUsers);

            if (company == null)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Company with ID {condominium.CompanyId} not found."
                };
            }

            condominium.Company = _converterHelper.ToCompanyDto(company);

            if (condominium.Company == null)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = "Failed to convert company data."
                };
            }

            return new Response<object>
            {
                IsSuccess = true,
                Message = "Linked Successfully"
            };
        }


    }
}
