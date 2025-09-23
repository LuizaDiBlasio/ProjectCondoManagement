using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services;
using MobileCondoManagement.Services.Interfaces;

namespace MobileCondoManagement.Models
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

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

        public LoginViewModel(IApiService apiService)
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

                    // Salvar o e-mail do usuário
                    await SecureStorage.Default.SetAsync("user_email", Email);

                    //Reencaminhamento
                    switch (loginResult.UserRole)
                    {
                        case "SysAdmin":
                            await Shell.Current.GoToAsync("SysAdminDashboardPage");
                            break;
                        case "CompanyAdmin":
                            await Shell.Current.GoToAsync("//CompanyAdminDashboardPage");
                            break;
                        case "CondoMember":
                            await Shell.Current.GoToAsync("//CondoMemberDashboardPage");
                            break;
                        case "CondoManager":
                            await Shell.Current.GoToAsync("//CondoManagerDashboardPage");
                            break;
                    }
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
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }
    }
}
