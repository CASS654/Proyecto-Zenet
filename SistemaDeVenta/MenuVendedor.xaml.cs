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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para MenuVendedor.xaml
    /// </summary>
    public partial class MenuVendedor : Window
    {
        public MenuVendedor()
        {
            InitializeComponent();
        }

        private void Ventas_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Ventas();
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Abrir la ventana de Login
                Login login = new Login();
                login.Show();

                // Cerrar la ventana actual (Menu)
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar sesión: " + ex.Message);
            }
        }
    }
}
