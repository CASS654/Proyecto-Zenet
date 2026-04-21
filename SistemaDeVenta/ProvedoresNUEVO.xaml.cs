using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static SistemaDeVenta.ClassProveedores;

namespace SistemaDeVenta.Views
{
    public partial class ProveedoresNUEVO : UserControl
    {
        private bool _panelAbierto = false;
        private bool _modoEdicion = false;
        private Proveedores1 _proveedorSeleccionado = null;

        public ProveedoresNUEVO()
        {
            InitializeComponent();
            CargarProveedores();
        }

        // ───────────────────────────────
        // CARGAR PROVEEDORES
        // ───────────────────────────────
        private void CargarProveedores()
        {
            try
            {
                ClassTest obj = new ClassTest();
                DataTable registros = obj.ListarRegistros("SELECT * FROM Proveedores");

                List<Proveedores1> lista = new List<Proveedores1>();

                foreach (DataRow row in registros.Rows)
                {
                    lista.Add(new Proveedores1
                    {
                        IdProveedor = row["IdProveedor"] == DBNull.Value ? 0 : Convert.ToInt32(row["IdProveedor"]),
                        Nombre = row["Nombre"] == DBNull.Value ? string.Empty : row["Nombre"].ToString(),
                        Telefono = row["Telefono"] == DBNull.Value ? string.Empty : row["Telefono"].ToString(),
                        Direccion = row["Direccion"] == DBNull.Value ? string.Empty : row["Direccion"].ToString()
                    });
                }

                TablaProveedores.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
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
                lblPanelTitulo.Text = "EDITAR PROVEEDOR";
                lblPanelSubtitulo.Text = $"EDITAR_PROVEEDOR : #PR-{_proveedorSeleccionado?.IdProveedor}";
                txtBotonGuardar.Text = "GUARDAR CAMBIOS";
                iconBoton.Text = "\uE70F";
            }
            else
            {
                lblPanelTitulo.Text = "NUEVO PROVEEDOR";
                lblPanelSubtitulo.Text = "NUEVO_PROVEEDOR : #—";
                txtBotonGuardar.Text = "GUARDAR PROVEEDOR";
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
            txtNombreProveedor.Text = "";
            txtTelefonoProveedor.Text = "";
            txtDireccionProveedor.Text = "";
            _proveedorSeleccionado = null;
        }

        // ───────────────────────────────
        // BOTONES
        // ───────────────────────────────
        private void AbrirPanelNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            AbrirPanel(false);
        }

        private void ModificarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Proveedores1 proveedor)
            {
                _proveedorSeleccionado = proveedor;

                txtNombreProveedor.Text = proveedor.Nombre;
                txtTelefonoProveedor.Text = proveedor.Telefono;
                txtDireccionProveedor.Text = proveedor.Direccion;

                AbrirPanel(true);
            }
        }

        private void GuardarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) return;

            ClassProveedores db = new ClassProveedores();
            int resp;

            if (_modoEdicion && _proveedorSeleccionado != null)
            {
                _proveedorSeleccionado.Nombre = txtNombreProveedor.Text.Trim();
                _proveedorSeleccionado.Telefono = txtTelefonoProveedor.Text.Trim();
                _proveedorSeleccionado.Direccion = txtDireccionProveedor.Text.Trim();

                resp = db.EditarProveedor(_proveedorSeleccionado);
            }
            else
            {
                var nuevo = new Proveedores1
                {
                    Nombre = txtNombreProveedor.Text.Trim(),
                    Telefono = txtTelefonoProveedor.Text.Trim(),
                    Direccion = txtDireccionProveedor.Text.Trim()
                };

                resp = db.InsertarProveedor(nuevo);
            }

            if (resp == 0)
            {
                MessageBox.Show("¡Éxito!");
                CargarProveedores();
                CerrarPanel();
            }
            else
            {
                MessageBox.Show("Error en la base de datos.");
            }
        }

        private void EliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Proveedores1 proveedor)
            {
                if (MessageBox.Show("¿Eliminar proveedor?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                ClassProveedores db = new ClassProveedores();
                int resp = db.EliminarProveedor(proveedor.IdProveedor);

                if (resp == 0)
                {
                    MessageBox.Show("Proveedor eliminado.");
                    CargarProveedores();
                }
                else
                {
                    MessageBox.Show("Error al eliminar.");
                }
            }
        }

        private void CerrarPanel_Click(object sender, RoutedEventArgs e) => CerrarPanel();

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => CerrarPanel();

        // ───────────────────────────────
        // VALIDACIÓN
        // ───────────────────────────────
        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return false;
            }

            return true;
        }
    }
}