using Sistema_Bancario;
using SistemaDeVentaPrueba;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SistemaDeVenta.ClassProveedores;

namespace SistemaDeVenta
{
    public partial class comprasbueno : UserControl
    {
        private bool modoCantidad = false;
        private string bufferCantidad = "";
        private int cantidadActual = 1;
        public ObservableCollection<ProductoCompra> carrito { get; set; }
            = new ObservableCollection<ProductoCompra>();
        private List<Proveedores1> listaProveedores = new List<Proveedores1>();
        private Proveedores1 proveedorSeleccionado;


        public comprasbueno()
        {
            InitializeComponent();
            ItemsCompra.ItemsSource = carrito;

            CargarProveedores(); // 🔥 AQUI

            Loaded += (s, e) =>
            {
                TxtBuscar.Focus();
                Keyboard.Focus(TxtBuscar);
            };
        }
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!TxtBuscar.IsFocused)
            {
                TxtBuscar.Focus();
                Keyboard.Focus(TxtBuscar);
            }
        }

        // 🔹 ABRIR BUSCADOR (NO SE TOCA)
        private void AbrirBuscador()
        {
            BusquedaProducto buscador = new BusquedaProducto();

            if (buscador.ShowDialog() == true)
            {
                if (buscador.ProductoSeleccionado != null)
                {
                    var productoCompra = ObtenerProductoCompra(buscador.ProductoSeleccionado.Id);

                    if (productoCompra != null)
                        AgregarProducto(productoCompra, cantidadActual);
                        ResetCantidad();
                }
            }
        }

        private void CbProveedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbProveedores.SelectedItem is Proveedores1 proveedor)
            {
                proveedorSeleccionado = proveedor;

                // 🔥 CAMBIO VISUAL
                txtEstadoProveedor.Visibility = Visibility.Collapsed;
                txtNombreProveedor.Visibility = Visibility.Visible;

                txtNombreProveedor.Text = proveedor.Nombre;
            }
        }

        // 🔹 CONSULTA A BD PARA COMPRAS
        private ProductoCompra ObtenerProductoCompra(string id)
        {
            try
            {
                ClassTest db = new ClassTest();

                string sql = $@"
                    SELECT p.IdProducto, p.Nombre, p.PrecioCompra
                    FROM Productos p
                    WHERE p.IdProducto = {id}";

                DataTable dt = db.ListarRegistros(sql);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    return new ProductoCompra
                    {
                        Id = row["IdProducto"].ToString(),
                        Nombre = row["Nombre"].ToString(),
                        Costo = Convert.ToDecimal(row["PrecioCompra"]),
                        Cantidad = 1
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener producto: " + ex.Message);
            }

            return null;
        }

        // 🔹 AGREGAR AL CARRITO
        private void AgregarProducto(ProductoCompra producto, int cantidad)
        {
            var existente = carrito.FirstOrDefault(p => p.Id == producto.Id);

            if (existente != null)
                existente.Cantidad += cantidad;
            else
            {
                producto.Cantidad = cantidad;
                carrito.Add(producto);
            }

            ActualizarTotales();
        }
        private void CargarProveedores()
        {
            try
            {
                ClassTest obj = new ClassTest();
                DataTable registros = obj.ListarRegistros("SELECT * FROM Proveedores");

                listaProveedores.Clear();

                foreach (DataRow row in registros.Rows)
                {
                    listaProveedores.Add(new Proveedores1
                    {
                        IdProveedor = row["IdProveedor"] == DBNull.Value ? 0 : Convert.ToInt32(row["IdProveedor"]),
                        Nombre = row["Nombre"] == DBNull.Value ? "" : row["Nombre"].ToString()
                    });
                }

                CbProveedores.ItemsSource = listaProveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtBuscar.Text))
                TxtPlaceholder.Visibility = Visibility.Visible;
            else
                TxtPlaceholder.Visibility = Visibility.Collapsed;
        }

        // 🔹 SUMAR
        private void BtnSumar_Click(object sender, MouseButtonEventArgs e)
        {
            var producto = (sender as FrameworkElement).DataContext as ProductoCompra;
            if (producto != null)
            {
                producto.Cantidad++;
                ActualizarTotales();
            }
        }

        // 🔹 RESTAR
        private void BtnRestar_Click(object sender, MouseButtonEventArgs e)
        {
            var producto = (sender as FrameworkElement).DataContext as ProductoCompra;

            if (producto != null)
            {
                if (producto.Cantidad > 1)
                    producto.Cantidad--;
                else
                    carrito.Remove(producto);

                ActualizarTotales();
            }
        }

        // 🔹 ELIMINAR
        private void BtnEliminar_Click(object sender, MouseButtonEventArgs e)
        {
            var producto = (sender as FrameworkElement).DataContext as ProductoCompra;

            if (producto != null)
            {
                carrito.Remove(producto);
                ActualizarTotales();
            }
        }

        // 🔹 ENTER EN BUSCADOR
        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                modoCantidad = true;
                bufferCantidad = "";
                cantidadActual = 1;

                if (PanelQty != null)
                    PanelQty.Visibility = Visibility.Visible;

                if (TxtQty != null)
                    TxtQty.Text = "1";

                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter || e.Key == Key.F3)
            {
                if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                {
                    int.TryParse(bufferCantidad, out cantidadActual);
                }

                AbrirBuscador();
                ResetCantidad();
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                ResetCantidad();
            }
        }
        private void TxtBuscar_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (modoCantidad)
            {
                if (char.IsDigit(e.Text, 0))
                {
                    bufferCantidad += e.Text;

                    if (int.TryParse(bufferCantidad, out int resultado))
                    {
                        cantidadActual = resultado;

                        if (TxtQty != null)
                            TxtQty.Text = cantidadActual.ToString();
                    }
                }

                e.Handled = true;
            }
        }
        private void ResetCantidad()
        {
            modoCantidad = false;
            bufferCantidad = "";
            cantidadActual = 1;

            if (PanelQty != null)
                PanelQty.Visibility = Visibility.Collapsed;

            if (TxtQty != null)
                TxtQty.Text = "1";
            txtEstadoProveedor.Visibility = Visibility.Visible;
            txtNombreProveedor.Visibility = Visibility.Collapsed;

            txtEstadoProveedor.Text = "SIN PROVEEDOR";
        }

        // 🔹 CLICK EN BOTÓN ADD
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AbrirBuscador();
        }

        // 🔹 TOTALES
        private void ActualizarTotales()
        {
            decimal subtotal = carrito.Sum(p => p.Total);
            decimal impuesto = subtotal * 0.085m;
            decimal total = subtotal + impuesto;

            txtContadorItems.Text = $"ITEMS EN CARRITO: {carrito.Count}";

            txtSubtotal.Text = subtotal.ToString("C");
            txtImpuesto.Text = impuesto.ToString("C");
            txtTotal.Text = total.ToString("C");
        }


        private void GenerarCompra()
        {
            try
            {
                if (carrito.Count == 0)
                {
                    MessageBox.Show("Carrito vacío");
                    return;
                }

                if (proveedorSeleccionado == null)
                {
                    MessageBox.Show("Selecciona proveedor");
                    return;
                }

                ClassCompras compras = new ClassCompras();

                int idCompra = compras.RegistrarCompra(
                    carrito.ToList(),
                    proveedorSeleccionado.IdProveedor,
                    1
                );

                MessageBox.Show($"Compra registrada ID: {idCompra}");

                carrito.Clear();
                ActualizarTotales();
                ResetCantidad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GenerarCompra();
        }
    }

    // 🔥 MODELO DE COMPRA (RESPETA TU XAML)
    public class ProductoCompra
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string SKU { get; set; }
        public decimal Costo { get; set; }
        public int Cantidad { get; set; }

        public decimal Total => Costo * Cantidad;
    }
}