using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaDeVenta
{
    internal class InventarioDAO
    {
        public List<InventarioView> ObtenerInventario()
        {
            List<InventarioView> lista = new List<InventarioView>();

            try
            {
                MySqlConnection conn = ClassConexion.ObtenerConexion();

                // 🔥 ABRIR CONEXIÓN SI ESTÁ CERRADA
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                string query = @"SELECT p.IdProducto, p.Nombre, p.Categoria,
                         p.PrecioCompra, p.PrecioVenta,
                         i.Stock, p.Disponible
                         FROM Productos p
                         INNER JOIN Inventario i 
                         ON p.IdProducto = i.IdProducto";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new InventarioView
                        {
                            IdProducto = reader.GetInt32("IdProducto"),
                            Nombre = reader.GetString("Nombre"),
                            Categoria = reader.GetString("Categoria"),
                            PrecioCompra = reader.GetDecimal("PrecioCompra"),
                            PrecioVenta = reader.GetDecimal("PrecioVenta"),
                            Stock = reader.GetDecimal("Stock"),
                            Disponible = reader.GetBoolean("Disponible")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar inventario: " + ex.ToString());
            }

            return lista;
        }
    }
}
