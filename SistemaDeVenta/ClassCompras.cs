using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SistemaDeVenta; // 👈 IMPORTANTE: usa tu modelo ProductoCompra

namespace Sistema_Bancario
{
    public class ClassCompras
    {
        private MySqlConnection conexion;

        public ClassCompras()
        {
            conexion = ClassConexion.SQLConnection;
        }

        // 🔥 MÉTODO PRINCIPAL
        public int RegistrarCompra(List<ProductoCompra> carrito, int idProveedor, int idUsuario)
        {
            if (carrito == null || carrito.Count == 0)
                throw new Exception("El carrito está vacío");

            if (idProveedor <= 0)
                throw new Exception("Proveedor inválido");

            try
            {
                if (conexion.State != ConnectionState.Open)
                    conexion.Open();

                MySqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    decimal total = carrito.Sum(p => p.Total);

                    // 🔹 INSERTAR COMPRA
                    string sqlCompra = @"
                    INSERT INTO Compras (Fecha, IdProveedor, IdUsuario, Total)
                    VALUES (NOW(), @proveedor, @usuario, @total);
                    SELECT LAST_INSERT_ID();";

                    MySqlCommand cmdCompra = new MySqlCommand(sqlCompra, conexion, transaccion);
                    cmdCompra.Parameters.AddWithValue("@proveedor", idProveedor);
                    cmdCompra.Parameters.AddWithValue("@usuario", idUsuario);
                    cmdCompra.Parameters.AddWithValue("@total", total);

                    int idCompra = Convert.ToInt32(cmdCompra.ExecuteScalar());

                    // 🔹 INSERTAR DETALLE (EL TRIGGER MANEJA INVENTARIO 🔥)
                    foreach (var item in carrito)
                    {
                        decimal subtotal = item.Cantidad * item.Costo;

                        string sqlDetalle = @"
                        INSERT INTO DetallesCompras 
                        (IdCompra, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                        VALUES 
                        (@compra, @producto, @cantidad, @precio, @subtotal);";

                        MySqlCommand cmdDetalle = new MySqlCommand(sqlDetalle, conexion, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@compra", idCompra);
                        cmdDetalle.Parameters.AddWithValue("@producto", item.Id);
                        cmdDetalle.Parameters.AddWithValue("@cantidad", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@precio", item.Costo);
                        cmdDetalle.Parameters.AddWithValue("@subtotal", subtotal);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    // 🔥 CONFIRMAR TODO
                    transaccion.Commit();

                    return idCompra;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
            finally
            {
                if (conexion.State == ConnectionState.Open)
                    conexion.Close();
            }
        }
    }
}