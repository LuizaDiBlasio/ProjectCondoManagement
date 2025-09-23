using ClassLibrary.DtoModels;
using ClassLibrary.DtoModelsMobile;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileCondoManagement.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MobileCondoManagement.Services.Interfaces;

namespace MobileCondoManagement.Models
{
    public partial class CondoMemberDashboardViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private CondoMemberDto? condoMember;

        [ObservableProperty]
        private FinancialAccountDto? financialAccount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;
        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string errorMessage;

        public ObservableCollection<UnitDto> Units { get; } = new();
        public ObservableCollection<MessageDto> Messages { get; } = new();
        public ObservableCollection<MeetingDto> Meetings { get; } = new();
        public ObservableCollection<PaymentDto> Payments { get; } = new();
        public ObservableCollection<OccurrenceDto> Occurrences { get; } = new();

        public CondoMemberDashboardViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Recebe query params (Shell) — invoca o load (fire-and-forget)
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
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }
               

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var dashboard = await _apiService.GetAsync<CondoMemberDashboardDto>($"api/CondoMembers/CondoMemberDashboard/{email}");

                if (dashboard == null)
                {
                    ErrorMessage = "Error retrieving info";
                    return;
                }

                CondoMember = dashboard.CondoMember;
                FinancialAccount = dashboard.FinancialAccount;

                Units.Clear();
                foreach (var u in dashboard.Units ?? Enumerable.Empty<UnitDto>())
                {
                    Units.Add(u);
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
                    

                Meetings.Clear();
                foreach (var mt in dashboard.Meetings ?? Enumerable.Empty<MeetingDto>())
                {
                    Meetings.Add(mt);
                }
                    

                Occurrences.Clear();
                foreach (var occ in dashboard.Occurrences ?? Enumerable.Empty<OccurrenceDto>())
                {
                    Occurrences.Add(occ);
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
