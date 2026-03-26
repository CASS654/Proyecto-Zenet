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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para VentanaInventario.xaml
    /// </summary>
    public partial class VentanaInventario : UserControl
    {
        public VentanaInventario()
        {
            InitializeComponent();
            CargarInventario();
        }

        private void CargarInventario()
        {
            InventarioDAO dao = new InventarioDAO();
            var lista = dao.ObtenerInventario();
            ItemsProductos.ItemsSource = null;
            ItemsProductos.ItemsSource = lista;
        }

        private void BtnAcciones_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn == null)
            {
                MessageBox.Show("Botón es null");
                return;
            }

            var producto = btn.Tag as InventarioView;

            if (producto == null)
            {
                MessageBox.Show("Producto es NULL");
                return;
            }

            // 🔥 CREAR MENÚ
            ContextMenu menu = new ContextMenu();

            // OPCIÓN EDITAR
            MenuItem editar = new MenuItem { Header = "Editar" };
            editar.Click += (s, ev) =>
            {
                VentanaEditarProducto ventana = new VentanaEditarProducto(producto);
                ventana.ShowDialog();
                CargarInventario();
            };

            // OPCIÓN MERMA
            MenuItem merma = new MenuItem { Header = "Registrar Merma" };
            merma.Click += (s, ev) =>
            {
                VentanaMerma ventana = new VentanaMerma(producto);
                ventana.ShowDialog();
                CargarInventario();
            };

            // OPCIÓN DISPONIBILIDAD
            MenuItem disponibilidad = new MenuItem { Header = "Cambiar Disponibilidad" };
            disponibilidad.Click += (s, ev) =>
            {
                MessageBox.Show("Aquí irá la lógica de disponibilidad");
            };

            // AGREGAR AL MENÚ
            menu.Items.Add(editar);
            menu.Items.Add(merma);
            menu.Items.Add(disponibilidad);

            // MOSTRAR MENÚ
            btn.ContextMenu = menu;
            menu.IsOpen = true;
        }
    }
}
