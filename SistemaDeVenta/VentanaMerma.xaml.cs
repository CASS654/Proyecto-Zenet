using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaDeVenta
{
    public partial class VentanaMerma : Window
    {
        public VentanaMerma()
        {
            InitializeComponent();
        }

        private InventarioView producto;

        public VentanaMerma(InventarioView p)
        {
            InitializeComponent();

            producto = p;

            lblProducto.Text = "Producto: " + producto.Nombre;
            lblStockActual.Text = "Stock actual: " + producto.Stock.ToString();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (producto == null)
            {
                MessageBox.Show("Error: producto no válido");
                return;
            }

            if (globales.IdUsuarioGlobal == 0)
            {
                MessageBox.Show("Error: No hay usuario logueado");
                return;
            }

            if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad))
            {
                MessageBox.Show("Cantidad inválida");
                return;
            }

            if (cantidad <= 0 || cantidad > producto.Stock)
            {
                MessageBox.Show("Cantidad incorrecta");
                return;
            }

            if (cbRazon.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una razón de merma");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Deseas registrar la merma?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                var conn = ClassConexion.SQLConnection;

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                var transaction = conn.BeginTransaction();

                try
                {
                    // 🔴 1. INSERT MERMA
                    string queryMerma = @"INSERT INTO Merma (IdUsuario, Observaciones)
                                         VALUES (@Usuario, @Obs);
                                         SELECT LAST_INSERT_ID();";

                    MySqlCommand cmdMerma = new MySqlCommand(queryMerma, conn, transaction);
                    cmdMerma.Parameters.AddWithValue("@Usuario", globales.IdUsuarioGlobal);
                    cmdMerma.Parameters.AddWithValue("@Obs",
                        (cbRazon.SelectedItem as ComboBoxItem).Content.ToString());

                    int idMerma = Convert.ToInt32(cmdMerma.ExecuteScalar());

                    // 🔴 2. CALCULAR SUBTOTAL
                    decimal precioReferencia = producto.PrecioCompra;
                    decimal subtotal = precioReferencia * cantidad;

                    // 🔴 3. INSERT DETALLE (CORRECTO SEGÚN TU BD)
                    string queryDetalle = @"INSERT INTO MermaDetalles 
                                           (IdMerma, IdProducto, Cantidad, PrecioReferencia, Subtotal)
                                           VALUES (@IdMerma, @Producto, @Cantidad, @Precio, @Subtotal)";

                    MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, conn, transaction);
                    cmdDetalle.Parameters.AddWithValue("@IdMerma", idMerma);
                    cmdDetalle.Parameters.AddWithValue("@Producto", producto.IdProducto);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmdDetalle.Parameters.AddWithValue("@Precio", precioReferencia);
                    cmdDetalle.Parameters.AddWithValue("@Subtotal", subtotal);

                    cmdDetalle.ExecuteNonQuery();

                    // 🔴 4. ACTUALIZAR TOTAL MERMA
                    string queryTotal = @"UPDATE Merma 
                                         SET TotalMerma = @Total
                                         WHERE IdMerma = @IdMerma";

                    MySqlCommand cmdTotal = new MySqlCommand(queryTotal, conn, transaction);
                    cmdTotal.Parameters.AddWithValue("@Total", subtotal);
                    cmdTotal.Parameters.AddWithValue("@IdMerma", idMerma);

                    cmdTotal.ExecuteNonQuery();

                    // 🔴 5. ACTUALIZAR STOCK
                    string queryStock = @"UPDATE Inventario 
                                         SET Stock = Stock - @Cantidad
                                         WHERE IdProducto = @Producto
                                         AND Stock >= @Cantidad";

                    MySqlCommand cmdStock = new MySqlCommand(queryStock, conn, transaction);
                    cmdStock.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmdStock.Parameters.AddWithValue("@Producto", producto.IdProducto);

                    int filas = cmdStock.ExecuteNonQuery();

                    if (filas == 0)
                        throw new Exception("Stock insuficiente");

                    transaction.Commit();

                    MessageBox.Show("Merma registrada correctamente");

                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error en la operación: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}