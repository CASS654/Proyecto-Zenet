using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SistemaDeVenta.Classusuarios;

namespace SistemaDeVenta
{

    public partial class Usuarios : UserControl
    {
        int IdSeleccionado = 0;
        public string FiltroSeleccionado = "Nombre Completo";
        public Usuarios()
        {
            InitializeComponent();
            CargarUsuarios();
        }
        private void CargarUsuarios()
        {
            ClassTest obj = new ClassTest();
            DataTable registros = obj.ListarRegistros("SELECT * FROM Usuarios");

            List<ClassUsuarios> lista = new List<ClassUsuarios>();

            foreach (DataRow row in registros.Rows)
            {
                lista.Add(new ClassUsuarios
                {
                    IdUsuario = row["IdUsuario"] == DBNull.Value ? 0 : Convert.ToInt32(row["IdUsuario"]),
                    Nombre_Completo = row["Nombre_Completo"] == DBNull.Value ? string.Empty : row["Nombre_Completo"].ToString(),
                    Usser = row["usser"] == DBNull.Value ? string.Empty : row["usser"].ToString(),
                    Password = row["password"] == DBNull.Value ? string.Empty : row["password"].ToString(),
                    Rol = row["rol"] == DBNull.Value ? string.Empty : row["rol"].ToString(),
                    Activo = row["activo"] == DBNull.Value ? false : (Convert.ToInt32(row["activo"]) == 1)
                });
            }

            TablaUsuarios.ItemsSource = lista;
        }
        // Boton guardar
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            ClassUsuarios u = new ClassUsuarios();

            u.Nombre_Completo = txtNombre.Text;
            u.Usser = txtUsuario.Text;
            u.Password = txtClave.Password;
            u.Rol = cbRol.Text;
            u.Activo = (cbEstado.Text == "Activo"); 

            ClassUsuarios db = new ClassUsuarios();
            int resp = db.InsertarUsuario(u);

            if (resp == 0)
            {
                MessageBox.Show("Usuario agregado correctamente");
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Error al agregar usuario");
            }
        }


        //Boton modificar 
        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
            if (IdSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            ClassUsuarios u = new ClassUsuarios
            {
                IdUsuario = IdSeleccionado,
                Nombre_Completo = txtNombre.Text,
                Usser = txtUsuario.Text,
                Password = txtClave.Password,
                Rol = cbRol.Text,
                Activo = (cbEstado.Text == "Activo")
            };

            ClassUsuarios db = new ClassUsuarios();
            int resp = db.EditarUsuario(u);

            if (resp == 0)
            {
                MessageBox.Show("Usuario modificado correctamente.");
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Error al modificar usuario.");
            }
        }


        // Boton modificar
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (IdSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            if (MessageBox.Show("¿Eliminar usuario seleccionado?",
                                "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ClassUsuarios db = new ClassUsuarios();
            int resp = db.EliminarUsuario(IdSeleccionado);

            if (resp == 0)
            {
                MessageBox.Show("Usuario eliminado correctamente.");
                CargarUsuarios();
                IdSeleccionado = 0; // Reseteamos
            }
            else
            {
                MessageBox.Show("Error al eliminar usuario.");
            }
        }

        private void TablaUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablaUsuarios.SelectedItem == null)
                return;

            ClassUsuarios usuario = (ClassUsuarios)TablaUsuarios.SelectedItem;

            // ENVIAR LOS DATOS A LOS TEXTBOX
            txtNombre.Text = usuario.Nombre_Completo;
            txtUsuario.Text = usuario.Usser;
            IdSeleccionado = usuario.IdUsuario;  // 🔥 guardamos el ID
                                                 // PasswordBox usa Password, no Text
            txtClave.Password = usuario.Password;

            // ComboBox
            cbRol.Text = usuario.Rol;
            cbEstado.Text = usuario.Activo ? "Activo" : "Inactivo";
        }

        private void btnFiltro_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)FindResource("MenuFiltroTemplate");

            menu.PlacementTarget = btnFiltro;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; // 👈 Esto lo hace abrirse hacia abajo
            menu.IsOpen = true;
        }

        private void FiltroMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
                FiltroSeleccionado = item.Header.ToString();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            string texto = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                CargarUsuarios();
                return;
            }

            string columna = "";

            switch (FiltroSeleccionado)
            {
                case "Nombre Completo":
                    columna = "Nombre_Completo";
                    break;

                case "Usuario":
                    columna = "usser";
                    break;

                case "Rol":
                    columna = "rol";
                    break;

                case "Estado":
                    columna = "activo";
                    break;
            }

            ClassTest db = new ClassTest();
            DataTable registros;

            // 🔥 Caso especial: Estado (Activo/Inactivo)
            if (columna == "activo")
            {
                int valor = (texto.ToLower() == "activo") ? 1 : 0;

                registros = db.ListarRegistrosConParametro(
                    "SELECT * FROM Usuarios WHERE activo = @valor",
                    "@valor",
                    valor.ToString()
                );
            }
            else
            {
                // 🔥 Búsqueda normal con LIKE
                registros = db.ListarRegistrosConParametro(
                    $"SELECT * FROM Usuarios WHERE {columna} LIKE @param",
                    "@param",
                    "%" + texto + "%"
                );
            }

            // Convertir resultados a la lista
            List<ClassUsuarios> lista = new List<ClassUsuarios>();

            foreach (DataRow row in registros.Rows)
            {
                lista.Add(new ClassUsuarios
                {
                    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                    Nombre_Completo = row["Nombre_Completo"].ToString(),
                    Usser = row["usser"].ToString(),
                    Password = row["password"].ToString(),
                    Rol = row["rol"].ToString(),
                    Activo = Convert.ToInt32(row["activo"]) == 1
                });
            }

            TablaUsuarios.ItemsSource = lista;
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            Buscar_Click(sender, e);
        }
    }
}
