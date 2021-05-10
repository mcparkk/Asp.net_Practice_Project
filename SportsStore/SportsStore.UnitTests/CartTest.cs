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
            CartController target = new CartController(mock.Object , null);

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
            CartController target = new CartController(mock.Object , null);

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
            CartController target = new CartController(null, null);

            //Act -Index 액션메서드를 호출한다.
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //Assert 
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        // 카트가 빈상태로는 지불 처리가 되지 않는다는 점을 테스트 
        // Mock 구현 개체에서 ProcessorOrder 메서드가 호출된 적 없다는것 확인
        // CheckOut메서드가 반환하는 뷰가 기본 뷰인지 확인 
        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - Mock 주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            //Arrange - 빈 Cart 개체를 생성한다.
            Cart cart = new Cart();
            //Arrange - 배송정보를 생성한다.
            ShippingDetails shippingDetails = new ShippingDetails();
            //Arrange - 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);

            //Act 
            // cart와 shippingDetails는 비어있다!!
            ViewResult result = target.Checkout(cart, shippingDetails);

            //Assert - 주문이 주문 처리기에 전달되지 않은 것을 확인한다. 
            // IOrderProcessor 의 Mock구현 개체에서 Processor 메서드가 호출된 적이 없다는 것 확인/ 앞의 메서드 -> time.never() = 실행된 적 없다는 것을 검증
            // https://jacking75.github.io/csharp_UnitTestMock/
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            //Assert - 메서드가 기본 뷰를 반환했는지 확인한다.
            // CheckOut메서드가 반환하는 뷰가 기본뷰인지 확인
            Assert.AreEqual("", result.ViewName);

            //Assert - 유효하지 않은 모델을 뷰에 전달했는지 확인한다.
            // 뷰에 전달된 모델 상태가 유효하지 않은 지 확인
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        // 유효하지 않은 배송정보로 요청시 확인 
        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - Mock주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrage - 하나의 상품이 담긴 Cart개체를 생성한다.
            Cart cart = new Cart();
            cart.AddItems(new Product(), 1);

            // Arrange - 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);
            // Arrange - 모델에 오류를 추가한다.
            target.ModelState.AddModelError("error", "error");

            // Act - 지불 처리를 시도한다.
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - 주문이 주문처리기에 전달되지 않은 것을 확인한다.
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - 메서드가 기본 뷰를 반환했는지 확인한다.
            Assert.AreEqual("", result.ViewName);
            // Assert - 유효하지 않은 모델을 뷰에 전달하는지 확인한다.
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        
        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - Mock 주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - 하나의 상품이 담긴 Cart 개체를 생성한다.
            Cart cart = new Cart();
            cart.AddItems(new Product(), 1);

            // Arrange - 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);
            
            // Act - 지불 처리를 시도한다.
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - 주문 처리기에 주문이 전달된것을 확인한다.
            mock.Verify(x => x.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);
            // Assert - 메서드가 Completed 뷰를 반환하는지 확인한다.
            Assert.AreEqual("Completed", result.ViewName);
            // Assert - 유효한 모델을 뷰에 전달하는지 확인한다.
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
