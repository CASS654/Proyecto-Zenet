using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System.Collections.Generic;

namespace SistemaDeVenta
{
    internal class ReportesDAO
    {
        public List<dynamic> ObtenerVentas()
        {
            var lista = new List<dynamic>();

            using (var conn = ClassConexion.ObtenerConexion())
            {
                string query = @"
    SELECT 
        hv.IdHistorialVenta,
        u.Nombre_Completo AS Usuario,
        hv.TipoAccion,
        hv.TotalRegistrado,
        hv.FechaAccion
    FROM HistorialVentas hv
    INNER JOIN Usuarios u ON hv.IdUsuario = u.IdUsuario
    ORDER BY hv.FechaAccion DESC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Id = reader["IdHistorialVenta"],
                        Usuario = reader["Usuario"],
                        Tipo = reader["TipoAccion"],
                        Total = reader["TotalRegistrado"],
                        Fecha = reader["FechaAccion"]
                    });
                }
            }

            return lista;
        }

        public List<dynamic> ObtenerCompras()
        {
            var lista = new List<dynamic>();

            using (var conn = ClassConexion.ObtenerConexion())
            {
                string query = @"
    SELECT 
        hc.IdHistorialCompra,
        u.Nombre_Completo AS Usuario,
        hc.TipoAccion,
        hc.TotalRegistrado,
        hc.FechaAccion
    FROM HistorialCompras hc
    INNER JOIN Usuarios u ON hc.IdUsuario = u.IdUsuario
    ORDER BY hc.FechaAccion DESC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Id = reader["IdHistorialCompra"],
                        Usuario = reader["Usuario"],
                        Tipo = reader["TipoAccion"],
                        Total = reader["TotalRegistrado"],
                        Fecha = reader["FechaAccion"]
                    });
                }
            }

            return lista;
        }

        public List<dynamic> ObtenerCambios()
        {
            var lista = new List<dynamic>();

            using (var conn = ClassConexion.ObtenerConexion())
            {
                string query = @"
    SELECT 
        hc.IdHistorial,
        u.Nombre_Completo AS Usuario,
        hc.TablaAfectada,
        hc.CampoModificado,
        hc.ValorAnterior,
        hc.ValorNuevo,
        hc.FechaCambio
    FROM HistorialCambios hc
    INNER JOIN Usuarios u ON hc.IdUsuario = u.IdUsuario
    ORDER BY hc.FechaCambio DESC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Id = reader["IdHistorial"],
                        Usuario = reader["Usuario"],
                        Tabla = reader["TablaAfectada"],
                        Campo = reader["CampoModificado"],
                        Anterior = reader["ValorAnterior"],
                        Nuevo = reader["ValorNuevo"],
                        Fecha = reader["FechaCambio"]
                    });
                }
            }

            return lista;
        }

        public List<dynamic> ObtenerMerma()
        {
            var lista = new List<dynamic>();

            using (var conn = ClassConexion.ObtenerConexion())
            {
                string query = @"
    SELECT 
        m.IdMerma,
        u.Nombre_Completo AS Usuario,
        p.Nombre AS Producto,
        md.Cantidad,
        md.PrecioReferencia,
        md.Subtotal,
        m.Fecha
    FROM Merma m
    INNER JOIN Usuarios u ON m.IdUsuario = u.IdUsuario
    INNER JOIN MermaDetalles md ON m.IdMerma = md.IdMerma
    INNER JOIN Productos p ON md.IdProducto = p.IdProducto
    ORDER BY m.Fecha DESC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new
                    {
                        Id = reader["IdMerma"],
                        Usuario = reader["Usuario"],
                        Producto = reader["Producto"],
                        Cantidad = reader["Cantidad"],
                        Precio = reader["PrecioReferencia"],
                        Subtotal = reader["Subtotal"],
                        Fecha = reader["Fecha"]
                    });
                }
            }

            return lista;
        }
    }
}