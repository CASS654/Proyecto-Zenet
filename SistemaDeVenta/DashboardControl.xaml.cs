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
            CargarKPIsMensuales ();
                CargarProductos();
                CargarProductosBajos();
            }

        private void CargarKPIsMensuales()
        {
            try
            {
                MySqlConnection conn = ClassConexion.SQLConnection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    // 1. TOTAL VENTAS DEL MES ACTUAL
                    // Filtra donde el mes y el año coincidan con el presente
                    cmd.CommandText = @"SELECT IFNULL(SUM(Total), 0) FROM Ventas 
                                WHERE MONTH(Fecha) = MONTH(CURDATE()) 
                                AND YEAR(Fecha) = YEAR(CURDATE())";
                    decimal ventasMes = Convert.ToDecimal(cmd.ExecuteScalar());
                    TxtVentasHoy.Text = ventasMes.ToString("C2"); // Puedes renombrar el Label a TxtVentasMes

                    // 2. TOTAL PRODUCTOS VENDIDOS EN EL MES
                    cmd.CommandText = @"SELECT IFNULL(SUM(dv.Cantidad), 0) 
                                FROM DetalleVenta dv 
                                INNER JOIN Ventas v ON dv.IdVenta = v.IdVenta 
                                WHERE MONTH(v.Fecha) = MONTH(CURDATE()) 
                                AND YEAR(v.Fecha) = YEAR(CURDATE())";

                    object resProd = cmd.ExecuteScalar();
                    TxtProductosVendidos.Text = resProd != null ? Convert.ToDouble(resProd).ToString("N2") : "0.00";

                    // 3. CLIENTES DISTINTOS EN EL MES
                    // Cuenta cuántas ventas únicas se han realizado este mes
                    cmd.CommandText = @"SELECT COUNT(IdVenta) FROM Ventas 
                                WHERE MONTH(Fecha) = MONTH(CURDATE()) 
                                AND YEAR(Fecha) = YEAR(CURDATE())";
                    TxtClientes.Text = cmd.ExecuteScalar().ToString();

                    // 4. PRODUCTOS BAJOS (Este KPI suele ser estático, no depende del mes)
                    cmd.CommandText = "SELECT COUNT(*) FROM Inventario WHERE Stock <= 10";
                    TxtBajoStock.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Dashboard Mensual: " + ex.Message);
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
                 WHERE i.Stock <= 20
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
                public decimal Vendidos { get; set; }
            }

        private void CargarProductos()
        {
            List<ProductoTop> lista = new List<ProductoTop>();
            try
            {
                MySqlConnection conn = ClassConexion.SQLConnection;
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                // Agregamos el filtro de fecha para que coincida con los KPIs de arriba
                string query = @"SELECT 
                            p.Nombre,
                            p.Categoria,
                            p.PrecioVenta AS Precio,
                            SUM(dv.Cantidad) AS Vendidos
                         FROM DetalleVenta dv
                         JOIN Productos p ON dv.IdProducto = p.IdProducto
                         JOIN Ventas v ON dv.IdVenta = v.IdVenta
                         WHERE MONTH(v.Fecha) = MONTH(CURDATE()) 
                           AND YEAR(v.Fecha) = YEAR(CURDATE())
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
                            // USAR DECIMAL AQUÍ es vital para ventas por kilo
                            Vendidos = Convert.ToDecimal(reader["Vendidos"])
                        });
                    }
                }
                TopProductsGrid.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando Ranking: " + ex.Message);
            }
        }
    }
    }
