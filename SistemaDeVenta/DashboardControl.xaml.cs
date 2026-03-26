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
using MySql.Data.MySqlClient;
using Sistema_Bancario;
using SistemaDeVentaPrueba;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para DashboardControl.xaml
    /// </summary>
    public partial class DashboardControl : UserControl
    {
        public DashboardControl()
        {
            InitializeComponent();
            CargarKPIs();
            CargarProductos();
            CargarProductosBajos();
        }

        private void CargarKPIs()
        {
            MySqlConnection conn = ClassConexion.SQLConnection;

            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = "SELECT IFNULL(SUM(Total),0) FROM Ventas WHERE DATE(Fecha)=CURDATE();";
                TxtVentasHoy.Text = "$" + cmd.ExecuteScalar().ToString();

                cmd.CommandText = @"SELECT IFNULL(SUM(Cantidad),0)
                            FROM DetalleVenta dv
                            JOIN Ventas v ON dv.IdVenta=v.IdVenta
                            WHERE DATE(v.Fecha)=CURDATE();";
                TxtProductosVendidos.Text = cmd.ExecuteScalar().ToString();

                cmd.CommandText = "SELECT COUNT(*) FROM Ventas;";
                TxtClientes.Text = cmd.ExecuteScalar().ToString();

                cmd.CommandText = "SELECT COUNT(*) FROM Inventario WHERE Stock < 10;";
                TxtBajoStock.Text = cmd.ExecuteScalar().ToString();
            }
        }
        public class ProductoBajo
        {
            public string Nombre { get; set; }
            public decimal Stock { get; set; }
        }

        private void CargarProductosBajos()
        {
            List<ProductoBajo> lista = new List<ProductoBajo>();

            MySqlConnection conn = ClassConexion.SQLConnection;

            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            string query = @"SELECT p.Nombre, i.Stock
                     FROM Inventario i
                     INNER JOIN Productos p ON p.IdProducto = i.IdProducto
                     WHERE i.Stock <= 10
                     ORDER BY i.Stock ASC";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new ProductoBajo
                    {
                        Nombre = reader["Nombre"].ToString(),
                        Stock = reader["Stock"] != DBNull.Value
                                ? Convert.ToDecimal(reader["Stock"])
                                : 0
                    });
                }
            }

            LowStockList.ItemsSource = lista;
        }
        public class ProductoTop
        {
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public double Precio { get; set; }
            public int Vendidos { get; set; }
        }

        private void CargarProductos()
        {
            List<ProductoTop> lista = new List<ProductoTop>();

            MySqlConnection conn = ClassConexion.SQLConnection;

            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            string query = @"SELECT 
                        p.Nombre,
                        p.Categoria,
                        p.PrecioVenta AS Precio,
                        SUM(dv.Cantidad) AS Vendidos
                     FROM DetalleVenta dv
                     JOIN Productos p ON dv.IdProducto = p.IdProducto
                     GROUP BY p.Nombre, p.Categoria, p.PrecioVenta
                     ORDER BY Vendidos DESC
                     LIMIT 5;";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new ProductoTop
                    {
                        Nombre = reader["Nombre"].ToString(),
                        Categoria = reader["Categoria"].ToString(),
                        Precio = Convert.ToDouble(reader["Precio"]),
                        Vendidos = Convert.ToInt32(reader["Vendidos"])
                    });
                }
            }

            TopProductsGrid.ItemsSource = lista;
        }
    }
}
