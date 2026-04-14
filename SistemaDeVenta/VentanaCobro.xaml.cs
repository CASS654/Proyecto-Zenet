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
        private const decimal TasaImpuesto = 0.08m; // 8% — cámbialo aquí si necesitas otro %

        private bool modoCantidad = false;
        private string bufferCantidad = "";
        private int cantidadActual = 1;

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
                ActualizarTotales(); // ← cuando cambia cantidad de existente (no dispara CollectionChanged)
            }
            else
            {
                producto.Quantity = qty;
                carrito.Add(producto); // ← esto sí dispara CollectionChanged
            }
        }

        private void ActualizarTotales()
        {
            decimal subtotal = carrito.Sum(p => p.Total);
            decimal impuesto = subtotal * TasaImpuesto;
            decimal total = subtotal + impuesto;

            // Subtotal y Tax
            TxtSubtotal.Text = subtotal.ToString("C");
            TxtTax.Text = impuesto.ToString("C");

            // Total separado en entero y decimales para el efecto visual grande
            string totalStr = total.ToString("F2");          // ej: "456.82"
            string[] partes = totalStr.Split('.');
            TxtTotalEntero.Text = "$" + partes[0];               // "$456"
            TxtTotalDecimal.Text = "." + (partes.Length > 1 ? partes[1] : "00"); // ".82"

            // Contador de items en el footer
            int totalItems = carrito.Sum(p => p.Quantity);
            TxtTotalItems.Text = $"TOTAL ITEMS: {totalItems}";
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            // 1. Activar modo cantidad con la tecla X
            if (e.Key == Key.X)
            {
                modoCantidad = true;
                bufferCantidad = "";
                cantidadActual = 1;

                if (PanelQty != null) PanelQty.Visibility = Visibility.Visible;
                if (TxtQty != null) TxtQty.Text = "1";

                e.Handled = true; // Evita que la 'x' se escriba en el buscador
                return;
            }

            // 2. Abrir buscador con F2 o ENTER
            if (e.Key == Key.F2 || e.Key == Key.Enter)
            {
                // Si el modo cantidad está activo y hay un número en el buffer, lo aseguramos
                if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
                {
                    int.TryParse(bufferCantidad, out cantidadActual);
                }

                AbrirBuscador();
                e.Handled = true;
            }

            // 3. Cancelar modo cantidad con ESC
            if (e.Key == Key.Escape)
            {
                ResetCantidad();
            }
        }

        private void TxtBusqueda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (modoCantidad)
            {
                // Solo permitir números (Regex de 0 a 9)
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

                e.Handled = true; // Bloquea la escritura en el TextBox principal mientras pones la cantidad
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
            // Si el modo cantidad está activo y hay un número en el buffer, lo aseguramos
            if (modoCantidad && !string.IsNullOrEmpty(bufferCantidad))
            {
                int.TryParse(bufferCantidad, out cantidadActual);
            }

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
    }
}