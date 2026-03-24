using MySql.Data.MySqlClient;
using Sistema_Bancario;
using SistemaDeVentaPrueba;
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
            return resultadoConexion == 0;
        }

        // 🔥 VALIDACIÓN COMPLETA (usuario + password + activo + rol)
        private string ValidarUsuario(string usuario, string clave)
        {
            string query = "SELECT Rol, activo FROM Usuarios WHERE usser = @user AND password = @pass";

            using (MySqlCommand cmd = new MySqlCommand(query, ClassConexion.SQLConnection))
            {
                cmd.Parameters.AddWithValue("@user", usuario);
                cmd.Parameters.AddWithValue("@pass", clave);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool activo = Convert.ToBoolean(reader["activo"]);

                        if (!activo)
                        {
                            return "INACTIVO";
                        }

                        return reader["Rol"].ToString();
                    }
                }
            }

            return "NO_EXISTE";
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            usuario = UsernameTextBox.Text.Trim();

            if (PasswordHidden.Visibility == Visibility.Visible)
                clave = PasswordHidden.Password;
            else
                clave = PasswordVisible.Text;

            // 1. Validar campos vacíos
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Ingrese usuario y contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LimpiarCampos();
                return;
            }

            // 2. Conectar BD
            if (!ConectarDB("localhost", "Fruteria2", "root", "Cesar654"))
            {
                MessageBox.Show("No se pudo conectar a la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Validar Usuario
            string resultado = ValidarUsuario(usuario, clave);

            if (resultado == "NO_EXISTE")
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LimpiarCampos();
                return;
            }

            if (resultado == "INACTIVO")
            {
                MessageBox.Show("El usuario está desactivado. Contacte al administrador.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Stop);
                LimpiarCampos();
                return;
            }

            // 4. Si llegamos aquí, el login es exitoso y 'resultado' contiene el ROL
            rol = resultado;

            // Instanciar ventanas
            switch (rol)
            {
                case "Admin":
                    AdministradorWindow admin = new AdministradorWindow();
                    admin.Show();
                    Window.GetWindow(this).Close(); // Forma segura de cerrar la ventana actual en WPF
                    break;

                case "Cajero":
                    VetanaCobro cobro = new VetanaCobro();
                    cobro.Show();
                    Window.GetWindow(this).Close();
                    break;

                case "Gerente":
                    MenuGerente menuGerente = new MenuGerente();
                    menuGerente.Show();
                    Window.GetWindow(this).Close();
                    break;

                default:
                    MessageBox.Show("Rol no reconocido: " + rol, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LimpiarCampos();
                    break;
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

        private void UsernameTextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (PasswordHidden.Visibility == Visibility.Visible)
                    PasswordHidden.Focus();
                else
                    PasswordVisible.Focus();
            }
        }

        private void PasswordHidden_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("¡Recuperación de contraseña no implementada aún!", "Información",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void LimpiarCampos()
        {
            UsernameTextBox.Clear();
            PasswordHidden.Clear();
            PasswordVisible.Clear();
            UsernameTextBox.Focus();
        }
    }
}