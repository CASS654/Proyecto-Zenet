using MySql.Data.MySqlClient;
using Sistema_Bancario;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SistemaDeVenta
{
    /// <summary>
    /// Lógica de interacción para VentanaInventario.xaml
    /// </summary>
    public partial class VentanaInventario : UserControl
    {
        private List<InventarioView> listaOriginal = new List<InventarioView>();
        public VentanaInventario()
        {
            InitializeComponent();
            CargarInventario();
            CargarResumen();
            PlaceholderBuscar.Visibility = Visibility.Visible;

        }
        private void BtnVolverInventario_Click(object sender, RoutedEventArgs e)
        {
            PanelReportes.Visibility = Visibility.Collapsed;
            PanelInventario.Visibility = Visibility.Visible;
        }

        private void CargarInventario()
        {
            InventarioDAO dao = new InventarioDAO();
            listaOriginal = dao.ObtenerInventario();
            ItemsProductos.ItemsSource = null;
            ItemsProductos.ItemsSource = listaOriginal;
        }

        private void BtnAcciones_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn == null)
            {
                MessageBox.Show("Botón es null");
                return;
            }

            var producto = btn.Tag as InventarioView;

            if (producto == null)
            {
                MessageBox.Show("Producto es NULL");
                return;
            }

            // 🔥 CREAR MENÚ
            ContextMenu menu = new ContextMenu();

            // OPCIÓN EDITAR
            MenuItem editar = new MenuItem { Header = "Editar" };
            editar.Click += (s, ev) =>
            {
                VentanaEditarProducto ventana = new VentanaEditarProducto(producto);
                ventana.ShowDialog();
                CargarInventario();
            };

            // OPCIÓN MERMA
            MenuItem merma = new MenuItem { Header = "Registrar Merma" };
            merma.Click += (s, ev) =>
            {
                VentanaMerma ventana = new VentanaMerma(producto);
                ventana.ShowDialog();
                CargarInventario();
            };
            // OPCIÓN DISPONIBILIDAD
            MenuItem disponibilidad = new MenuItem { Header = "Cambiar Disponibilidad" };
            disponibilidad.Click += (s, ev) =>
            {
                if (producto == null)
                {
                    MessageBox.Show("Producto no válido");
                    return;
                }

                bool nuevoEstado = !producto.Disponible;

                string mensaje = nuevoEstado
                    ? "¿Deseas ACTIVAR este producto?"
                    : "¿Deseas DESACTIVAR este producto?";

                var confirm = MessageBox.Show(
                    mensaje,
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                    return;

                try
                {
                    var conn = ClassConexion.SQLConnection;

                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    string query = @"UPDATE Productos 
                         SET Disponible = @Disponible
                         WHERE IdProducto = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Disponible", nuevoEstado);
                    cmd.Parameters.AddWithValue("@Id", producto.IdProducto);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Disponibilidad actualizada correctamente");

                    CargarInventario(); // 🔥 refresca la tabla
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            };

            // AGREGAR AL MENÚ
            menu.Items.Add(editar);
            menu.Items.Add(merma);
            menu.Items.Add(disponibilidad);

            // MOSTRAR MENÚ
            btn.ContextMenu = menu;
            menu.IsOpen = true;
        }

        private void CargarResumen()
        {
            InventarioDAO dao = new InventarioDAO();
            var data = dao.ObtenerResumen();

            TxtTotalProductos.Text = data.total.ToString();
            TxtStockBajo.Text = data.bajos.ToString();
            TxtAgotados.Text = data.agotados.ToString();
        }
        private void TxtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderBuscar.Visibility = Visibility.Collapsed;
        }

        private void TxtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBuscar.Text))
            {
                PlaceholderBuscar.Visibility = Visibility.Visible;
            }
        }
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = (sender as TextBox).Text.ToLower();

            InventarioDAO dao = new InventarioDAO();
            var lista = dao.ObtenerInventario();

            var filtrada = lista.Where(p =>
                p.Nombre.ToLower().Contains(texto) ||
                p.Categoria.ToLower().Contains(texto) ||
                p.IdProducto.ToString().Contains(texto)
            ).ToList();

            ItemsProductos.ItemsSource = null;
            ItemsProductos.ItemsSource = filtrada;
        }

        private void BtnFiltros_Click(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            // TODOS
            MenuItem todos = new MenuItem { Header = "Todos"  };


            todos.Click += (s, ev) =>
            {
                ItemsProductos.ItemsSource = listaOriginal;
            };

            // DISPONIBLES
            MenuItem disponibles = new MenuItem { Header = "Disponibles" };
            disponibles.Click += (s, ev) =>
            {
                var filtrado = listaOriginal.Where(p => p.Disponible).ToList();
                ItemsProductos.ItemsSource = filtrado;
            };

            // NO DISPONIBLES
            MenuItem noDisponibles = new MenuItem { Header = "No disponibles" };
            noDisponibles.Click += (s, ev) =>
            {
                var filtrado = listaOriginal.Where(p => !p.Disponible).ToList();
                ItemsProductos.ItemsSource = filtrado;
            };

            // STOCK BAJO (<20)
            MenuItem bajo = new MenuItem { Header = "Stock bajo" };
            bajo.Click += (s, ev) =>
            {
                var filtrado = listaOriginal.Where(p => p.Stock > 0 && p.Stock <= 20).ToList();
                ItemsProductos.ItemsSource = filtrado;
            };  

            // AGOTADOS
            MenuItem agotados = new MenuItem { Header = "Agotados" };
            agotados.Click += (s, ev) =>
            {
                var filtrado = listaOriginal.Where(p => p.Stock == 0).ToList();
                ItemsProductos.ItemsSource = filtrado;
            };

            // AGREGAR AL MENÚ
            menu.Items.Add(todos);
            menu.Items.Add(disponibles);
            menu.Items.Add(noDisponibles);
            menu.Items.Add(bajo);
            menu.Items.Add(agotados);

            // MOSTRAR
            var border = sender as Border;
            border.ContextMenu = menu;
            menu.IsOpen = true;
        }
        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = new ContextMenu
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1C2132")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3348")),
                BorderThickness = new Thickness(1)
            };

            // 🔹 HISTORIAL DE VENTAS
            MenuItem ventas = new MenuItem
            {
                Header = "Historial de Ventas",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                Padding = new Thickness(10)
            };

            ventas.Click += (s, ev) =>
            {
                MostrarReportes("Ventas");
            };

            // 🔹 HISTORIAL DE COMPRAS
            MenuItem compras = new MenuItem
            {
                Header = "Historial de Compras",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                Padding = new Thickness(10)
            };

            compras.Click += (s, ev) =>
            {
                MostrarReportes("Compras");
            };

            // 🔹 HISTORIAL DE CAMBIOS / MERMA
            MenuItem cambios = new MenuItem
            {
                Header = "Historial de Cambios",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                Padding = new Thickness(10)
            };

            cambios.Click += (s, ev) =>
            {
                MostrarReportes("Cambios");
            };

            // 🔹 HISTORIAL DE CAMBIOS / MERMA
            MenuItem merma = new MenuItem
            {
                Header = "Merma",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                Padding = new Thickness(10)
            };

            merma.Click += (s, ev) =>
            {
                MostrarReportes("Merma");
            };

            // 🔥 EFECTO HOVER (igual al estilo anterior pero sin Resource)
            void AplicarHover(MenuItem item)
            {
                item.MouseEnter += (s, ev) =>
                {
                    item.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3348"));
                };

                item.MouseLeave += (s, ev) =>
                {
                    item.Background = Brushes.Transparent;
                };
            }

            AplicarHover(ventas);
            AplicarHover(compras);
            AplicarHover(cambios);
            AplicarHover(merma);

            // AGREGAR ITEMS
            menu.Items.Add(ventas);
            menu.Items.Add(compras);
            menu.Items.Add(cambios);
            menu.Items.Add(merma);

            // MOSTRAR
            var btn = sender as Button;
            btn.ContextMenu = menu;
            menu.IsOpen = true;
        }
        private void MostrarReportes(string tipo)
        {
            PanelInventario.Visibility = Visibility.Collapsed;
            PanelReportes.Visibility = Visibility.Visible;

            TituloReporte.Text = "Historial de " + tipo;

            ReportesDAO dao = new ReportesDAO();

            if (tipo == "Ventas")
                TablaReportes.ItemsSource = dao.ObtenerVentas();

            else if (tipo == "Compras")
                TablaReportes.ItemsSource = dao.ObtenerCompras();

            else if (tipo == "Cambios")
                TablaReportes.ItemsSource = dao.ObtenerCambios();

            else if (tipo == "Merma")
                TablaReportes.ItemsSource = dao.ObtenerMerma();
        }


    }
}
