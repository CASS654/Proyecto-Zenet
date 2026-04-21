using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static SistemaDeVenta.Classusuarios;

namespace SistemaDeVentas.Views
{
    public partial class UsuariosView : UserControl
    {
        private bool _panelAbierto = false;
        private bool _modoEdicion = false;

        private ClassUsuarios _usuarioSeleccionado = null;

        public UsuariosView()
        {
            InitializeComponent();
            CargarUsuarios();
        }

        // ───────────────────────────────
        // CARGAR USUARIOS
        // ───────────────────────────────
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
                    Nombre_Completo = row["Nombre_Completo"]?.ToString(),
                    Usser = row["usser"]?.ToString(),
                    Password = row["password"]?.ToString(),
                    Rol = row["rol"]?.ToString(),
                    Activo = row["activo"] != DBNull.Value && Convert.ToInt32(row["activo"]) == 1
                });
            }

            TablaUsuarios.ItemsSource = lista;
        }

        // ───────────────────────────────
        // PANEL
        // ───────────────────────────────
        private void AbrirPanel(bool editar = false)
        {
            _modoEdicion = editar;
            _panelAbierto = true;

            Overlay.Visibility = Visibility.Visible;
            SlidePanel.Visibility = Visibility.Visible;

            if (editar)
            {
                lblPanelTitulo.Text = "EDITAR USUARIO";
                txtBotonGuardar.Text = "GUARDAR CAMBIOS";
                iconBoton.Text = "\uE70F";
            }
            else
            {
                lblPanelTitulo.Text = "NUEVO USUARIO";
                txtBotonGuardar.Text = "CREAR USUARIO";
                iconBoton.Text = "\uE8FA";
            }

            var sb = (Storyboard)Resources["SlideIn"];
            sb.Begin();
        }

        private void CerrarPanel()
        {
            if (!_panelAbierto) return;

            var sb = (Storyboard)Resources["SlideOut"];
            sb.Completed += (s, e) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                SlidePanel.Visibility = Visibility.Collapsed;
                LimpiarFormulario();
            };
            sb.Begin();

            _panelAbierto = false;
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtUsuario.Text = "";
            txtClave.Clear();
            cbRol.SelectedIndex = -1;
            chkActivo.IsChecked = true;
            _usuarioSeleccionado = null;
        }

        // ───────────────────────────────
        // BOTONES
        // ───────────────────────────────
        private void AbrirPanelNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            AbrirPanel(false);
        }

        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ClassUsuarios usuario)
            {
                _usuarioSeleccionado = usuario;

                txtNombre.Text = usuario.Nombre_Completo;
                txtUsuario.Text = usuario.Usser;
                txtClave.Password = usuario.Password;

                SeleccionarEnCombo(cbRol, usuario.Rol);
                chkActivo.IsChecked = usuario.Activo;

                AbrirPanel(true);
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) return;

            ClassUsuarios u = new ClassUsuarios();

            if (_modoEdicion && _usuarioSeleccionado != null)
                u.IdUsuario = _usuarioSeleccionado.IdUsuario;

            u.Nombre_Completo = txtNombre.Text.Trim();
            u.Usser = txtUsuario.Text.Trim();
            u.Password = _mostrarPassword ? txtClaveVisible.Text : txtClave.Password;
            u.Rol = ((ComboBoxItem)cbRol.SelectedItem).Content.ToString();
            u.Activo = chkActivo.IsChecked == true;

            ClassUsuarios db = new ClassUsuarios();

            int resp = (_modoEdicion)
                ? db.EditarUsuario(u)
                : db.InsertarUsuario(u);

            if (resp == 0)
            {
                MessageBox.Show("¡Éxito!");
                CargarUsuarios();
                CerrarPanel();
            }
            else
            {
                MessageBox.Show("Error en la base de datos.");
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ClassUsuarios usuario)
            {
                if (MessageBox.Show("¿Eliminar usuario?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                ClassUsuarios db = new ClassUsuarios();
                int resp = db.EliminarUsuario(usuario.IdUsuario);

                if (resp == 0)
                {
                    MessageBox.Show("Usuario eliminado");
                    CargarUsuarios();
                }
                else
                {
                    MessageBox.Show("Error al eliminar");
                }
            }
        }

        private void CerrarPanel_Click(object sender, RoutedEventArgs e) => CerrarPanel();

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => CerrarPanel();

        // ───────────────────────────────
        // UTILIDADES
        // ───────────────────────────────
        private void SeleccionarEnCombo(ComboBox combo, string valor)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString().Trim().Equals(valor?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Nombre requerido");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Usuario requerido");
                return false;
            }

            if (cbRol.SelectedIndex < 0)
            {
                MessageBox.Show("Selecciona un rol");
                return false;
            }

            return true;
        }
        private bool _mostrarPassword = false;

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _mostrarPassword = !_mostrarPassword;

            if (_mostrarPassword)
            {
                txtClaveVisible.Text = txtClave.Password;
                txtClave.Visibility = Visibility.Collapsed;
                txtClaveVisible.Visibility = Visibility.Visible;

                iconEye.Text = "\uE70F"; // ojo abierto
            }
            else
            {
                txtClave.Password = txtClaveVisible.Text;
                txtClaveVisible.Visibility = Visibility.Collapsed;
                txtClave.Visibility = Visibility.Visible;

                iconEye.Text = "\uED1A"; // ojo cerrado
            }
        }

        private void cbRol_SelectionChanged(object sender, SelectionChangedEventArgs e) { }


    }
}