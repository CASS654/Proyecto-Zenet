using Sistema_Bancario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using static SistemaDeVenta.ClassProveedores;

namespace SistemaDeVenta.Views
{
    public partial class ProveedoresNUEVO : UserControl
    {
        private List<Proveedores1> listaProveedores = new List<Proveedores1>();

        public ProveedoresNUEVO()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            try
            {
                ClassTest obj = new ClassTest();
                DataTable registros = obj.ListarRegistros("SELECT * FROM Proveedores");

                listaProveedores.Clear();

                foreach (DataRow row in registros.Rows)
                {
                    listaProveedores.Add(new Proveedores1
                    {
                        IdProveedor = row["IdProveedor"] == DBNull.Value ? 0 : Convert.ToInt32(row["IdProveedor"]),
                        Nombre = row["Nombre"] == DBNull.Value ? string.Empty : row["Nombre"].ToString(),
                        Telefono = row["Telefono"] == DBNull.Value ? string.Empty : row["Telefono"].ToString(),
                        Direccion = row["Direccion"] == DBNull.Value ? string.Empty : row["Direccion"].ToString()
                    });
                }

                TablaProveedores.ItemsSource = null;
                TablaProveedores.Items.Clear();
                TablaProveedores.ItemsSource = listaProveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }

        private void GuardarProveedor_Click(object sender, RoutedEventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            ClassProveedores db = new ClassProveedores();
            int resp;

            // LÓGICA DE DECISIÓN: ¿Estamos editando o insertando?
            if (TablaProveedores.SelectedItem != null)
            {
                // --- MODO EDICIÓN ---
                Proveedores1 proveedorExistente = (Proveedores1)TablaProveedores.SelectedItem;

                // Actualizamos los datos del objeto con lo que hay en los TextBox
                proveedorExistente.Nombre = txtNombreProveedor.Text;
                proveedorExistente.Telefono = txtTelefonoProveedor.Text;
                proveedorExistente.Direccion = txtDireccionProveedor.Text;

                resp = db.EditarProveedor(proveedorExistente);

                if (resp == 0) MessageBox.Show("Proveedor actualizado correctamente.");
            }
            else
            {
                // --- MODO NUEVO ---
                var nuevoProveedor = new Proveedores1
                {
                    Nombre = txtNombreProveedor.Text,
                    Telefono = txtTelefonoProveedor.Text,
                    Direccion = txtDireccionProveedor.Text
                };

                resp = db.InsertarProveedor(nuevoProveedor);

                if (resp == 0) MessageBox.Show("Proveedor agregado correctamente.");
            }

            // Si la operación fue exitosa (resp == 0), refrescamos la vista
            if (resp == 0)
            {
                CargarProveedores();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Ocurrió un error en la base de datos.");
            }
        }

        // Este método ahora solo sirve para CARGAR los datos al formulario
        private void ModificarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var proveedor = boton.DataContext as Proveedores1;

            if (proveedor != null)
            {
                // Cargamos los datos en los campos
                txtNombreProveedor.Text = proveedor.Nombre;
                txtTelefonoProveedor.Text = proveedor.Telefono;
                txtDireccionProveedor.Text = proveedor.Direccion;

                // Seleccionamos el ítem en la tabla para que 'Guardar' sepa que es una edición
                TablaProveedores.SelectedItem = proveedor;
            }
        }

        // MUY IMPORTANTE: Limpiar la selección al limpiar campos
        private void LimpiarCampos()
        {
            txtNombreProveedor.Text = "";
            txtTelefonoProveedor.Text = "";
            txtDireccionProveedor.Text = "";
            TablaProveedores.SelectedItem = null; // Reset de la selección
        }

        private void EliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (TablaProveedores.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            Proveedores1 proveedor = (Proveedores1)TablaProveedores.SelectedItem;

            if (MessageBox.Show("¿Eliminar proveedor seleccionado?",
                                "Confirmar",
                                MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ClassProveedores db = new ClassProveedores();
            int resp = db.EliminarProveedor(proveedor.IdProveedor);

            if (resp == 0)
            {
                MessageBox.Show("Proveedor eliminado correctamente.");
                CargarProveedores();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al eliminar proveedor.");
            }
        }

        private void TablaProveedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablaProveedores.SelectedItem == null)
                return;

            Proveedores1 proveedor = (Proveedores1)TablaProveedores.SelectedItem;

            txtNombreProveedor.Text = proveedor.Nombre;
            txtTelefonoProveedor.Text = proveedor.Telefono;
            txtDireccionProveedor.Text = proveedor.Direccion;
        }


        private void TablaProveedores_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}