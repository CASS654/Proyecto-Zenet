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
        private bool bloqueandoESC = false;
        private bool modoCantidad = false;
        private string bufferCantidad = "";
        private decimal cantidadActual = 1m ;

        // 🔥 CONTROL DEL PLACEHOLDER
        private bool mostrarPlaceholder = true;

        private ProductoPOS productoPendiente = null;
        public ObservableCollection<ProductoPOS> carrito { get; set; }
            = new ObservableCollection<ProductoPOS>();

        public VetanaCobrousuario()
        {
            InitializeComponent();
            GridProductos.ItemsSource = carrito;

            carrito.CollectionChanged += Carrito_CollectionChanged;
            TxtBusqueda.Focus();
        }

        private void Carrito_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ActualizarTotales();
        }

        // 🔥 CONTROL VISUAL DEL PLACEHOLDER
        private void ActualizarPlaceholder()
        {
            if (!mostrarPlaceholder)
            {
                PlaceholderBusqueda.Visibility = Visibility.Collapsed;
                return;
            }

            PlaceholderBusqueda.Visibility =
                string.IsNullOrEmpty(TxtBusqueda.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void TxtBusqueda_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ActualizarPlaceholder();
        }

        private void AbrirBuscador()
        {
            bloqueandoESC = true; // 🔥 BLOQUEAR ESC

            mostrarPlaceholder = false;
            ActualizarPlaceholder();

            TxtBusqueda.Text = "";

            BusquedaProducto buscador = new BusquedaProducto();

            try
            {
                if (buscador.ShowDialog() == true)
                {
                    if (buscador.ProductoSeleccionado != null)
                    {
                        // 🔥 GUARDAR PERO NO AGREGAR
                        productoPendiente = buscador.ProductoSeleccionado;

                        // 🔥 MOSTRAR EN LA BARRA
                        if (cantidadActual <= 1)
                            TxtBusqueda.Text = productoPendiente.Name;
                        else
                            TxtBusqueda.Text = $" * {productoPendiente.Name}";

                        mostrarPlaceholder = false;
                        ActualizarPlaceholder();
                    }
                }
            }
            finally
            {
                bloqueandoESC = false; // 🔥 DESBLOQUEAR SIEMPRE
            }
        }

        private void AgregarAlCarrito(ProductoPOS producto, decimal qty)
        {
            var existente = carrito.FirstOrDefault(p => p.Id == producto.Id);
            if (existente != null)
            {
                existente.Quantity += qty;
                GridProductos.Items.Refresh();
                ActualizarTotales();
            }
            else
            {
                producto.Quantity = qty;
                carrito.Add(producto);
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

            decimal totalItems = carrito.Sum(p => p.Quantity);
            TxtTotalItems.Text = $"TOTAL ITEMS: {totalItems:0.###}";
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                modoCantidad = true;
                bufferCantidad = "";
                cantidadActual = 1;

                PanelQty.Visibility = Visibility.Visible;
                TxtQty.Text = "1";

                TxtBusqueda.Clear();              // 🔥 LIMPIAR TEXTO
                productoPendiente = null;         // 🔥 LIMPIAR PRODUCTO
                mostrarPlaceholder = false;       // 🔥 ocultar placeholder
                ActualizarPlaceholder();

                TxtBusqueda.Focus();              // 🔥 asegurar input

                e.Handled = true;
                return;
            }

            // 🔥 ENTER AHORA AGREGA
            if (e.Key == Key.Enter)
            {
                if (productoPendiente != null)
                {

                    // DESPUÉS
                    if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                        decimal.TryParse(bufferCantidad,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out cantidadActual);

                    AgregarAlCarrito(productoPendiente, cantidadActual);

                    productoPendiente = null;

                    ResetCantidad();
                    TxtBusqueda.Clear();
                    ActualizarPlaceholder();
                }

                e.Handled = true;
                return;
            }

            // 🔥 F2 SOLO ABRE BUSCADOR
            if (e.Key == Key.F2)
            {
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
                // Acepta dígitos, punto y coma (coma se convierte a punto)
                string entrada = e.Text == "," ? "." : e.Text;
                Regex regex = new Regex("[^0-9.]+");

                // Evitar doble punto
                if (!regex.IsMatch(entrada) && !(entrada == "." && bufferCantidad.Contains(".")))
                {
                    bufferCantidad += entrada;

                    if (decimal.TryParse(bufferCantidad,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimal resultado))
                    {
                        cantidadActual = resultado;
                        TxtQty.Text = cantidadActual.ToString("0.###");
                    }
                }

                e.Handled = true;
            }
        }

        private void ResetCantidad()
        {
            modoCantidad = false;
            bufferCantidad = "";
            cantidadActual = 1m;

            PanelQty.Visibility = Visibility.Collapsed;
            TxtQty.Text = "1";

            TxtBusqueda.Clear();

            mostrarPlaceholder = true; // 🔥 reactivar placeholder
            ActualizarPlaceholder();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                decimal.TryParse(bufferCantidad,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out cantidadActual);
            mostrarPlaceholder = false; // 🔥 ocultar placeholder
            ActualizarPlaceholder();

            TxtBusqueda.Text = "";
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
            LoginPage login = new LoginPage();
            login.Show();
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (bloqueandoESC) return; // 🔥 CLAVE

            if (e.Key == Key.Escape)
            {
                CerrarSesion();
            }
        }

        public void CargarUsuario(string nombre)
        {
            TxtNombreUsuario.Text = nombre;

            if (!string.IsNullOrEmpty(nombre))
            {
                TxtInicial.Text = nombre.Substring(0, 1).ToUpper();
            }
        }

        private void Border_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            CerrarSesion();
        }
    }
}