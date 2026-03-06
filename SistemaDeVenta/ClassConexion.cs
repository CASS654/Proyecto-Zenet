using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;


namespace Sistema_Bancario
{
    //cambio prueba
    public class ClassConexion
    {
        public string vServidor { get; set; }
        public string vUsuario { get; set; }
        public string vPassword { get; set; }
        public string vBaseDeDatos { get; set; }

        public static MySqlConnection SQLConnection = new MySqlConnection();
        string vServerString = "";
        string vEstatus_Conexion = "";

        public int ABRIR_CONEXION_DB_MYSQL(ClassConexion vObjConexion)
        {
            int vResultado = 1;
            vServerString = ("Server="
                        + (vObjConexion.vServidor + (";" + ("user Id="
                        + (vObjConexion.vUsuario + (";" + ("Password="
                        + (vObjConexion.vPassword + (";" + ("Database=" + vObjConexion.vBaseDeDatos))))))))));
            SQLConnection.Close();
            SQLConnection.ConnectionString = vServerString;

            globales.vBaseDeDatosConnect_Global = vObjConexion.vBaseDeDatos;
            globales.vServidorConnect_Global = vObjConexion.vServidor;
            globales.vUsuarioINIConnect_Global = vObjConexion.vUsuario;
            globales.vPassUsuarioINIConnect_Global = vObjConexion.vPassword;

            try
            {
                if ((SQLConnection.State == ConnectionState.Closed))
                {

                    SQLConnection.Open();
                    vResultado = 0;
                    vEstatus_Conexion = "OPEN";
                    return vResultado;
                }
                else
                {
                    SQLConnection.Close();
                    vResultado = 1;
                    vEstatus_Conexion = "CLOSE";
                    return vResultado;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

            return vResultado;
        }
    }
}
