using GetBaked.Controllers;
using GetBaked.Data;
using GetBaked.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // set up mock db
        private ApplicationDbContext _context;

        // controller object for use in all tests
        CategoriesController controller;

        // shared list of categories for checking our data
        List<Category> categories = new List<Category>();

        // this method runs automatically before every test to avoid duplication
        [TestInitialize]
        public void TestInitialize()
        {
            // sets up new in memory db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // add some data to the mock db
            var category = new Category { CategoryId = 234, Name = "Some Category" };
            _context.Categories.Add(category);
            categories.Add(category);

            category = new Category { CategoryId = 567, Name = "Another Category" };
            _context.Categories.Add(category);
            categories.Add(category);

            category = new Category { CategoryId = 980, Name = "One More Category" };
            _context.Categories.Add(category);
            categories.Add(category);

            _context.SaveChanges();

            // create the controller using the mock db
            controller = new CategoriesController(_context);
        }
        
        [TestMethod]
        public void DetailsNullIdReturnsError()
        {
            // arrange

            // act => try /Categories/Details with no id parameter value
            var result = (ViewResult)controller.Details(null).Result;

            // assert
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public void DetailsInvalidIdReturnsError()
        {
            // arrange

            // act
            var result = (ViewResult)controller.Details(89).Result;

            // assert
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public void DetailsValidIdReturnsView()
        {
            // act
            var result = (ViewResult)controller.Details(567).Result;

            // assert
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]    
        public void DetailsValidIdReturnsCategory()
        {
            // act
            var result = (ViewResult)controller.Details(567).Result;

            // assert - is the corresponding record in our mock list the same as the record shown in the view?
            Assert.AreEqual(categories[1], result.Model);
        }
    }
}
