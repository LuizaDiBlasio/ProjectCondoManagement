using ClassLibrary.DtoModels;
using ClassLibrary.DtoModelsMobile;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MobileCondoManagement.Models
{
    public partial class CondoManagerDashboardViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private UserDto? condoManager;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;
        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string errorMessage;

        // Coleções específicas do manager
        public ObservableCollection<MeetingDto> Meetings { get; } = new();
        public ObservableCollection<PaymentDto> Payments { get; } = new();
        public ObservableCollection<MessageDto> Messages { get; } = new();
        public ObservableCollection<OccurrenceDto> Occurrences { get; } = new();
        public ObservableCollection<CondominiumDto> Condominiums { get; } = new();


        public CondoManagerDashboardViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("email", out var emailObj) && emailObj is string email)
            {
                _ = LoadDashboardAsync(email);
            }
        }

        [RelayCommand]
        public async Task LoadDashboardAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var dashboard = await _apiService.GetAsync<CondoManagerDashboardDto>(
                    $"api/Account/CondoManagerDashboard/{email}");

                 if (dashboard == null)
                {
                    ErrorMessage = "Error retrieving info";
                    return;
                }

                CondoManager = dashboard.CondoManager;

                Meetings.Clear();
                foreach (var mt in dashboard.Meetings ?? Enumerable.Empty<MeetingDto>()) 
                {
                   Meetings.Add(mt);
                }
                    

                Payments.Clear();
                foreach (var p in dashboard.Payments ?? Enumerable.Empty<PaymentDto>())
                {
                   Payments.Add(p);
                }
                    

                Messages.Clear();
                foreach (var m in dashboard.Messages ?? Enumerable.Empty<MessageDto>())
                {
                    Messages.Add(m);
                }
                    

                Occurrences.Clear();
                foreach (var o in dashboard.Occurrences ?? Enumerable.Empty<OccurrenceDto>())
                {
                    Occurrences.Add(o);
                }

                Condominiums.Clear();
                foreach (var c in dashboard.Condominiums ?? Enumerable.Empty<CondominiumDto>())
                {
                    Condominiums.Add(c);
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
        private async Task LogoutAsync()
        {
            // Limpar credenciais
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_email");

            // Redirecionar para a tela de Login
            await Shell.Current.GoToAsync("//LoginPage");
        }


    }
}
