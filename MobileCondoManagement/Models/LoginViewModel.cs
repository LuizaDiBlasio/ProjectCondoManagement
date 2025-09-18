using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services;

namespace MobileCondoManagement.Models
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;
        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var loginResult = await _apiService.RequestLoginAsync(Email, Password);

                if (loginResult.IsSuccess)
                {
                    // Lógica de armazenamento do token
                    await SecureStorage.Default.SetAsync("auth_token", loginResult.Token);

                    //  página principal
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    ErrorMessage = loginResult.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            // Lógica de navegação para a página de redefinição de senha
            await Shell.Current.GoToAsync("//ForgotPasswordPage");
        }
    }
}
