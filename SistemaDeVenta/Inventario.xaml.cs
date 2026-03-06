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

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para Inventario.xaml
    /// </summary>
    public partial class Inventario : UserControl
    {
        string FiltroSeleccionado = "Nombre del Producto";
        private int IdProductoSeleccionado = 0;
        public Inventario()
        {
            InitializeComponent();
            CargarInventario();
        }
        private void CargarInventario()
        {
            ClassTest obj = new ClassTest();
            DataTable registros = obj.ListarRegistros("SELECT * FROM inventario");

            List<Classinventario> lista = new List<Classinventario>();

            foreach (DataRow row in registros.Rows)
            {
                lista.Add(new Classinventario
                {
                    IdProducto = Convert.ToInt32(row["IdProducto"]),
                    Nombre = row["Nombre"].ToString(),
                    Categoria = row["Categoria"].ToString(),
                    PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),
                    PrecioCompra = Convert.ToDecimal(row["PrecioCompra"]),
                    Unidad = row["Unidad"].ToString(),
                    Stock = Convert.ToDecimal(row["Stock"]),
                    Disponible = Convert.ToInt32(row["Disponible"]) == 1

                });
            }

            TablaProductos.ItemsSource = lista;
        }

        private void btnFiltro_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)FindResource("MenuFiltroTemplate");

            menu.PlacementTarget = btnFiltro;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; // 👈 Esto lo hace abrirse hacia abajo
            menu.IsOpen = true;
        }

        private void FiltroMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            FiltroSeleccionado = item.Header.ToString();

        }

        private void TablaProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablaProductos.SelectedItem == null)
                return;

            Classinventario producto = (Classinventario)TablaProductos.SelectedItem;

            // ENVIAR LOS DATOS A LOS TEXTBOX
            txtNombre.Text = producto.Nombre;
            txtStock.Text = producto.Stock.ToString();
            txtPrecioVenta.Text = producto.PrecioVenta.ToString("0.00");
            txtPrecioCompra.Text = producto.PrecioCompra.ToString("0.00");

            // COMBOBOX: buscar el ComboBoxItem que coincida con el string y seleccionarlo
            cbCategoria.SelectedItem = cbCategoria.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString().Equals(producto.Categoria, StringComparison.OrdinalIgnoreCase));

            cbUnidad.SelectedItem = cbUnidad.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString().Equals(producto.Unidad, StringComparison.OrdinalIgnoreCase));

            cbDisponible.SelectedItem = cbDisponible.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == (producto.Disponible ? "Disponible" : "No Disponible"));

            // Guardamos el ID para modificaciones/eliminaciones
            IdProductoSeleccionado = producto.IdProducto;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Classinventario p = new Classinventario
            {
                Nombre = txtNombre.Text,
                Categoria = cbCategoria.SelectedItem is ComboBoxItem c1 ? c1.Content.ToString() : "",
                Unidad = cbUnidad.SelectedItem is ComboBoxItem uni ? uni.Content.ToString() : "",
                Stock = decimal.TryParse(txtStock.Text, out decimal stockVal) ? stockVal : 0,
                PrecioVenta = decimal.TryParse(txtPrecioVenta.Text, out decimal pv) ? pv : 0,
                PrecioCompra = decimal.TryParse(txtPrecioCompra.Text, out decimal pc) ? pc : 0,
                Disponible = (cbDisponible.SelectedItem is ComboBoxItem disp ? disp.Content.ToString() : "No Disponible") == "Disponible"
            };

            Classinventario db = new Classinventario();
            int resp = db.InsertarProducto(p);

            if (resp == 0)
            {
                MessageBox.Show("Producto agregado correctamente.");
                CargarInventario();
            }
            else
            {
                MessageBox.Show("Error al agregar producto.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (IdProductoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            Classinventario p = new Classinventario
            {
                IdProducto = IdProductoSeleccionado,
                Nombre = txtNombre.Text,
                Categoria = ((ComboBoxItem)cbCategoria.SelectedItem).Content.ToString(),
                Unidad = ((ComboBoxItem)cbUnidad.SelectedItem).Content.ToString(),
                Stock = decimal.TryParse(txtStock.Text, out decimal stockVal) ? stockVal : 0,
                PrecioVenta = decimal.TryParse(txtPrecioVenta.Text, out decimal pv) ? pv : 0,
                PrecioCompra = decimal.TryParse(txtPrecioCompra.Text, out decimal pc) ? pc : 0,
                Disponible = ((ComboBoxItem)cbDisponible.SelectedItem).Content.ToString() == "Disponible"
            };

            Classinventario db = new Classinventario();
            int resp = db.EditarProducto(p);

            if (resp == 0)
            {
                MessageBox.Show("Producto modificado correctamente.");
                CargarInventario();
            }
            else
            {
                MessageBox.Show("Error al modificar producto.");
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (IdProductoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            if (MessageBox.Show("¿Eliminar producto seleccionado?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            Classinventario db = new Classinventario();
            int resp = db.EliminarProducto(IdProductoSeleccionado);

            if (resp == 0)
            {
                MessageBox.Show("Producto eliminado correctamente.");
                CargarInventario();
                IdProductoSeleccionado = 0;
            }
            else
            {
                MessageBox.Show("Error al eliminar producto.");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                string textoBusqueda = txtBuscar.Text.Trim();

                // Si no hay texto, cargamos todo
                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    CargarInventario();
                    return;
                }

                string columna = "Nombre"; // por defecto

                switch (FiltroSeleccionado)
                {
                    case "Nombre del Producto":
                        columna = "Nombre";
                        break;
                    case "Precio Venta":
                        columna = "PrecioVenta";
                        break;
                    case "Disponible":
                        columna = "Disponible";
                        break;
                }

                // Consulta siempre con LIKE y % para coincidencia parcial
                string consulta = $"SELECT * FROM inventario WHERE {columna} LIKE @buscar";

                ClassTest obj = new ClassTest();
                DataTable registros = obj.ListarRegistrosConParametro(consulta, "@buscar", "%" + textoBusqueda + "%");

                // Limpiar y cargar la lista
                List<Classinventario> lista = new List<Classinventario>();
                foreach (DataRow row in registros.Rows)
                {
                    lista.Add(new Classinventario
                    {
                        IdProducto = Convert.ToInt32(row["IdProducto"]),
                        Nombre = row["Nombre"].ToString(),
                        Categoria = row["Categoria"].ToString(),
                        PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),
                        PrecioCompra = Convert.ToDecimal(row["PrecioCompra"]),
                        Unidad = row["Unidad"].ToString(),
                        Stock = Convert.ToDecimal(row["Stock"]),
                        Disponible = Convert.ToInt32(row["Disponible"]) == 1
                    });
                }

                TablaProductos.ItemsSource = null;
                TablaProductos.Items.Clear();
                TablaProductos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar productos: " + ex.Message);
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            Button_Click_3(sender, e);
        }
    }
}

