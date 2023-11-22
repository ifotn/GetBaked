using GetBaked.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetBakedUnitTesting
{
    [TestClass]
    public class CategoriesControllerUnitTests
    {
        [TestMethod]
        public void DetailsNullIdReturnsError()
        {
            // arrange
            var controller = new CategoriesController();

            // act => try /Categories/Details with no id parameter value
            var result = controller.Details(null);

            // assert

        }
    }
}
