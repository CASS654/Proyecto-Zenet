using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta
{
    internal class ClassProveedores
    {
        public class Proveedores1
        {
            public int IdProveedor { get; set; }
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
        }
        public int InsertarProveedor(Proveedores1 p)
        {
            try
            {
                string sql = "INSERT INTO Proveedores (Nombre, Telefono, Direccion) " +
                             "VALUES (@Nombre, @Telefono, @Direccion)";

                using (var cmd = new MySqlCommand(sql, ClassConexion.SQLConnection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
                    cmd.Parameters.AddWithValue("@Direccion", p.Direccion);

                    return cmd.ExecuteNonQuery() == 1 ? 0 : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        // 🔹 MÉTODO ACTUALIZAR
        public int EditarProveedor(Proveedores1 p)
        {
            try
            {
                string sql = "UPDATE Proveedores SET Nombre=@Nombre, Telefono=@Telefono, Direccion=@Direccion " +
                             "WHERE IdProveedor=@IdProveedor";

                using (var cmd = new MySqlCommand(sql, ClassConexion.SQLConnection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
                    cmd.Parameters.AddWithValue("@Direccion", p.Direccion);
                    cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);

                    return cmd.ExecuteNonQuery() == 1 ? 0 : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        // 🔹 MÉTODO ELIMINAR
        public int EliminarProveedor(int id)
        {
            try
            {
                string sql = "DELETE FROM Proveedores WHERE IdProveedor=@IdProveedor";

                using (var cmd = new MySqlCommand(sql, ClassConexion.SQLConnection))
                {
                    cmd.Parameters.AddWithValue("@IdProveedor", id);

                    return cmd.ExecuteNonQuery() == 1 ? 0 : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

    }
}
