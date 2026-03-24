using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sistema_Bancario;

namespace SistemaDeVentaPrueba
{
    public partial class BusquedaProducto : Window
    {
        private List<ProductoBusqueda> listaOriginal = new List<ProductoBusqueda>();
        public int ProductoSeleccionadoId { get; private set; } = -1;

        public BusquedaProducto()
        {
            InitializeComponent();
            CargarDatos();
            txtBuscar.Focus();
        }

        private void CargarDatos()
        {
            try
            {
                listaOriginal.Clear();
                ClassTest db = new ClassTest();

                string sql = "SELECT IdProducto, Nombre, PrecioVenta, Stock FROM Inventario WHERE Disponible = 1";
                DataTable dt = db.ListarRegistros(sql);

                foreach (DataRow row in dt.Rows)
                {
                    listaOriginal.Add(new ProductoBusqueda
                    {
                        Id = Convert.ToInt32(row["IdProducto"]),
                        Nombre = row["Nombre"].ToString(),
                        Precio = Convert.ToDecimal(row["PrecioVenta"]),
                        Stock = Convert.ToInt32(row["Stock"])
                    });
                }

                dgProductos.ItemsSource = null;
                dgProductos.ItemsSource = listaOriginal;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar inventario: " + ex.Message);
            }
        }

        private void AplicarFiltroManual()
        {
            string filtro = txtBuscar.Text.Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                dgProductos.ItemsSource = null;
                dgProductos.ItemsSource = listaOriginal;
                return;
            }

            var listaFiltrada = listaOriginal.Where(p =>
                p.Nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Id.ToString() == filtro
            ).ToList();

            dgProductos.ItemsSource = null;
            dgProductos.ItemsSource = listaFiltrada;

            // 🔥 SOLO cambiar foco si NO estás escribiendo
            if (!txtBuscar.IsFocused && listaFiltrada.Count > 0)
            {
                dgProductos.SelectedIndex = 0;
                dgProductos.Focus();
            }
        }

        private void SeleccionarYSalir()
        {
            if (dgProductos.SelectedItem is ProductoBusqueda seleccionado)
            {
                ProductoSeleccionadoId = seleccionado.Id;
                this.DialogResult = true;
                this.Close();
            }
        }

        // 🔵 BOTÓN BUSCAR (CLICK)
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltroManual();
        }

        // 🔵 DOBLE CLICK EN LA TABLA
        private void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SeleccionarYSalir();
        }

        // 🔵 CONTROL DE TECLAS
        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (txtBuscar.IsFocused)
                {
                    // ENTER en el textbox → FILTRAR
                    AplicarFiltroManual();
                }
                else
                {
                    // ENTER en la tabla → SELECCIONAR
                    SeleccionarYSalir();
                }
            }

            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                dgProductos.Focus();
            }
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltroManual();
        }
    }

    public class ProductoBusqueda
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}