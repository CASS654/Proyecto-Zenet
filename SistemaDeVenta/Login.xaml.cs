using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;


namespace SistemaDeVenta
{
    public partial class Login : Window
    {
        string usuario = "";
        string clave = "";
        string rol = "";

        public Login()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            InitializeComponent();
            txtUsuario.Focus();
        }

        // =========================
        //  CONECTAR A MYSQL
        // =========================
        private bool ConectarDB(string server, string database, string user, string password)
        {
            ClassConexion objConexion = new ClassConexion();
            objConexion.vServidor = server;
            objConexion.vBaseDeDatos = database;
            objConexion.vUsuario = user;
            objConexion.vPassword = password;

            int resultadoConexion = objConexion.ABRIR_CONEXION_DB_MYSQL(objConexion);
            return resultadoConexion == 0; // True si conecta
        }

        // =========================
        //  VALIDAR USUARIO EN BD
        // =========================
        private bool ValidarUsuario(string usuario, string clave)
        {
            string query = "SELECT * FROM Usuarios WHERE usser = @user AND password = @pass";

            using (MySqlCommand cmd = new MySqlCommand(query, ClassConexion.SQLConnection))
            {
                cmd.Parameters.AddWithValue("@user", usuario);
                cmd.Parameters.AddWithValue("@pass", clave);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    return reader.HasRows; // Existe → Login válido
                }
            }
        }
        private string ResultadoLogin(string usuario, string clave)
        {
            string query = "SELECT Rol FROM Usuarios WHERE usser = @user AND password = @pass";

            using (MySqlCommand cmd = new MySqlCommand(query, ClassConexion.SQLConnection))
            {
                cmd.Parameters.AddWithValue("@user", usuario);
                cmd.Parameters.AddWithValue("@pass", clave);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())  // existe
                    { 
                        {
                            return rol = reader["Rol"].ToString();
                        }
                        ;
                    }
                }
            }
            return ""; // No existe
        }

        // =========================
        //  CLICK EN ACEPTAR
        // =========================
        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            usuario = txtUsuario.Text;
            clave = txtClave.Password;
            Menu ventana = new Menu();
            MenuVendedor menuVendedor = new MenuVendedor();
            MenuGerente menuGerente = new MenuGerente();

            // 1. Conectar a BD
            if (!ConectarDB("localhost", "fruteria2", "root", "felixeduardo200605#"))  // <-- AJUSTA TUS DATOS
            {
                MessageBox.Show("No se pudo conectar con la base de datos.");
                return;
            }

            // 2. Validar usuario
            if (ValidarUsuario(usuario, clave))
            {
                ResultadoLogin(usuario, clave);
                switch (rol)
                {
                    case "Admin":
                        MessageBox.Show("Buen dia Administrador: " + usuario,
                                        "Acceso Permitido",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                                        ventana.Show();
                                        this.Close();
                        break;


                    case "Cajero":
                        menuVendedor.Show();
                        this.Close();
                        break;


                    case "Gerente":

                        menuGerente.Show();
                        this.Close();
                        break;


                    default:
                        MessageBox.Show("Rol no reconocido o inválido",
                                        "Error");
                                        
                        return; 
                }
                
                
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsuario.Text = "";
                txtClave.Password = "";
                txtUsuario.Focus();
            }
        }

        private void txtUsuario_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtClave.Focus();   // pasa a la contraseña
            }
        }

        private void txtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Aceptar_Click(sender, e);  // llama al botón aceptar
            }
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
