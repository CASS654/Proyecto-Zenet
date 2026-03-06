using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using static SistemaDeVenta.ClassProducto;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para Ventas.xaml
    /// </summary>
    public partial class Ventas : UserControl
    {
        private Producto productoSeleccionado = null;

        private List<Producto> listaProductos = new List<Producto>();
        private List<CarritoItem> carrito = new List<CarritoItem>();
        public Ventas()
        {
            InitializeComponent();
            TablaVentas.AddHandler(Button.ClickEvent, new RoutedEventHandler(BtnEliminarFila_Click));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CargarProductos();

            Window1 ventana = new Window1(listaProductos)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
            {
                int idProducto = ventana.ProductoSeleccionadoId;
                productoSeleccionado = listaProductos.First(p => p.IdProducto == idProducto);

                // Solo actualizar los TextBox
                txtProducto.Text = productoSeleccionado.Nombre;
                txtPrecio.Text = productoSeleccionado.PrecioVenta.ToString("0.00");
                txtStock.Text = "Disponible"; // o stock real si lo tienes
            }
        }
        private void CargarProductos()
        {
            listaProductos.Clear();
            ClassTest db = new ClassTest();
            DataTable dt = db.ListarRegistros("SELECT * FROM Inventario WHERE Disponible=1");

            foreach (DataRow row in dt.Rows)
            {
                listaProductos.Add(new Producto
                {
                    IdProducto = Convert.ToInt32(row["IdProducto"]),
                    Nombre = row["Nombre"].ToString(),
                    PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),

                });
            }
        }
        private void AgregarAlCarrito(int idProducto)
        {
            var producto = listaProductos.First(p => p.IdProducto == idProducto);

            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = producto.IdProducto,
                    Producto = producto.Nombre,
                    PrecioVenta = producto.PrecioVenta,
                    Cantidad = 1
                });
            }
            TablaVentas.ItemsSource = null;
            TablaVentas.ItemsSource = carrito;
            ActualizarTotal();

            // Actualizar TextBox
            txtProducto.Text = producto.Nombre;
            txtPrecio.Text = producto.PrecioVenta.ToString("0.00");
            txtStock.Text = "Disponible"; // opcional, puedes mostrar stock real si lo agregas
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto primero.");
                return;
            }

            // Tomar la cantidad del TextBox
            decimal cantidad = 0;
            if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.");
                return;
            }

            // Verificar si el producto ya existe en el carrito
            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == productoSeleccionado.IdProducto);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = productoSeleccionado.IdProducto,
                    Producto = productoSeleccionado.Nombre,
                    PrecioVenta = productoSeleccionado.PrecioVenta,
                    Cantidad = cantidad
                });
            }

            // Actualizar DataGrid
            TablaVentas.ItemsSource = null;
            TablaVentas.ItemsSource = carrito;
            ActualizarTotal();

            // Limpiar selección
            productoSeleccionado = null;
            txtProducto.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            txtCantidad.Text = "1"; // reset
        }

        private void BtnEliminarFila_Click(object sender, RoutedEventArgs e)
        {
            // Revisar que el origen del click sea un botón
            if (e.OriginalSource is Button btn)
            {
                // Obtener el item de la fila
                if (btn.DataContext is CarritoItem item)
                {
                    carrito.Remove(item);

                    // Refrescar DataGrid
                    TablaVentas.ItemsSource = null;
                    TablaVentas.ItemsSource = carrito;

                    ActualizarTotal();
                }
            }
        }
        private void ActualizarTotal()
        {
            decimal total = carrito.Sum(item => item.Subtotal);
            txtTotal.Text = total.ToString("0.00");
        }
        private void ActualizarCambio()
        {
            decimal total = 0;
            decimal pagaCon = 0;

            // Intentar leer el total
            decimal.TryParse(txtTotal.Text, out total);

            // Intentar leer el monto que paga el cliente
            decimal.TryParse(txtPagaCon.Text, out pagaCon);

            // Calcular cambio
            decimal cambio = pagaCon - total;

            // No permitir que sea negativo
            txtCambio.Text = (cambio >= 0 ? cambio : 0).ToString("0.00");
        }

        // Clase interna para los productos del carrito
        public class CarritoItem
        {
            public int IdProducto { get; set; }
            public string Producto { get; set; }
            public decimal PrecioVenta { get; set; }
            public decimal Cantidad { get; set; }
            public decimal Subtotal => PrecioVenta * Cantidad;
        }

        private void txtPagaCon_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarCambio();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RegistrarVenta();
        }
        private void RegistrarVenta()
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("No hay productos en el carrito.");
                return;
            }

            try
            {
                int idUsuario = ObtenerIdUsuario();

                // 1. Insertar venta
                string insertVenta = "INSERT INTO Ventas (Fecha, IdUsuario, Total) VALUES (NOW(), @IdUsuario, @Total);";
                MySqlCommand cmdVenta = new MySqlCommand(insertVenta, ClassConexion.SQLConnection);
                cmdVenta.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmdVenta.Parameters.AddWithValue("@Total", decimal.Parse(txtTotal.Text));
                cmdVenta.ExecuteNonQuery();

                // 2. Obtener el ID de la venta recién insertada
                string selectId = "SELECT LAST_INSERT_ID();";
                MySqlCommand cmdId = new MySqlCommand(selectId, ClassConexion.SQLConnection);
                int idVenta = Convert.ToInt32(cmdId.ExecuteScalar());

                // 3. Insertar detalle de cada producto
                foreach (var item in carrito)
                {
                    string insertDetalle = @"
                INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                VALUES (@IdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal);";

                    MySqlCommand cmdDetalle = new MySqlCommand(insertDetalle, ClassConexion.SQLConnection);
                    cmdDetalle.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmdDetalle.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", item.PrecioVenta);
                    cmdDetalle.Parameters.AddWithValue("@Subtotal", item.Subtotal);

                    cmdDetalle.ExecuteNonQuery();
                }

                MessageBox.Show("Venta registrada correctamente.");

                // Limpiar carrito y TextBox
                carrito.Clear();
                TablaVentas.ItemsSource = null;
                txtTotal.Text = "";
                txtPagaCon.Text = "";
                txtCambio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta: " + ex.Message);
            }
        }

        private int ObtenerIdUsuario()
        {
            // Por ahora devuelve un Id fijo (1)
            // Más adelante reemplazar con el Id del usuario que haya iniciado sesión
            return 1;
        }


    }

}

