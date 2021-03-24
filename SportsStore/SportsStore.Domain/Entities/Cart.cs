using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        private List<Cartline> lineCollection = new List<Cartline>();

        public void AddItems(Product product, int quantity)
        {
            Cartline line = lineCollection.Where(x => x.Product.ProductID == product.ProductID).FirstOrDefault();

            if (line == null)
                lineCollection.Add(new Cartline { Product = product, Quantity = quantity });
            else
                line.Quantity += quantity;
        }

        public void RemoveLine(Product product)
        {
            lineCollection.RemoveAll(x => x.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTatolValue()
        {
            return lineCollection.Sum(x => x.Product.Price * x.Quantity);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<Cartline> Lines
        {
            get { return lineCollection; }
        }
    }

    public class Cartline
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
