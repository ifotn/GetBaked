using GetBaked.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GetBakedUnitTesting
{
    [TestClass]
    public class HomeControllerTests
    {
        // test names usually include the Method being tested, conditions & expected result
        [TestMethod]
        public void IndexRendersHomePage()
        {
            // arrange
            var controller = new HomeController();

            // act
            var result = (ViewResult)controller.Index();

            // assert
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void PrivacyRendersView()
        {
            // arrange
            var controller = new HomeController();

            // act
            var result = (ViewResult)controller.Privacy();

            // assert
            Assert.AreEqual("Privacy", result.ViewName);
        }
    }
}