using MobileCondoManagement.Models;

namespace MobileCondoManagement.Views
{
    public partial class CondoManagerDashboardPage : ContentPage
    {
        public CondoManagerDashboardPage(CondoManagerDashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
