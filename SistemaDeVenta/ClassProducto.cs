using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta
{
    internal class ClassProducto
    {
        public class Producto
        {
            public int IdProducto { get; set; }
            public string Nombre { get; set; }
            public decimal PrecioVenta { get; set; }
            public decimal PrecioCompra { get; set; }
            public string Imagen { get; set; } // ruta de imagen
        }

    }
}
