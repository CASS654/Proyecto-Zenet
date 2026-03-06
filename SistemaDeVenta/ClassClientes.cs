using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;
namespace Sistema_Bancario
{
    public class ClassClientes
    {
        public int idCliente { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string nombre { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string telefono { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string beneficiario { get; set; }

        // El codigo que nos instera un registro en la tabla 
        //INSERT SQL
        public int InsertarByTransaction( ClassClientes vObjeto)
        {
            int vResultado = 1; // Inicializa como error

            using (MySqlConnection connection = new MySqlConnection("Database=" + globales.vBaseDeDatosConnect_Global + "; Data Source=" + globales.vServidorConnect_Global + "; User Id=" + globales.vUsuarioINIConnect_Global + "; Password=" + globales.vPassUsuarioINIConnect_Global + ";CharSet=utf8;"))
            {
                connection.Open();
                // Utiliza una transacción explícita para la inserción
                MySqlTransaction trans = null;

                try
                {
                    // Inicia la transacción
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    string vSQL = "INSERT INTO clientes " +
                        "(apellido_paterno,apellido_materno,nombre,fecha_nacimiento,telefono,curp,RFC,correo_electronico,direccion,Beneficiario) VALUES " +
                        "(@apellido_paterno,@apellido_materno,@nombre,@fecha_nacimiento,@telefono,@curp,@RFC,@correo_electronico,@direccion,@Beneficiario)";
                    cmd.CommandText = vSQL;

                    cmd.Parameters.AddWithValue("@apellido_paterno", vObjeto.apellido_paterno);
                    cmd.Parameters.AddWithValue("@apellido_materno", vObjeto.apellido_materno);
                    cmd.Parameters.AddWithValue("@nombre", vObjeto.nombre);
                    cmd.Parameters.AddWithValue("@fecha_nacimiento", vObjeto.fecha_nacimiento);
                    cmd.Parameters.AddWithValue("@telefono", vObjeto.telefono);
                    cmd.Parameters.AddWithValue("@curp", vObjeto.CURP);
                    cmd.Parameters.AddWithValue("@RFC", vObjeto.RFC);
                    cmd.Parameters.AddWithValue("@correo_electronico", vObjeto.email);
                    cmd.Parameters.AddWithValue("@direccion", vObjeto.direccion);
                    cmd.Parameters.AddWithValue("@Beneficiario", vObjeto.beneficiario);


                    // Ejecuta la consulta
                    cmd.ExecuteNonQuery();

                    // Completa la transacción
                    trans.Commit();

                    vResultado = 0; // Éxito
                }
                catch (Exception ex)
                {
                    // Si hay un error, realiza un rollback de la transacción
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    //MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Cierra la conexión
                    connection.Close();
                }
            }

            return vResultado;
        }

        public int EditartarByTransaction(ClassClientes vObjeto)
        {
            int vResultado = 1; // Inicializa como error

            using (MySqlConnection connection = new MySqlConnection("Database=" + globales.vBaseDeDatosConnect_Global + "; Data Source=" + globales.vServidorConnect_Global + "; User Id=" + globales.vUsuarioINIConnect_Global + "; Password=" + globales.vPassUsuarioINIConnect_Global + ";CharSet=utf8;"))
            {
                connection.Open();
                // Utiliza una transacción explícita para la inserción
                MySqlTransaction trans = null;

                try
                {
                    // Inicia la transacción
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    string vSQL = "UPDATE clientes " +
                        " SET apellido_paterno=@apellidopaterno, apellido_materno=@apellidomaterno, nombre=@nombre, fecha_nacimiento=@fechanacimiento, telefono=@telefono, curp=@curp, RFC =@RFC, correo_electronico=@email, direccion=@direccion, beneficiario=@beneficiario  " +
                        " WHERE " +
                        " IdCliente='" + vObjeto.idCliente + "';";
                    cmd.CommandText = vSQL;

                    cmd.Parameters.AddWithValue("@apellidopaterno", vObjeto.apellido_paterno);
                    cmd.Parameters.AddWithValue("@apellidomaterno", vObjeto.apellido_materno);
                    cmd.Parameters.AddWithValue("@nombre", vObjeto.nombre);
                    cmd.Parameters.AddWithValue("@fechanacimiento", vObjeto.fecha_nacimiento);
                    cmd.Parameters.AddWithValue("@telefono", vObjeto.telefono);
                    cmd.Parameters.AddWithValue("@curp", vObjeto.CURP);
                    cmd.Parameters.AddWithValue("@RFC", vObjeto.RFC);
                    cmd.Parameters.AddWithValue("@email", vObjeto.email);
                    cmd.Parameters.AddWithValue("@direccion", vObjeto.direccion);
                    cmd.Parameters.AddWithValue("@beneficiario", vObjeto.beneficiario);


                    // Ejecuta la consulta
                    cmd.ExecuteNonQuery();

                    // Completa la transacción
                    trans.Commit();

                    vResultado = 0; // Éxito
                }
                catch (Exception ex)
                {
                    // Si hay un error, realiza un rollback de la transacción
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Cierra la conexión
                    connection.Close();
                }
            }

            return vResultado;
        }

        // Eliminar 
        public int EliminarByTransaction(ClassClientes vObjeto)
        {
            int vResultado = 1; // Inicializa como error

            using (MySqlConnection connection = new MySqlConnection("Database=" + globales.vBaseDeDatosConnect_Global + "; Data Source=" + globales.vServidorConnect_Global + "; User Id=" + globales.vUsuarioINIConnect_Global + "; Password=" + globales.vPassUsuarioINIConnect_Global + ";CharSet=utf8;"))
            {
                connection.Open();
                // Utiliza una transacción explícita para la inserción
                MySqlTransaction trans = null;

                try
                {
                    // Inicia la transacción
                    trans = connection.BeginTransaction();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = connection;

                    string vSQL = "DELETE FROM clientes " +
      
                        " WHERE " +
                        " IdCliente='" + vObjeto.idCliente + "';";
                    cmd.CommandText = vSQL;


                    // Ejecuta la consulta
                    cmd.ExecuteNonQuery();

                    // Completa la transacción
                    trans.Commit();

                    vResultado = 0; // Éxito
                }
                catch (Exception ex)
                {
                    // Si hay un error, realiza un rollback de la transacción
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Cierra la conexión
                    connection.Close();
                }
            }

            return vResultado;
        }

        public DataTable ListarRegistros(string vSQL)
        {
            MySqlDataAdapter vAdaptador = new MySqlDataAdapter(vSQL, ClassConexion.SQLConnection);
            DataTable vTable = new DataTable();
            vAdaptador.Fill(vTable);

            ClassConexion.SQLConnection.Close();
            ClassConexion.SQLConnection.Dispose();
            return vTable;
        }
        public bool ExisteClienteUnico(string curp, string rfc, string email)
        {
            bool existe = false;

            using (MySqlConnection connection = new MySqlConnection("Database=" + globales.vBaseDeDatosConnect_Global + "; Data Source=" + globales.vServidorConnect_Global + "; User Id=" + globales.vUsuarioINIConnect_Global + "; Password=" + globales.vPassUsuarioINIConnect_Global + ";CharSet=utf8;"))
            {
                try
                {
                    connection.Open();

                    string consulta = "SELECT COUNT(*) FROM clientes WHERE curp = @curp OR RFC = @rfc OR correo_electronico = @correo";
                    MySqlCommand cmd = new MySqlCommand(consulta, connection);

                    cmd.Parameters.AddWithValue("@curp", curp);
                    cmd.Parameters.AddWithValue("@rfc", rfc);
                    cmd.Parameters.AddWithValue("@correo", email);

                    int total = Convert.ToInt32(cmd.ExecuteScalar());

                    if (total > 0)
                    {
                        existe = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al verificar duplicados: " + ex.Message);
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
