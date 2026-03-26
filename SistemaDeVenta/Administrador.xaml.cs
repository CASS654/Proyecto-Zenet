using SistemaDeVenta.Views;
using SistemaDeVentaPrueba;
using SistemaDeVentas.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SistemaDeVenta
{
    public partial class AdministradorWindow : Window
    {
        private bool _isSidebarExpanded = true;

        private const double EXPANDED_WIDTH = 240;
        private const double COLLAPSED_WIDTH = 60;

        public AdministradorWindow()
        {
            InitializeComponent();
        }

        private void AnimateSidebar(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(250),
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };

            animation.CurrentTimeInvalidated += (s, e) =>
            {
                AnimationClock clock = s as AnimationClock;
                if (clock?.CurrentProgress != null)
                {
                    double value = from + ((to - from) * clock.CurrentProgress.Value);
                    SidebarColumn.Width = new GridLength(value);
                }
            };

            animation.Completed += (s, e) =>
            {
                SidebarColumn.Width = new GridLength(to);
            };

            this.BeginAnimation(WidthProperty, animation);
        }
        

        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }
        }
        private void ExpandSidebar()
        {
            if (!_isSidebarExpanded)
            {
                _isSidebarExpanded = true;

                AnimateSidebar(COLLAPSED_WIDTH, EXPANDED_WIDTH);

                LogoContainer.Visibility = Visibility.Visible;
                LogoCollapsed.Visibility = Visibility.Collapsed;
                LogoText.Visibility = Visibility.Visible;
                MenuToggleButton.Visibility = Visibility.Visible;
                MenuToggleButtonCollapsed.Visibility = Visibility.Collapsed;
                ProfileFull.Visibility = Visibility.Visible;
                ProfileAvatar.Visibility = Visibility.Collapsed;

                TxtDashboard.Visibility = Visibility.Visible;
                TxtSales.Visibility = Visibility.Visible;
                TxtInventory.Visibility = Visibility.Visible;
                TxtEmployees.Visibility = Visibility.Visible;
                TxtCustomers.Visibility = Visibility.Visible;
                TxtSuppliers.Visibility = Visibility.Visible;
                TxtAllData.Visibility = Visibility.Visible;
            }
        }
        private void CollapseSidebar()
        {
            _isSidebarExpanded = false;

            AnimateSidebar(EXPANDED_WIDTH, COLLAPSED_WIDTH);

            LogoContainer.Visibility = Visibility.Collapsed;
            LogoCollapsed.Visibility = Visibility.Visible;
            LogoText.Visibility = Visibility.Collapsed;
            MenuToggleButton.Visibility = Visibility.Collapsed;
            MenuToggleButtonCollapsed.Visibility = Visibility.Visible;
            ProfileFull.Visibility = Visibility.Collapsed;
            ProfileAvatar.Visibility = Visibility.Visible;

            TxtDashboard.Visibility = Visibility.Collapsed;
            TxtSales.Visibility = Visibility.Collapsed;
            TxtInventory.Visibility = Visibility.Collapsed;
            TxtEmployees.Visibility = Visibility.Collapsed;
            TxtCustomers.Visibility = Visibility.Collapsed;
            TxtSuppliers.Visibility = Visibility.Collapsed;
            TxtAllData.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Application.Current.Shutdown();
            BusquedaProducto ventanaBusqueda = new BusquedaProducto();

            // ShowDialog hace que la ventana sea modal
            ventanaBusqueda.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 1. Instanciamos la ventana de búsqueda
    BusquedaProducto ventanaBusqueda = new BusquedaProducto();

            // 2. Establecemos quién es la ventana "padre" para que aparezca centrada sobre el Administrador
            ventanaBusqueda.Owner = this;

            // 3. La abrimos como Modal (bloquea el fondo hasta que se cierre)
            ventanaBusqueda.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }

            MainContent.Content = new DashboardControl();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }
            MainContent.Content = new UsuariosView();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }
            MainContent.Content = new ProveedoresNUEVO();
        }
    }
}