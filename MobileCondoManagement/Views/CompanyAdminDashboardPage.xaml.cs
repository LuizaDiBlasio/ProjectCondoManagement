using ClassLibrary.DtoModelsMobile;
using MobileCondoManagement.Models;

namespace MobileCondoManagement.Views;

public partial class CompanyAdminDashboardPage : ContentPage
{
	public CompanyAdminDashboardPage(CompanyAdminDashboardViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}


