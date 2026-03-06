using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaDeVenta
{
    public partial class LoginPage
    {
        private bool isUsernamePlaceholder = true;
        private bool passwordVisible = false;
        string usuario = "";
        string clave = "";
        string rol = "";


        public LoginPage()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            InitializeComponent();
            UsernameTextBox.Focus();
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isUsernamePlaceholder)
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.Foreground = Brushes.White;
                isUsernamePlaceholder = false;
            }
        }
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
        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Enter your username";
                UsernameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(107, 114, 128));
                isUsernamePlaceholder = true;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UsernameTexBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 1. Obtener usuario
            usuario = UsernameTextBox.Text.Trim();

            // 2. Obtener password según modo
            if (PasswordHidden.Visibility == Visibility.Visible)
            {
                clave = PasswordHidden.Password;
            }
            else
            {
                clave = PasswordVisible.Text;
            }

            // 3. Validar campos vacíos
            if (string.IsNullOrWhiteSpace(usuario))
            {
                MessageBox.Show("Ingrese el usuario", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Ingrese la contraseña", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Conectar BD
            if (!ConectarDB("localhost", "Fruteria", "root", "Cesar654"))
            {
                MessageBox.Show("No se pudo conectar a la base de datos",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 5. Validar usuario
            if (ValidarUsuario(usuario, clave))
            {
                // Obtener rol
                rol = ResultadoLogin(usuario, clave);

                // Crear ventanas
                Menu ventana = new Menu();
                MenuVendedor menuVendedor = new MenuVendedor();
                MenuGerente menuGerente = new MenuGerente();
                AdministradorWindow admin = new AdministradorWindow();
                // 6. Abrir según rol
                switch (rol)
                {
                    case "Admin":

                        //MessageBox.Show("Bienvenido Administrador: " + usuario,
                        //                "Acceso Permitido",
                        //                MessageBoxButton.OK,
                        //                MessageBoxImage.Information);

                        ventana.Show();
                        this.Close();
                        break;


                    case "Cajero":

                        admin.Show();
                        this.Close();
                        break;


                    case "Gerente":

                        menuGerente.Show();
                        this.Close();
                        break;


                    default:

                        MessageBox.Show("Rol no reconocido",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                UsernameTextBox.Clear();
                PasswordHidden.Clear();
                PasswordVisible.Clear();

                UsernameTextBox.Focus();
            }
        }



        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!passwordVisible)
            {
                PasswordVisible.Text = PasswordHidden.Password;
                PasswordHidden.Visibility = Visibility.Collapsed;
                PasswordVisible.Visibility = Visibility.Visible;
                passwordVisible = true;
            }
            else
            {
                PasswordHidden.Password = PasswordVisible.Text;
                PasswordVisible.Visibility = Visibility.Collapsed;
                PasswordHidden.Visibility = Visibility.Visible;
                passwordVisible = false;
            }
        }

        private void UsernameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Si está oculto el password
                if (PasswordHidden.Visibility == Visibility.Visible)
                {
                    PasswordHidden.Focus();
                }
                else
                {
                    PasswordVisible.Focus();
                }
            }
        }

        private void PasswordHidden_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);  // llama al botón aceptar
            }
        }
    }
}