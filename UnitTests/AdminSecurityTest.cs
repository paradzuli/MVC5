using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebUI.Infrastructure.Abstract;
using WebUI.Models;
using WebUI.Controllers;
using System.Web.Mvc;

namespace UnitTests
{
    [TestClass]
    public class AdminSecurityTest
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);
            LoginViewModel model = new LoginViewModel { UserName = "admin", Password = "secret" };
            AccountController controller = new AccountController(mock.Object);
            ActionResult result = controller.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("incorrectUser", "incorrectpwd")).Returns(false);
            LoginViewModel model = new LoginViewModel { UserName = "incorrectuser", Password = "incorretpwd" };
            AccountController controller = new AccountController(mock.Object);
            ActionResult result = controller.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);

        }
    }
}
