using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},
                new Product{ProductID = 2, Name = "P2"},
                new Product{ProductID = 3, Name = "P3"},
                new Product{ProductID = 4, Name = "P4"},
                new Product{ProductID = 5, Name = "P5"},
                
            });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            //Assert
            Product[] productArray = result.Products.ToArray();
            Assert.IsTrue(productArray.Length == 2);
            Assert.AreEqual(productArray[0].Name, "P4");
            Assert.AreEqual(productArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange - 확장 메서드를 적용하기 위한 HTML헬퍼를 정의한다.
            HtmlHelper myHelper = null;

            //Arrange - PagingInfo 데이터를 생성한다.
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //Arrange - 람다 표현식을 사용해서 델리게이트를 설정한다.
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},
                new Product{ProductID = 2, Name = "P2"},
                new Product{ProductID = 3, Name = "P3"},
                new Product{ProductID = 4, Name = "P4"},
                new Product{ProductID = 5, Name = "P5"},
            });

            //Arrage
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            //Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Product()
        {
            //Arrange - Mock 레파지토리 생성 
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "P1", Category="Cat1"},
                new Product{ProductID = 2, Name = "P2", Category="Cat2"},
                new Product{ProductID = 3, Name = "P3", Category="Cat1"},
                new Product{ProductID = 4, Name = "P4", Category="Cat2"},
                new Product{ProductID = 5, Name = "P5", Category="Cat3"}
            });

            //Arrange - 컨트롤러 생성 -> 페이지 크기를 세 개의 항목으로 설정한다. 
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Action 
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[0].Category == "Cat2");
        }
    }
}










