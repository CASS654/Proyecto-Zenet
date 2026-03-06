using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para Administrador.xaml
    /// </summary>
    public partial class AdministradorWindow : Window
    {
        private bool _isSidebarExpanded = true;
        private const double EXPANDED_WIDTH = 240;
        private const double COLLAPSED_WIDTH = 60;
        public AdministradorWindow()
        {
            InitializeComponent();
        }

        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            _isSidebarExpanded = !_isSidebarExpanded;

            if (_isSidebarExpanded)
            {
                // EXPANDIR
                SidebarColumn.Width = new System.Windows.GridLength(EXPANDED_WIDTH);

                // Mostrar elementos expandidos
                LogoContainer.Visibility = Visibility.Visible;
                LogoCollapsed.Visibility = Visibility.Collapsed;
                LogoText.Visibility = Visibility.Visible;
                MenuToggleButton.Visibility = Visibility.Visible;
                MenuToggleButtonCollapsed.Visibility = Visibility.Collapsed;
                ProfileFull.Visibility = Visibility.Visible;
                ProfileAvatar.Visibility = Visibility.Collapsed;
                TxtDashboard.Visibility = Visibility.Visible;
                TxtSales.Visibility = Visibility.Visible;
                TxtHistory.Visibility = Visibility.Visible;
                TxtInventory.Visibility = Visibility.Visible;
                TxtReports.Visibility = Visibility.Visible;
                TxtEmployees.Visibility = Visibility.Visible;
                TxtCustomers.Visibility = Visibility.Visible;
                TxtSuppliers.Visibility = Visibility.Visible;
                TxtAllData.Visibility = Visibility.Visible;
            }
            else
            {
                // COLAPSAR
                SidebarColumn.Width = new System.Windows.GridLength(COLLAPSED_WIDTH);

                // Mostrar elementos colapsados
                LogoContainer.Visibility = Visibility.Collapsed;
                LogoCollapsed.Visibility = Visibility.Visible;
                LogoText.Visibility = Visibility.Collapsed;
                MenuToggleButton.Visibility = Visibility.Collapsed;
                MenuToggleButtonCollapsed.Visibility = Visibility.Visible;
                ProfileFull.Visibility = Visibility.Collapsed;
                ProfileAvatar.Visibility = Visibility.Visible;
                TxtDashboard.Visibility = Visibility.Collapsed;
                TxtSales.Visibility = Visibility.Collapsed;
                TxtHistory.Visibility = Visibility.Collapsed;
                TxtInventory.Visibility = Visibility.Collapsed;
                TxtReports.Visibility = Visibility.Collapsed;
                TxtEmployees.Visibility = Visibility.Collapsed;
                TxtCustomers.Visibility = Visibility.Collapsed;
                TxtSuppliers.Visibility = Visibility.Collapsed;
                TxtAllData.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

}
