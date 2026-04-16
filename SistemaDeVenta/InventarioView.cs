using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SistemaDeVenta
{
    public class InventarioView
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        public bool Disponible { get; set; }

        public string EstadoStock
        {
            get
            {
                if (Stock == 0)
                    return "SIN STOCK";

                if (Stock <= 20)
                    return "BAJO";

                return "DISPONIBLE";
            }
        }


        public Brush ColorStock
        {
            get
            {
                if (Stock == 0)
                    return Brushes.Red;

                if (Stock <= 20)
                    return Brushes.Orange;

                return Brushes.Green;
            }
        }
    }
}
