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
using static SistemaDeVenta.ClassProveedores;

namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para Proveedores.xaml
    /// </summary>
    public partial class Proveedores : UserControl
    {
        private string FiltroActual = "Nombre"; // Valor por defecto

        private List<Proveedores1> listaProveedores = new List<Proveedores1>();

        public Proveedores()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private void btnFiltro_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)FindResource("MenuFiltroTemplate");

            menu.PlacementTarget = btnFiltro;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; // 👈 Esto lo hace abrirse hacia abajo
            menu.IsOpen = true;
        }

        private void FiltroMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
                FiltroActual = item.Header.ToString();

        }
        private void CargarProveedores()
        {
            try
            {
                // Crear objeto que hace la consulta
                ClassTest obj = new ClassTest();
                DataTable registros = obj.ListarRegistros("SELECT * FROM Proveedores"); // Tu tabla MySQL

                listaProveedores.Clear(); // Limpia la lista antes de llenar
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

                // Limpiar el DataGrid antes de asignar ItemsSource
                TablaProveedores.ItemsSource = null;
                TablaProveedores.Items.Clear();
                TablaProveedores.ItemsSource = listaProveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string textoBusqueda = txtBuscar.Text.Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    // Si no hay texto, cargamos todos los proveedores
                    CargarProveedores();
                    return;
                }

                // Creamos la consulta SQL dinámica según el filtro seleccionado
                string columna = "Nombre"; // valor por defecto

                switch (FiltroActual)
                {
                    case "Nombre":
                        columna = "Nombre";
                        break;
                    case "Teléfono":
                        columna = "Telefono";
                        break;
                    case "Dirección":
                        columna = "Direccion";
                        break;
                }

                string consulta = $"SELECT * FROM Proveedores WHERE {columna} LIKE @buscar";

                ClassTest obj = new ClassTest();

                // Aquí asumimos que ClassTest tiene un método que recibe parámetros
                DataTable registros = obj.ListarRegistrosConParametro(consulta, "@buscar", "%" + textoBusqueda + "%");

                // Limpiamos la lista antes de llenar
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

                // Refrescar DataGrid
                TablaProveedores.ItemsSource = null;
                TablaProveedores.Items.Clear();
                TablaProveedores.ItemsSource = listaProveedores;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedores: " + ex.Message);
            }
        }

        private void TablaProveedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablaProveedores.SelectedItem == null)
                return;

            // Obtenemos el proveedor seleccionado
            Proveedores1 proveedor = (Proveedores1)TablaProveedores.SelectedItem;

            // Llenamos los TextBox con los valores del proveedor
            txtNombre.Text = proveedor.Nombre;
            txtTelefono.Text = proveedor.Telefono;
            txtDireccion.Text = proveedor.Direccion;
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            Button_Click(sender, e);    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var p = new Proveedores1
            {
                Nombre = txtNombre.Text,
                Telefono = txtTelefono.Text,
                Direccion = txtDireccion.Text
            };

            ClassProveedores db = new ClassProveedores();
            int resp = db.InsertarProveedor(p);

            if (resp == 0)
            {
                MessageBox.Show("Proveedor agregado correctamente.");
                CargarProveedores();
            }
            else
            {
                MessageBox.Show("Error al agregar proveedor.");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (TablaProveedores.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            Proveedores1 proveedor = (Proveedores1)TablaProveedores.SelectedItem;

            proveedor.Nombre = txtNombre.Text;
            proveedor.Telefono = txtTelefono.Text;
            proveedor.Direccion = txtDireccion.Text;

            ClassProveedores db = new ClassProveedores();
            int resp = db.EditarProveedor(proveedor);

            if (resp == 0)
            {
                MessageBox.Show("Proveedor modificado correctamente.");
                CargarProveedores();
            }
            else
            {
                MessageBox.Show("Error al modificar proveedor.");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (TablaProveedores.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            Proveedores1 proveedor = (Proveedores1)TablaProveedores.SelectedItem;

            if (MessageBox.Show("¿Eliminar proveedor seleccionado?",
                                "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ClassProveedores db = new ClassProveedores();
            int resp = db.EliminarProveedor(proveedor.IdProveedor);

            if (resp == 0)
            {
                MessageBox.Show("Proveedor eliminado correctamente.");
                CargarProveedores();
            }
            else
            {
                MessageBox.Show("Error al eliminar proveedor.");
            }
        }
    }

}

