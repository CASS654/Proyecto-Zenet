
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Sistema_Bancario
{
    public class ClassTest
    {
       
        public DataTable ListarRegistros(string vSQL)
        {
            MySqlDataAdapter vAdaptador = new MySqlDataAdapter(vSQL, ClassConexion.SQLConnection);
            DataTable vTable = new DataTable();
            vAdaptador.Fill(vTable);
            return vTable;
        }
        public DataTable ListarRegistrosConParametro(string query, string paramName, string paramValue)
        {
            using (var cmd = new MySqlCommand(query, ClassConexion.SQLConnection))
            {
                cmd.Parameters.AddWithValue(paramName, paramValue);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
