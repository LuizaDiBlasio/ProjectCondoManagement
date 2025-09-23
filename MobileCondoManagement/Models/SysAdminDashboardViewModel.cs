using ClassLibrary.DtoModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileCondoManagement.Models
{
    public partial class SysAdminDashboardViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        // Propriedades para a UI
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;
        public bool IsNotBusy => !IsBusy;

        

        [ObservableProperty]
        private string errorMessage;

        // Coleções para os dados da dashboard
        public ObservableCollection<UserDto> CondoMembers { get; } = new();
        public ObservableCollection<UserDto> CompanyAdmins { get; } = new();
        public ObservableCollection<UserDto> CondoManagers { get; } = new();

        //SysAdmin
        [ObservableProperty]
        private UserDto? sysAdmin;


        public SysAdminDashboardViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _ = LoadDashboardAsync();
        }

        

        [RelayCommand]
        public async Task LoadDashboardAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var userEmail = await LoadUserEmail(); 

                var condoMembersTask = _apiService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoMember");
                var companyAdminsTask = _apiService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CompanyAdmin");
                var condoManagersTask = _apiService.GetAsync<IEnumerable<UserDto>>($"api/Account/GetUsersWithCompany?role=CondoManager");
                var sysAdminTask = _apiService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={userEmail}");

                await Task.WhenAll(condoMembersTask, companyAdminsTask, condoManagersTask, sysAdminTask);

                // Atribuir os resultados 
                var condoMembers = await condoMembersTask;
                var companyAdmins = await companyAdminsTask;
                var condoManagers = await condoManagersTask;

                if(sysAdminTask != null)
                {
                    SysAdmin = await sysAdminTask;
                }
                 
                // Limpar e popular as listas 
                CondoMembers.Clear();
                foreach (var user in condoMembers)
                {
                    CondoMembers.Add(user);
                }

                CompanyAdmins.Clear();
                foreach (var user in companyAdmins)
                {
                    CompanyAdmins.Add(user);
                }

                CondoManagers.Clear();
                foreach (var user in condoManagers)
                {
                    CondoManagers.Add(user);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dashboard: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]

        private async Task<string> LoadUserEmail()
        {
            return await SecureStorage.Default.GetAsync("user_email");

        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            // Limpar o token de autenticação e o e-mail do usuário
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_email");

            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}