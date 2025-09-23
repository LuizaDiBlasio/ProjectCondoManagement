using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services.Interfaces;
using System.Threading.Tasks;

namespace MobileCondoManagement.Models
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


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowSignInButton))]
        private bool showSendButton = true;

        public bool ShowSignInButton => !ShowSendButton;

        public ForgotPasswordViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private async Task SendResetLinkAsync()
        {
            IsBusy = true;
            Message = string.Empty; // Change this line
            IsSuccess = false;

            OnPropertyChanged(nameof(Message));

            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    Message = "Please enter your email address.";
                    return;
                }

                var result = await _apiService.RequestForgotPasswordAsync(Email);

                if (result.IsSuccess)
                {
                    IsSuccess = true;
                    Message = result.Message;
                    ShowSendButton = false;
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

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

