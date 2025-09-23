using MobileCondoManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileCondoManagement.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SysAdminDashboardPage : ContentPage
    {
        public SysAdminDashboardPage(SysAdminDashboardViewModel viewModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}