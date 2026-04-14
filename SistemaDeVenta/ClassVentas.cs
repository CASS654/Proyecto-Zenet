using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Sistema_Bancario;

namespace SistemaDeVenta
{
    public class VentaService
    {
        /// <summary>
        /// Usa la conexión estática que ya abrió LoginPage
        /// </summary>
        public int GuardarVenta(ObservableCollection<ProductoPOS> carrito,
                                int idUsuario,
                                string metodoPago = "EFECTIVO",
                                decimal tasaImpuesto = 0.08m)
        {
            if (carrito == null || carrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }

            decimal subtotal = 0;
            foreach (var p in carrito)
                subtotal += p.Total;

            decimal total = subtotal + (subtotal * tasaImpuesto);

            // Reutiliza la conexión que ya abrió el Login
            MySqlConnection conn = ClassConexion.SQLConnection;

            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    int idVenta = InsertarVenta(conn, transaction, idUsuario, total, metodoPago);

                    foreach (var producto in carrito)
                        InsertarDetalleVenta(conn, transaction, idVenta, producto);

                    transaction.Commit();
                    return idVenta;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error al guardar la venta:\n{ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
        }

        private int InsertarVenta(MySqlConnection conn, MySqlTransaction transaction,
                                   int idUsuario, decimal total, string metodoPago)
        {
            string sql = @"
                INSERT INTO Ventas (Fecha, IdUsuario, Total, MetodoPago)
                VALUES (@Fecha, @IdUsuario, @Total, @MetodoPago);
                SELECT LAST_INSERT_ID();";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void InsertarDetalleVenta(MySqlConnection conn, MySqlTransaction transaction,
                                           int idVenta, ProductoPOS producto)
        {
            string sql = @"
                INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                VALUES (@IdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal)";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                cmd.Parameters.AddWithValue("@IdProducto", producto.Id);
                cmd.Parameters.AddWithValue("@Cantidad", producto.Quantity);
                cmd.Parameters.AddWithValue("@PrecioUnitario", producto.UnitPrice);
                cmd.Parameters.AddWithValue("@Subtotal", producto.Total);

                cmd.ExecuteNonQuery();
            }
        }
    }
}