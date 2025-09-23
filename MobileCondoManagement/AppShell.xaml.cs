using MobileCondoManagement.Views;

namespace MobileCondoManagement
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("SysAdminDashboardPage", typeof(SysAdminDashboardPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        }
    }
}
