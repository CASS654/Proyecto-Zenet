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
using static System.Net.Mime.MediaTypeNames;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para HistorialVenta.xaml
    /// </summary>
    public partial class HistorialVenta : UserControl
    {
        string FiltroActual = "Producto"; // Valor por defecto
        private ClassTest test = new ClassTest();
        public HistorialVenta()
        {
            InitializeComponent();
            CargarDetalleVenta();
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
            FiltroActual = item.Header.ToString();

        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string valorBuscar = txtBuscar.Text.Trim();

                if (string.IsNullOrEmpty(valorBuscar))
                {
                    // Si no hay texto, cargar todo
                    CargarDetalleVenta();
                    return;
                }

                // Mapeo de filtros a columnas de la BD
                string columnaBD;

                switch (FiltroActual)
                {
                    case "ID Detalle":
                        columnaBD = "dv.IdDetalle";
                        break;
                    case "ID Venta":
                        columnaBD = "dv.IdVenta";
                        break;
                    case "ID Producto":
                        columnaBD = "dv.IdProducto";
                        break;
                    case "Producto":
                        columnaBD = "i.Nombre";
                        break;
                    case "Cantidad":
                        columnaBD = "dv.Cantidad";
                        break;
                    default:
                        columnaBD = "dv.IdDetalle";
                        break;
                }

                string query = $@"
            SELECT 
                dv.IdDetalle, 
                dv.IdVenta, 
                dv.IdProducto, 
                i.Nombre AS Producto, 
                i.Categoria, 
                dv.Cantidad, 
                dv.PrecioUnitario, 
                dv.Subtotal
            FROM DetalleVenta dv
            INNER JOIN Inventario i ON dv.IdProducto = i.IdProducto
            WHERE {columnaBD} LIKE @valor";

                DataTable dt = test.ListarRegistrosConParametro(query, "@valor", "%" + valorBuscar + "%");

                TablaReporte.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda: " + ex.Message);
            }
        }
        private void CargarDetalleVenta()
        {
            try
            {
                string query = @"
            SELECT 
                dv.IdDetalle, 
                dv.IdVenta, 
                dv.IdProducto, 
                i.Nombre AS Producto, 
                i.Categoria, 
                dv.Cantidad, 
                dv.PrecioUnitario, 
                dv.Subtotal
            FROM DetalleVenta dv
            INNER JOIN Inventario i ON dv.IdProducto = i.IdProducto";

                DataTable dt = test.ListarRegistros(query);

                // Asignar al DataGrid
                TablaReporte.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial de ventas: " + ex.Message);
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            Buscar_Click(sender, e);    
        }
    }
}
