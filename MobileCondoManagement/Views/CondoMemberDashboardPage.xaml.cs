using MobileCondoManagement.Models;

namespace MobileCondoManagement.Views
{
    public partial class CondoMemberDashboardPage : ContentPage
    {
        public CondoMemberDashboardPage(CondoMemberDashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }
}

