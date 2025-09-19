using MobileCondoManagement.Models;

namespace MobileCondoManagement.Views
{
    public partial class CondoManagerDashboardPage : ContentPage
    {
        public CondoManagerDashboardPage(CondoMemberDashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }
}

