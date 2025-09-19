using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services.Interfaces;
using System.Threading.Tasks;

namespace MobileCondoManagement.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private bool isSuccess;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))] 
        private bool isBusy;
        public bool IsNotBusy => !IsBusy; 

        public ForgotPasswordViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private async Task SendResetLinkAsync()
        {
            IsBusy = true;
            Message = string.Empty;
            IsSuccess = false;

            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    Message = "Please enter your email address.";
                    return;
                }

                // A sua API espera uma string no corpo da requisição.
                // O método RequestForgotPasswordAsync precisa ser adaptado para isso.
                var result = await _apiService.RequestForgotPasswordAsync(Email);

                if (result.IsSuccess)
                {
                    IsSuccess = true;
                    Message = result.Message;
                }
                else
                {
                    Message = result.Message ?? "An error occurred. Please try again.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
