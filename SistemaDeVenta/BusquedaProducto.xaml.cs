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
        public ProductoPOS ProductoSeleccionado { get; set; }

        public BusquedaProducto()
        {
            InitializeComponent();
            CargarDatos();
            txtBuscar.Focus();

            // Esto asegura que la ventana escuche las teclas antes que los controles
            this.PreviewKeyDown += BusquedaProducto_PreviewKeyDown;
        }

        private void CargarDatos()
        {
            try
            {
                listaOriginal.Clear();
                ClassTest db = new ClassTest();

                // Usamos INNER JOIN para traer el Nombre y Precio de 'Productos' 
                // y el Stock de 'Inventario'
                string sql = @"SELECT p.IdProducto, p.Nombre, p.PrecioVenta, i.Stock 
                       FROM Productos p 
                       INNER JOIN Inventario i ON p.IdProducto = i.IdProducto 
                       WHERE p.Disponible = TRUE";

                DataTable dt = db.ListarRegistros(sql);

                foreach (DataRow row in dt.Rows)
                {
                    listaOriginal.Add(new ProductoBusqueda
                    {
                        // Importante: Asegúrate de que los nombres coincidan con el SELECT
                        Id = Convert.ToInt32(row["IdProducto"]),
                        Nombre = row["Nombre"].ToString(),
                        Precio = Convert.ToDecimal(row["PrecioVenta"]),
                        Stock = Convert.ToInt32(Convert.ToDecimal(row["Stock"])) // Convertimos a decimal primero por si el stock tiene decimales en la BD
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

        private void SeleccionarYSalir()
        {
            // Verificamos que haya algo seleccionado en la tabla
            if (dgProductos.SelectedItem is ProductoBusqueda seleccionado)
            {
                ProductoSeleccionado = new ProductoPOS
                {
                    Id = seleccionado.Id.ToString(),
                    Name = seleccionado.Nombre,
                    UnitPrice = seleccionado.Precio,
                    Description = "Stock: " + seleccionado.Stock,
                    Unit = "PZ"
                };

                this.DialogResult = true; // Indica éxito a la ventana de Cobro
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista.");
            }
        }

        private void AplicarFiltroManual()
        {
            string filtro = txtBuscar.Text.Trim();
            var listaFiltrada = string.IsNullOrEmpty(filtro) ?
                listaOriginal :
                listaOriginal.Where(p => p.Nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 || p.Id.ToString() == filtro).ToList();

            dgProductos.ItemsSource = null;
            dgProductos.ItemsSource = listaFiltrada;
        }

        // --- EVENTOS CORREGIDOS ---

        private void BusquedaProducto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Cierra con ESC desde cualquier lado
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }

            // Si presiona ENTER
            if (e.Key == Key.Enter)
            {
                // Si el foco no está en el buscador, intentamos agregar
                if (!txtBuscar.IsFocused)
                {
                    SeleccionarYSalir();
                    e.Handled = true;
                }
                else
                {
                    // Si está en el buscador, aplica el filtro
                    AplicarFiltroManual();
                }
            }
        }

        // Este es el botón "AGREGAR" de tu interfaz
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarYSalir();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e) => AplicarFiltroManual();
        private void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SeleccionarYSalir();
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e) => AplicarFiltroManual();
    }

    public class ProductoBusqueda
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public decimal Stock { get; set; } // Cambiado de int a decimal para soportar Kilos
    }
}