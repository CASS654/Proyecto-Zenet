using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SistemaDeVenta.Classusuarios;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class Menu : Window
    {
        int IdSeleccionado = 0;
        public Menu()
        {
            InitializeComponent();
        }
                
        private void Usuarios_Click(object sender, RoutedEventArgs e)
        {
            Contenedor .Content = new Usuarios();
        }

        private void Inventario_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Inventario();
        }

        private void Ventas_Click(object sender, RoutedEventArgs e)
        {
            Contenedor.Content = new Ventas();
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
            Contenedor.Content = new pagina_prueba();
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
