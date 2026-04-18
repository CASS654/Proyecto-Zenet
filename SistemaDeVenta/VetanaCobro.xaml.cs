using Sistema_Bancario;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SistemaDeVenta
{
    public partial class VetanaCobrousuario : Window
    {
        private const decimal TasaImpuesto = 0.08m;

        private bool modoCantidad = false;
        private string bufferCantidad = "";
        private int cantidadActual = 1;

        public ObservableCollection<ProductoPOS> carrito { get; set; }
            = new ObservableCollection<ProductoPOS>();

        public VetanaCobrousuario()
        {
            InitializeComponent();
            GridProductos.ItemsSource = carrito;

            // Escuchar cuando se agrega o elimina un item
            carrito.CollectionChanged += Carrito_CollectionChanged;
        }

        private void Carrito_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ActualizarTotales();
        }

        public VetanaCobrousuario(string nombre)
        {
            InitializeComponent();

            // Mostrar nombre completo
            TxtNombreUsuario.Text = nombre;

            // Obtener inicial
            if (!string.IsNullOrEmpty(nombre))
            {
                TxtInicialUsuario.Text = nombre.Substring(0, 1).ToUpper();
            }
        }
        private void AbrirBuscador()
        {
            BusquedaProducto buscador = new BusquedaProducto();
            if (buscador.ShowDialog() == true)
            {
                if (buscador.ProductoSeleccionado != null)
                {
                    AgregarAlCarrito(buscador.ProductoSeleccionado, cantidadActual);
                }
            }
            ResetCantidad();
        }

        private void AgregarAlCarrito(ProductoPOS producto, int qty)
        {
            var existente = carrito.FirstOrDefault(p => p.Id == producto.Id);
            if (existente != null)
            {
                existente.Quantity += qty;
                GridProductos.Items.Refresh();
                ActualizarTotales(); // cantidad de existente no dispara CollectionChanged
            }
            else
            {
                producto.Quantity = qty;
                carrito.Add(producto); // esto sí dispara CollectionChanged
            }
        }

        private void ActualizarTotales()
        {
            decimal subtotal = carrito.Sum(p => p.Total);
            decimal impuesto = subtotal * TasaImpuesto;
            decimal total = subtotal + impuesto;

            TxtSubtotal.Text = subtotal.ToString("C");
            TxtTax.Text = impuesto.ToString("C");

            string totalStr = total.ToString("F2");
            string[] partes = totalStr.Split('.');
            TxtTotalEntero.Text = "$" + partes[0];
            TxtTotalDecimal.Text = "." + (partes.Length > 1 ? partes[1] : "00");

            int totalItems = carrito.Sum(p => p.Quantity);
            TxtTotalItems.Text = $"TOTAL ITEMS: {totalItems}";
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                modoCantidad = true;
                bufferCantidad = "";
                cantidadActual = 1;
                if (PanelQty != null) PanelQty.Visibility = Visibility.Visible;
                if (TxtQty != null) TxtQty.Text = "1";
                e.Handled = true;
                return;
            }

            if (e.Key == Key.F2 || e.Key == Key.Enter)
            {
                if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                    int.TryParse(bufferCantidad, out cantidadActual);

                AbrirBuscador();
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
                ResetCantidad();
        }

        private void TxtBusqueda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (modoCantidad)
            {
                Regex regex = new Regex("[^0-9]+");
                if (!regex.IsMatch(e.Text))
                {
                    bufferCantidad += e.Text;
                    if (int.TryParse(bufferCantidad, out int resultado))
                    {
                        cantidadActual = resultado;
                        if (TxtQty != null) TxtQty.Text = cantidadActual.ToString();
                    }
                }
                e.Handled = true;
            }
        }

        private void ResetCantidad()
        {
            modoCantidad = false;
            bufferCantidad = "";
            cantidadActual = 1;
            if (PanelQty != null) PanelQty.Visibility = Visibility.Collapsed;
            if (TxtQty != null) TxtQty.Text = "1";
            TxtBusqueda.Clear();
            TxtBusqueda.Focus();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                int.TryParse(bufferCantidad, out cantidadActual);

            AbrirBuscador();
            e.Handled = true;
        }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (carrito.Count == 0)

            {

                MessageBox.Show("El carrito está vacío.", "Aviso",

                                MessageBoxButton.OK, MessageBoxImage.Warning);

                return;

            }

            var confirmacion = MessageBox.Show(

                $"¿Confirmar venta por {TxtTotalEntero.Text}{TxtTotalDecimal.Text}?",

                "Confirmar Pago", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            // Sin ConnectionString, usa la conexión estática directamente

            var service = new VentaService();

            int idVenta = service.GuardarVenta(carrito, globales.IdUsuarioGlobal, "EFECTIVO");

            if (idVenta > 0)

            {

                MessageBox.Show($"✅ Venta #{idVenta} registrada.", "Éxito",

                                MessageBoxButton.OK, MessageBoxImage.Information);

                carrito.Clear();

                ActualizarTotales();

            }
        }

        private void CerrarSesion()
        {
            // Abrir login
            LoginPage login = new LoginPage();
            login.Show();

            // Cerrar ventana actual
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CerrarSesion();
            }
        }

        private void Border_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            CerrarSesion();
        }
    }
}