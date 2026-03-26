using MySql.Data.MySqlClient;
using Sistema_Bancario;
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
                if (producto == null)
                {
                    MessageBox.Show("Producto no válido");
                    return;
                }

                bool nuevoEstado = !producto.Disponible;

                string mensaje = nuevoEstado
                    ? "¿Deseas ACTIVAR este producto?"
                    : "¿Deseas DESACTIVAR este producto?";

                var confirm = MessageBox.Show(
                    mensaje,
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                    return;

                try
                {
                    var conn = ClassConexion.SQLConnection;

                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    string query = @"UPDATE Productos 
                         SET Disponible = @Disponible
                         WHERE IdProducto = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Disponible", nuevoEstado);
                    cmd.Parameters.AddWithValue("@Id", producto.IdProducto);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Disponibilidad actualizada correctamente");

                    CargarInventario(); // 🔥 refresca la tabla
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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
