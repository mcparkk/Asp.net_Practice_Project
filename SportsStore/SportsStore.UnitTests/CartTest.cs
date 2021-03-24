using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Linq;
using System.Web.Mvc;

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

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"}
            });

            //Arrange - Cart개체를 생성한다.
            Cart cart = new Cart();

            //Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object);

            //Act - 카트에 상품을 추가한다.
            target.AddToCart(cart, 1, null);

            //Assert 
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //Arrange - Mock repository 생성
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"}
            });

            //Arrange - Cart개체를 생성한다.
            Cart cart = new Cart();

            //Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object);

            //Act - 카트에 상품을 추가한다.
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //Assert 
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //Arrange - Cart개체를 생성한다.
            Cart cart = new Cart();

            //Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(null);

            //Act -Index 액션메서드를 호출한다.
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //Assert 
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
