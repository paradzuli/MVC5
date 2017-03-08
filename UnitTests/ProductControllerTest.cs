using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Abstract;
using Moq;
using Domain.Entities;
using WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models;
using WebUI.HtmlHelpers;
using Microsoft.CSharp;

namespace UnitTests
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1" },
                new Product {ProductID=2, Name="P2" },
                new Product {ProductID=3, Name="P3" },
                new Product {ProductID=4, Name="P4" },
                new Product {ProductID=5, Name="P5" }
            });
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange- A html Helper in order to apply extension method
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
                         

        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1" },
                new Product {ProductID=2, Name="P2" },
                new Product {ProductID=3, Name="P3" },
                new Product {ProductID=4, Name="P4" },
                new Product {ProductID=5, Name="P5" }
            });
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

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
        public void Can_Filter_Products()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1",Category="Cat1" },
                new Product {ProductID=2, Name="P2" ,Category="Cat2"},
                new Product {ProductID=3, Name="P3" ,Category="Cat3"},
                new Product {ProductID=4, Name="P4" ,Category="Cat4"},
                new Product {ProductID=5, Name="P5" ,Category="Cat5"}
            });
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            Product[] result = ((ProductsListViewModel)controller.List("Cat1", 1).Model).Products.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 1);
            Assert.IsTrue(result[0].Name == "P1" && result[0].Category == "Cat1");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //--Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                  new Product {ProductID=1, Name="P1",Category="A" },
                new Product {ProductID=2, Name="P2" ,Category="C"},
                new Product {ProductID=3, Name="P3" ,Category="B"},
                new Product {ProductID=4, Name="P4" ,Category="A"},
                new Product {ProductID=5, Name="P5" ,Category="D"}
            });

            var navController = new NavController(mock.Object);

            //Act
            string[] results = ((IEnumerable<string>)navController.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 4);
            Assert.AreEqual(results[0], "A");
            Assert.AreEqual(results[1], "B");
            Assert.AreEqual(results[2], "C");
            Assert.AreEqual(results[3], "D");

        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1",Category="A" },
                new Product {ProductID=2, Name="P2" ,Category="C"},
                new Product {ProductID=3, Name="P3" ,Category="B"},
                new Product {ProductID=4, Name="P4" ,Category="A"},
                new Product {ProductID=5, Name="P5" ,Category="D"}
            });
            NavController target = new NavController(mock.Object);

            string categoryToSelect = "A";
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                 new Product {ProductID=1, Name="P1",Category="A" },
                new Product {ProductID=2, Name="P2" ,Category="C"},
                new Product {ProductID=3, Name="P3" ,Category="B"},
                new Product {ProductID=4, Name="P4" ,Category="A"},
                new Product {ProductID=5, Name="P5" ,Category="D"}
            });

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //Act
            int res1 = ((ProductsListViewModel)target.List("A").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("D").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 1);
            Assert.AreEqual(res3, 5);

        }
    }
}
