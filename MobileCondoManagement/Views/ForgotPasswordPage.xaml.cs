
using MobileCondoManagement.Models;


namespace MobileCondoManagement.Views
{
    
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
        {
            InitializeComponent();

            this.BindingContext = viewModel;
        }
    }
}