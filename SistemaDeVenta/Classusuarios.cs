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
    internal class Classusuarios
    {
        public class ClassUsuarios
        {
            public int IdUsuario { get; set; }
            public string Nombre_Completo { get; set; }
            public string Usser { get; set; }
            public string Password { get; set; }
            public string Rol { get; set; }
            public bool Activo { get; set; } // 1 = Activo, 0 = Inactivo

            // INSERTAR USUARIO
            public int InsertarUsuario(ClassUsuarios obj)
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

                        cmd.CommandText =
                            "INSERT INTO usuarios (Nombre_Completo, usser, password, rol, activo) " +
                            "VALUES (@nombre, @usser, @password, @rol, @activo)";

                        cmd.Parameters.AddWithValue("@nombre", obj.Nombre_Completo);
                        cmd.Parameters.AddWithValue("@usser", obj.Usser);
                        cmd.Parameters.AddWithValue("@password", obj.Password);
                        cmd.Parameters.AddWithValue("@rol", obj.Rol);
                        cmd.Parameters.AddWithValue("@activo", obj.Activo);

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

            // EDITAR USUARIO
            public int EditarUsuario(ClassUsuarios obj)
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

                        cmd.CommandText =
                            "UPDATE usuarios SET " +
                            "Nombre_Completo=@nombre, usser=@usser, password=@password, rol=@rol, activo=@activo " +
                            "WHERE IdUsuario=@id";

                        cmd.Parameters.AddWithValue("@nombre", obj.Nombre_Completo);
                        cmd.Parameters.AddWithValue("@usser", obj.Usser);
                        cmd.Parameters.AddWithValue("@password", obj.Password);
                        cmd.Parameters.AddWithValue("@rol", obj.Rol);
                        cmd.Parameters.AddWithValue("@activo", obj.Activo);
                        cmd.Parameters.AddWithValue("@id", obj.IdUsuario);

                        cmd.ExecuteNonQuery();
                        trans.Commit();

                        resultado = 0;
                    }
                    catch (Exception ex)
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

            // ELIMINAR USUARIO
            public int EliminarUsuario(int id)
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

                        cmd.CommandText = "DELETE FROM usuarios WHERE IdUsuario=@id";
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

            // LISTAR USUARIOS
            public DataTable Listar(string sql)
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(sql, ClassConexion.SQLConnection);
                DataTable table = new DataTable();
                adapt.Fill(table);

                ClassConexion.SQLConnection.Close();
                ClassConexion.SQLConnection.Dispose();

                return table;
            }

            // VALIDAR SI EXISTE USUARIO (username)
            public bool ExisteUsuario(string usuario)
            {
                bool existe = false;

                using (MySqlConnection connection = new MySqlConnection(
                    "Database=" + globales.vBaseDeDatosConnect_Global +
                    "; Data Source=" + globales.vServidorConnect_Global +
                    "; User Id=" + globales.vUsuarioINIConnect_Global +
                    "; Password=" + globales.vPassUsuarioINIConnect_Global +
                    ";CharSet=utf8;"))
                {
                    try
                    {
                        connection.Open();

                        string sql = "SELECT COUNT(*) FROM usuarios WHERE usser=@usser";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);

                        cmd.Parameters.AddWithValue("@usser", usuario);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        existe = count > 0;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                return existe;
            }
        }

    }
}
