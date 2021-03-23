using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System;
using System.Linq;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange 테스트할 상품들을 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange 새로운 카트를 생성한다.
            Cart target = new Cart();

            //Act
            target.AddItems(p1, 1);
            target.AddItems(p2, 1);
            Cartline[] results = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }


        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange - 테스트할 상품들을 생선하다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - 새로운 카트를 생성한다. 
            Cart target = new Cart();

            //Act
            target.AddItems(p1, 1);
            target.AddItems(p2, 1);
            target.AddItems(p1, 10);
            Cartline[] results = target.Lines.OrderBy(x => x.Product.ProductID).ToArray();

            //Assert 
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);

        }

        [TestMethod]
        public void Can_Remove_Lines()
        {
            //Arrange 테스트할 상품들을 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };
            Product p4 = new Product { ProductID = 4, Name = "P4" };
            Product p5 = new Product { ProductID = 5, Name = "P5" };

            //Arrange 새로운 카트를 생성하고 담는다.
            Cart target = new Cart();
            target.AddItems(p1, 1);
            target.AddItems(p2, 3);
            target.AddItems(p3, 5);
            target.AddItems(p2, 1);

            //Act 
            target.RemoveLine(p2);

            //Assert
            Assert.AreEqual(target.Lines.Where(x => x.Product.Name == "P2").Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //Arrange 테스트할 상품들을 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" ,Price = 100M};
            Product p2 = new Product { ProductID = 2, Name = "P2" ,Price = 50M};
            

            //Arrange 새로운 카트를 생성하고 담는다.
            Cart target = new Cart();
            target.AddItems(p1, 1);
            target.AddItems(p2, 1);
            target.AddItems(p1, 3);

            //Act 
            var result = target.ComputeTatolValue();

            //Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //Arrange 테스트할 상품들을 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //Arrange 새로운 카트를 생성하고 담는다.
            Cart target = new Cart();
            target.AddItems(p1, 1);
            target.AddItems(p2, 1);
            target.AddItems(p1, 3);

            //Act 
            target.Clear();

            //Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}
