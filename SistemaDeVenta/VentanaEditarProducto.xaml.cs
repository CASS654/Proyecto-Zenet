using MySql.Data.MySqlClient;
using Sistema_Bancario;
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
using System.Windows.Shapes;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para VentanaEditarProducto.xaml
    /// </summary>
    public partial class VentanaEditarProducto : Window
    {
      
        private InventarioView producto;

        public VentanaEditarProducto(InventarioView p)
        {
            InitializeComponent();

            this.producto = p; // ✅ AHORA SÍ

            txtNombre.Text = producto.Nombre;
            txtVenta.Text = producto.PrecioVenta.ToString();
            txtCompra.Text = producto.PrecioCompra.ToString();
            txtStock.Text = producto.Stock.ToString();
            cbCategoria.Text = producto.Categoria;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (producto == null)
            {
                MessageBox.Show("Error: No se cargaron los datos del producto correctamente.");
                return;
            }

            // 🔥 CONFIRMACIÓN
            var resultado = MessageBox.Show(
                "¿Estás seguro de guardar los cambios?",
                "Confirmar actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado == MessageBoxResult.No)
            {
                return; // ❌ Cancela todo
            }

            try
            {
                string query = @"UPDATE Productos 
                 SET Nombre=@Nombre,
                     Categoria=@Categoria,
                     PrecioCompra=@Compra,
                     PrecioVenta=@Venta
                 WHERE IdProducto=@Id";

                MySqlCommand cmd = new MySqlCommand(query, ClassConexion.ObtenerConexion());

                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@Categoria", cbCategoria.Text);
                cmd.Parameters.AddWithValue("@Compra", Convert.ToDecimal(txtCompra.Text));
                cmd.Parameters.AddWithValue("@Venta", Convert.ToDecimal(txtVenta.Text));
                cmd.Parameters.AddWithValue("@Id", producto.IdProducto);

                cmd.ExecuteNonQuery();

                // 🔥 (RECOMENDADO) también actualizar stock
                string queryStock = @"UPDATE Inventario 
                     SET Stock=@Stock 
                     WHERE IdProducto=@Id";

                MySqlCommand cmdStock = new MySqlCommand(queryStock, ClassConexion.SQLConnection);

                cmdStock.Parameters.AddWithValue("@Stock", Convert.ToDecimal(txtStock.Text));
                cmdStock.Parameters.AddWithValue("@Id", producto.IdProducto);

                cmdStock.ExecuteNonQuery();

                MessageBox.Show("Producto actualizado correctamente");

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }

        }
    }
}
