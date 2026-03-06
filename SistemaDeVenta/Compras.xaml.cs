using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SistemaDeVenta
{
    public partial class Compras : UserControl
    {
        // --- OBJETOS SELECCIONADOS ---
        private ClassProducto.Producto productoSeleccionado = null;
        private Proveedor proveedorSeleccionado = null;

        // --- LISTAS ---
        private List<ClassProducto.Producto> listaProductos = new List<ClassProducto.Producto>();
        private List<Proveedor> listaProveedores = new List<Proveedor>();
        private List<CompraItem> carritoCompras = new List<CompraItem>();

        public Compras()
        {
            InitializeComponent();

            // Habilitar eliminación desde los botones del DataGrid
            TablaCompras.AddHandler(Button.ClickEvent, new RoutedEventHandler(BtnEliminarFila_Click));
        }

        // ===========================================================
        //                    CARGAR PRODUCTOS
        // ===========================================================
        private void CargarProductos()
        {
            listaProductos.Clear();

            ClassTest db = new ClassTest();
            DataTable dt = db.ListarRegistros("SELECT IdProducto, Nombre, PrecioCompra, PrecioVenta FROM Inventario WHERE Disponible = 1");

            foreach (DataRow row in dt.Rows)
            {
                listaProductos.Add(new ClassProducto.Producto
                {
                    IdProducto = Convert.ToInt32(row["IdProducto"]),
                    Nombre = row["Nombre"].ToString(),
                    PrecioCompra = Convert.ToDecimal(row["PrecioCompra"]),
                    PrecioVenta = row.Table.Columns.Contains("PrecioVenta") ? Convert.ToDecimal(row["PrecioVenta"]) : 0m
                });
            }
        }


        // ===========================================================
        //                    CARGAR PROVEEDORES
        // ===========================================================
        private void CargarProveedores()
        {
            listaProveedores.Clear();

            ClassTest db = new ClassTest();
            DataTable dt = db.ListarRegistros("SELECT IdProveedor, Nombre FROM Proveedores");


            foreach (DataRow row in dt.Rows)
            {
                listaProveedores.Add(new Proveedor
                {
                    IdProveedor = Convert.ToInt32(row["IdProveedor"]),
                    Nombre = row["Nombre"].ToString()
                });
            }
        }

        // ===========================================================
        //                     ELIMINAR FILA
        // ===========================================================
        private void BtnEliminarFila_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button btn && btn.DataContext is CompraItem item)
            {
                carritoCompras.Remove(item);

                TablaCompras.ItemsSource = null;
                TablaCompras.ItemsSource = carritoCompras;

                ActualizarTotal();
            }
        }

        // ===========================================================
        //                     TOTAL
        // ===========================================================
        private void ActualizarTotal()
        {
            decimal total = carritoCompras.Sum(x => x.Subtotal);
            txtTotal.Text = total.ToString("0.00");
        }

        // ===========================================================
        //                   REGISTRAR COMPRA
        // ===========================================================

        private int ObtenerIdUsuario()
        {
            return 1; // cuando tengas login lo reemplazas
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CargarProductos();

            // MAPEAMOS SOLO LO QUE WINDOW1 NECESITA
            var listaParaWindow = listaProductos
                .Select(p => new Producto   // <-- el Producto que Window1 conoce
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    PrecioVenta = p.PrecioVenta
                })
                .ToList();

            Window1 ventana = new Window1(listaParaWindow)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
            {
                int idProducto = ventana.ProductoSeleccionadoId;

                // Usamos tu clase real: ClassProducto.Producto
                productoSeleccionado =
                    listaProductos.FirstOrDefault(p => p.IdProducto == idProducto);

                if (productoSeleccionado != null)
                {
                    txtProducto.Text = productoSeleccionado.Nombre;
                    txtPrecioCompra.Text = productoSeleccionado.PrecioCompra.ToString("0.00");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CargarProveedores();

            SelectorProveedor ventana = new SelectorProveedor(listaProveedores)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
            {
                proveedorSeleccionado = listaProveedores
                    .FirstOrDefault(x => x.IdProveedor == ventana.ProveedorSeleccionadoId);
                if (proveedorSeleccionado != null)
                {
                    txtRazonSocial.Text = proveedorSeleccionado.Nombre; // ✔ MOSTRAR NOMBRE DEL PROVEEDOR
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            if (proveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            if (!decimal.TryParse(cbCantidad.Text, out decimal cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            if (!decimal.TryParse(txtPrecioCompra.Text, out decimal precio))
            {
                MessageBox.Show("Precio de compra inválido.");
                return;
            }

            CompraItem item = new CompraItem
            {
                IdProducto = productoSeleccionado.IdProducto,
                Producto = productoSeleccionado.Nombre,
                IdProveedor = proveedorSeleccionado.IdProveedor,
                Cantidad = cantidad,
                PrecioCompra = precio
            };

            carritoCompras.Add(item);

            TablaCompras.ItemsSource = null;
            TablaCompras.ItemsSource = carritoCompras;

            ActualizarTotal();

            productoSeleccionado = null;
            txtProducto.Text = "";
            txtPrecioCompra.Text = "";
            cbCantidad.Text = "1";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (carritoCompras.Count == 0)
            {
                MessageBox.Show("No hay productos en la compra.");
                return;
            }

            if (proveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            try
            {
                int idUsuario = ObtenerIdUsuario();

                // ===============================
                //     INSERTAR COMPRA
                // ===============================
                string sqlCompra = @"
        INSERT INTO Compras (Fecha, IdProveedor, IdUsuario, Total)
        VALUES (NOW(), @prov, @u, @t)";

                using (var cmd = new MySqlCommand(sqlCompra, ClassConexion.SQLConnection))
                {
                    cmd.Parameters.AddWithValue("@prov", proveedorSeleccionado.IdProveedor);
                    cmd.Parameters.AddWithValue("@u", idUsuario);
                    cmd.Parameters.AddWithValue("@t", decimal.Parse(txtTotal.Text));
                    cmd.ExecuteNonQuery();
                }

                // OBTENER ID COMPRA
                int idCompra = Convert.ToInt32(
                    new MySqlCommand("SELECT LAST_INSERT_ID()", ClassConexion.SQLConnection).ExecuteScalar()
                );

                // ======================================
                //     INSERTAR DETALLES E INVENTARIO
                // ======================================
                foreach (var item in carritoCompras)
                {
                    string sqlDet = @"
            INSERT INTO HistorialCompras
            (IdCompra, IdProducto, Cantidad, PrecioUnitario, Subtotal)
            VALUES (@c, @p, @cant, @precio, @sub)";

                    using (var cmd = new MySqlCommand(sqlDet, ClassConexion.SQLConnection))
                    {
                        cmd.Parameters.AddWithValue("@c", idCompra);
                        cmd.Parameters.AddWithValue("@p", item.IdProducto);
                        cmd.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmd.Parameters.AddWithValue("@precio", item.PrecioCompra);
                        cmd.Parameters.AddWithValue("@sub", item.Subtotal);
                        cmd.ExecuteNonQuery();
                    }

                    // SUMAR STOCK
                    string sqlStock = "UPDATE Inventario SET Stock = Stock + @c WHERE IdProducto = @p";
                    using (var cmd = new MySqlCommand(sqlStock, ClassConexion.SQLConnection))
                    {
                        cmd.Parameters.AddWithValue("@c", item.Cantidad);
                        cmd.Parameters.AddWithValue("@p", item.IdProducto);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Compra registrada exitosamente.");

                carritoCompras.Clear();
                TablaCompras.ItemsSource = null;
                txtTotal.Text = "0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar compra: " + ex.Message);
            }
        }
    }

    // ===========================================================
    //            MODELO USADO POR EL DATAGRID
    // ===========================================================
    public class CompraItem
    {
        public int IdProducto { get; set; }
        public string Producto { get; set; }
        public int IdProveedor { get; set; }
        public string Proveedor { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Subtotal => PrecioCompra * Cantidad;
    }

    // ===========================================================
    //                PROVEEDOR (modelo real)
    // ===========================================================
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
    }

}
