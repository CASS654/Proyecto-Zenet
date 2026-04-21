    using Sistema_Bancario;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    namespace SistemaDeVenta
    {
        public partial class VentanaCobro : UserControl
        {
            private const decimal TasaImpuesto = 0.08m;

            private bool bloqueandoESC = false;
            private bool modoCantidad = false;
            private string bufferCantidad = "";
            private decimal cantidadActual = 1m;

        // Control del placeholder (igual que VetanaCobrousuario)
        private bool mostrarPlaceholder = true;

            // Producto pendiente — se agrega con ENTER, no al abrir el buscador
            private ProductoPOS productoPendiente = null;

            public ObservableCollection<ProductoPOS> carrito { get; set; }
                = new ObservableCollection<ProductoPOS>();

            public VentanaCobro()
            {
                InitializeComponent();
                GridProductos.ItemsSource = carrito;
                carrito.CollectionChanged += Carrito_CollectionChanged;
            }

            private void Carrito_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                ActualizarTotales();
            }

            // ── Placeholder ────────────────────────────────────────────────
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

            private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
            {
                ActualizarPlaceholder();
            }

            // ── Buscador ───────────────────────────────────────────────────
            private void AbrirBuscador()
            {
                bloqueandoESC = true;

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
                            // Guardar producto pero NO agregar aún — esperar ENTER
                            productoPendiente = buscador.ProductoSeleccionado;

                            TxtBusqueda.Text = cantidadActual <= 1
                                ? productoPendiente.Name
                                : $" * {productoPendiente.Name}";

                            mostrarPlaceholder = false;
                            ActualizarPlaceholder();
                        }
                    }
                }
                finally
                {
                    bloqueandoESC = false;
                }
            }

            // ── Carrito ────────────────────────────────────────────────────
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

            // ── Teclado ────────────────────────────────────────────────────
            private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
            {
                // X — activar modo cantidad
                if (e.Key == Key.X)
                {
                    modoCantidad = true;
                    bufferCantidad = "";
                    cantidadActual = 1;

                    PanelQty.Visibility = Visibility.Visible;
                    TxtQty.Text = "1";

                    TxtBusqueda.Clear();
                    productoPendiente = null;
                    mostrarPlaceholder = false;
                    ActualizarPlaceholder();

                    TxtBusqueda.Focus();
                    e.Handled = true;
                    return;
                }

                // ENTER — agrega el producto pendiente al carrito
                if (e.Key == Key.Enter)
                {
                    if (productoPendiente != null)
                    {
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

                // F2 — solo abre el buscador
                if (e.Key == Key.F2)
                {
                    AbrirBuscador();
                    e.Handled = true;
                }

                // ESC — cancelar modo cantidad
                if (e.Key == Key.Escape)
                    ResetCantidad();
            }

        private void TxtBusqueda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (modoCantidad)
            {
                string entrada = e.Text == "," ? "." : e.Text;
                Regex regex = new Regex("[^0-9.]+");

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
                mostrarPlaceholder = true;
                ActualizarPlaceholder();
            }

            // ── Botones ────────────────────────────────────────────────────
            private void Border_MouseDown(object sender, MouseButtonEventArgs e)
            {
            if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                decimal.TryParse(bufferCantidad,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out cantidadActual);

            mostrarPlaceholder = false;
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
        }
    }