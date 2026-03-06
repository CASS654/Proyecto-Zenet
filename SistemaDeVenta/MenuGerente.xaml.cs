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
    /// Lógica de interacción para MenuGerente.xaml
    /// </summary>
    public partial class MenuGerente : Window
    {
        public MenuGerente()
        {
            InitializeComponent();
        }

        private void Inventario_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Inventario();
        }

        private void Compras_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Compras();
        }

        private void Proveedores_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Proveedores();
        }

        private void HistorialVenta_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new HistorialVenta();
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
