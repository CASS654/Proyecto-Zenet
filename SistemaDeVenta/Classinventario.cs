using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta
{
    internal class Classinventario
    {
        // cesar 
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public string Unidad { get; set; }
        public Decimal Stock { get; set; }
        public Boolean Disponible { get; set; }

        public int InsertarProducto(Classinventario obj)
        {
            int resultado = 1;

            using (MySqlConnection connection = new MySqlConnection(
                "Database=" + globales.vBaseDeDatosConnect_Global +
                "; Data Source=" + globales.vServidorConnect_Global +
                "; User Id=" + globales.vUsuarioINIConnect_Global +
                "; Password=" + globales.vPassUsuarioINIConnect_Global +
                ";CharSet=utf8;"))
            {
                connection.Open();
                MySqlTransaction trans = null;

                try
                {
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    cmd.CommandText = @"
                            INSERT INTO inventario 
                            (Nombre, Categoria, PrecioVenta, PrecioCompra, Unidad, Stock, Disponible)
                            VALUES (@nombre, @categoria, @precioVenta, @precioCompra, @unidad, @stock, @disponible)";

                    cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@categoria", obj.Categoria);
                    cmd.Parameters.AddWithValue("@precioVenta", obj.PrecioVenta);
                    cmd.Parameters.AddWithValue("@precioCompra", obj.PrecioCompra);
                    cmd.Parameters.AddWithValue("@unidad", obj.Unidad);
                    cmd.Parameters.AddWithValue("@stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@disponible", obj.Disponible);

                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    resultado = 0;
                }
                catch (Exception)
                {
                    trans?.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }

            return resultado;
        }

        public int EditarProducto(Classinventario obj)
        {
            int resultado = 1;

            using (MySqlConnection connection = new MySqlConnection(
                "Database=" + globales.vBaseDeDatosConnect_Global +
                "; Data Source=" + globales.vServidorConnect_Global +
                "; User Id=" + globales.vUsuarioINIConnect_Global +
                "; Password=" + globales.vPassUsuarioINIConnect_Global +
                ";CharSet=utf8;"))
            {
                connection.Open();
                MySqlTransaction trans = null;

                try
                {
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    cmd.CommandText = @"
                            UPDATE inventario SET
                                Nombre=@nombre,
                                Categoria=@categoria,
                                PrecioVenta=@precioVenta,
                                PrecioCompra=@precioCompra,
                                Unidad=@unidad,
                                Stock=@stock,
                                Disponible=@disponible
                            WHERE IdProducto=@id";

                    cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@categoria", obj.Categoria);
                    cmd.Parameters.AddWithValue("@precioVenta", obj.PrecioVenta);
                    cmd.Parameters.AddWithValue("@precioCompra", obj.PrecioCompra);
                    cmd.Parameters.AddWithValue("@unidad", obj.Unidad);
                    cmd.Parameters.AddWithValue("@stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@disponible", obj.Disponible);
                    cmd.Parameters.AddWithValue("@id", obj.IdProducto);

                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    resultado = 0;
                }
                catch (Exception)
                {
                    trans?.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }

            return resultado;
        }
        public int EliminarProducto(int id)
        {
            int resultado = 1;

            using (MySqlConnection connection = new MySqlConnection(
                "Database=" + globales.vBaseDeDatosConnect_Global +
                "; Data Source=" + globales.vServidorConnect_Global +
                "; User Id=" + globales.vUsuarioINIConnect_Global +
                "; Password=" + globales.vPassUsuarioINIConnect_Global +
                ";CharSet=utf8;"))
            {
                connection.Open();
                MySqlTransaction trans = null;

                try
                {
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    cmd.CommandText = "DELETE FROM inventario WHERE IdProducto=@id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    resultado = 0;
                }
                catch (Exception)
                {
                    trans?.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }

            return resultado;
        }
    }
}
