using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SistemaDeVenta
{
    public partial class SelectorProveedor : Window
    {
        public int ProveedorSeleccionadoId { get; private set; }

        public SelectorProveedor(List<Proveedor> listaProveedores)
        {
            InitializeComponent();
            MostrarProveedores(listaProveedores);
        }

        private void MostrarProveedores(List<Proveedor> lista)
        {
            wpProveedores.Children.Clear();

            foreach (var prov in lista)
            {
                // Botón igual que productos pero solo con el nombre
                var btn = new Button
                {
                    Width = 140,
                    Height = 100,
                    Margin = new Thickness(8),
                    Tag = prov.IdProveedor,
                    Content = prov.Nombre,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                };

                btn.Click += Proveedor_Click;

                wpProveedores.Children.Add(btn);
            }
        }

        private void Proveedor_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ProveedorSeleccionadoId = (int)btn.Tag;

            this.DialogResult = true;
            this.Close();
        }
    }

    // Clase proveedor igual que la de Producto

}
