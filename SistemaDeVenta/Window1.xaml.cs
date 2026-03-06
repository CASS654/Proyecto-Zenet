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
using System.Windows.Shapes;


namespace SistemaDeVenta
{

    public partial class Window1 : Window
    {

        public int ProductoSeleccionadoId { get; private set; }
        public Window1(List<Producto> listaProductos)
        {
            InitializeComponent();
            MostrarProductos(listaProductos);
        }
        private void MostrarProductos(List<Producto> lista)
        {
            wpProductos.Children.Clear();

            foreach (var p in lista)
            {
                // Botón simple con nombre y precio
                var btn = new Button
                {
                    Width = 120,
                    Height = 120,
                    Margin = new Thickness(5),
                    Tag = p.IdProducto,
                    Content = $"{p.Nombre}\n${p.PrecioVenta:0.00}",
                    FontWeight = FontWeights.Bold,
                };

                btn.Click += Producto_Click;

                wpProductos.Children.Add(btn);
            }
        }

        private void Producto_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ProductoSeleccionadoId = (int)btn.Tag;

            this.DialogResult = true; // cierra la ventana modal
            this.Close();
        }
    }

    // Clase Producto pública para que no haya errores de accesibilidad
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}

