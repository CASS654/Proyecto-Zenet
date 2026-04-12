using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SistemaDeVenta.Classusuarios;

namespace SistemaDeVentas.Views
{
    public partial class UsuariosView : UserControl
    {
        int IdSeleccionado = 0;

        public UsuariosView()
        {
            InitializeComponent();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            ClassUsuarios db = new ClassUsuarios();
            DataTable registros = db.Listar("SELECT * FROM usuarios");

            List<ClassUsuarios> lista = new List<ClassUsuarios>();

            foreach (DataRow row in registros.Rows)
            {
                lista.Add(new ClassUsuarios
                {
                    IdUsuario = row["IdUsuario"] == DBNull.Value ? 0 : Convert.ToInt32(row["IdUsuario"]),
                    Nombre_Completo = row["Nombre_Completo"] == DBNull.Value ? "" : row["Nombre_Completo"].ToString(),
                    Usser = row["usser"] == DBNull.Value ? "" : row["usser"].ToString(),
                    Password = row["password"] == DBNull.Value ? "" : row["password"].ToString(),
                    Rol = row["rol"] == DBNull.Value ? "" : row["rol"].ToString(),
                    Activo = row["activo"] == DBNull.Value ? false : (Convert.ToInt32(row["activo"]) == 1)
                });
            }

            TablaUsuarios.ItemsSource = lista;
        }

        // Este método ahora se encarga de CARGAR los datos al formulario desde el botón de la tabla
        // 1. CARGAR DATOS AL FORMULARIO
        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is ClassUsuarios usuario)
            {
                IdSeleccionado = usuario.IdUsuario;
                txtNombre.Text = usuario.Nombre_Completo;
                txtUsuario.Text = usuario.Usser;
                txtClave.Password = usuario.Password;

                string rolBD = usuario.Rol?.Trim();
                SeleccionarEnCombo(cbRol, rolBD);

                string estadoTexto = usuario.Activo ? "Activo" : "Inactivo";
                SeleccionarEnCombo(cbEstado, estadoTexto);

                // CAMBIO DE TEXTO E ICONO (Opcional)
                txtBotonGuardar.Text = "Guardar Cambios";
                iconBoton.Text = "\uE70F";
                btnCancelar.Visibility = Visibility.Visible;
            }
        }
        private void Limpiar()
        {
            txtNombre.Text = "";
            txtUsuario.Text = "";
            txtClave.Password = "";
            cbRol.SelectedIndex = -1;
            cbEstado.SelectedIndex = -1;
            IdSeleccionado = 0;

            // Restaurar interfaz original
            IdSeleccionado = 0;
            txtBotonGuardar.Text = "Guardar Usuario";
            iconBoton.Text = "\uE74E";
            btnCancelar.Visibility = Visibility.Collapsed; // OCULTAR botón cancelar
        }
        // Método auxiliar infalible
        private void SeleccionarEnCombo(ComboBox combo, string valorABuscar)
        {
            if (string.IsNullOrEmpty(valorABuscar)) return;

            foreach (var item in combo.Items)
            {
                if (item is ComboBoxItem cbItem)
                {
                    if (cbItem.Content.ToString().Trim().Equals(valorABuscar.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        combo.SelectedItem = cbItem;
                        return;
                    }
                }
            }
        }


        // 2. LÓGICA DE GUARDAR (INSERTAR O EDITAR)
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Aseguramos que los combos tengan selección
            if (cbRol.SelectedIndex == -1 || cbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione Rol y Estado");
                return;
            }

            ClassUsuarios u = new ClassUsuarios();
            u.IdUsuario = IdSeleccionado;
            u.Nombre_Completo = txtNombre.Text.Trim();
            u.Usser = txtUsuario.Text.Trim();
            u.Password = txtClave.Password; // Verifica si tu CLASE encripta esto o si debes hacerlo aquí

            // Forma más segura de obtener el texto del combo
            u.Rol = ((ComboBoxItem)cbRol.SelectedItem).Content.ToString();
            u.Activo = ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString() == "Activo";

            ClassUsuarios db = new ClassUsuarios();
            // Aquí es donde se ejecuta la lógica que dices que está bien
            int resp = (IdSeleccionado == 0) ? db.InsertarUsuario(u) : db.EditarUsuario(u);

            if (resp == 0)
            {
                MessageBox.Show("¡Éxito!");
                CargarUsuarios();
                Limpiar();
            }
            else
            {
                // Si entra aquí, la CLASE devolvió error aunque digas que está bien. 
                // Revisa que los nombres de los parámetros en la clase coincidan con 'u.Nombre_Completo', etc.
                MessageBox.Show("Error al ejecutar en la base de datos.");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar(); // Al cancelar, simplemente reseteamos todo
        }
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            // Si el click viene de la tabla, obtenemos el ID primero
            if (sender is Button boton)
            {
                var usuario = boton.DataContext as ClassUsuarios;
                if (usuario != null) IdSeleccionado = usuario.IdUsuario;
            }

            if (IdSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            if (MessageBox.Show("¿Eliminar usuario seleccionado?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ClassUsuarios db = new ClassUsuarios();
            int resp = db.EliminarUsuario(IdSeleccionado);

            if (resp == 0)
            {
                MessageBox.Show("Usuario eliminado correctamente.");
                Limpiar();
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Error al eliminar usuario.");
            }
        }

        private void cbRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}