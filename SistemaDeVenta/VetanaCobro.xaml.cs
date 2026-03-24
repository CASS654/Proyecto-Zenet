using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SistemaDeVentaPrueba
{
    public partial class VetanaCobro : Window
    {
        private bool modoCantidad = false;
        private string bufferCantidad = "";
        private int cantidadActual = 1;

        public ObservableCollection<ProductoPOS> carrito { get; set; } = new ObservableCollection<ProductoPOS>();

        public VetanaCobro()
        {
            InitializeComponent();
            GridProductos.ItemsSource = carrito;
        }

        private void AbrirBuscador()
        {
            BusquedaProducto buscador = new BusquedaProducto();
            if (buscador.ShowDialog() == true)
            {
                if (buscador.ProductoSeleccionado != null)
                {
                    // Usamos la cantidad que el usuario escribió (ej: 4*)
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
            }
            else
            {
                producto.Quantity = qty;
                carrito.Add(producto);
            }
            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal total = carrito.Sum(p => p.Total);
            // txtTotalAmount.Text = total.ToString("C"); // Asegúrate de tener este control en XAML
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
    }

    public class ProductoPOS
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }
}