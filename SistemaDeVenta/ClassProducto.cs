using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta
{
        public class ProductoPOS
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public decimal Total => UnitPrice * Quantity;
        }

}
