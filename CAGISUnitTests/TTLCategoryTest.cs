using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace CAGISUnitTests
{
    [TestFixture]
    public class CategoryTests
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of category tests
        /// </summary>
        [SetUp]
        public void SetUpMethod()
        {
            driver = new EdgeDriver("../../../bin/Debug/netcoreapp3.1/");
            driver.Navigate().GoToUrl("https://localhost:44395/Administration");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtUserName")));
            IWebElement txtUserName = driver.FindElement(By.Id("txtUserName"));
            txtUserName.SendKeys("Admin");
            IWebElement txtPassword = driver.FindElement(By.Id("txtPassword"));
            txtPassword.SendKeys("Admin2!");
            IWebElement btnLogIn = driver.FindElement(By.Id("btnLogIn"));
            btnLogIn.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAdminContests")));
        }

        [TearDown]
        public void TearDownMethod()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }
        /// <summary>
        /// Confirm Successful addition of category
        /// </summary>
        [Test]
        public void CategoryTest_AddCategory()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string categoryName = "Test Category";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateCategory")));
            IWebElement txtBlogCategory = driver.FindElement(By.Id("txtBlogCategory"));
            txtBlogCategory.SendKeys(categoryName);

            IWebElement btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
            btnCreateCategory.Click();

            //get all select list options
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));
            IWebElement categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
            var options = categoryDropdown.FindElements(By.TagName("option"));
            
            string addedCategory = "";
            foreach (var option in options)
            {
                //find the select option with the correct category name
                if (option.Text == categoryName)
                {
                    addedCategory = option.Text;
                    break;
                }
            }

            Assert.AreEqual(categoryName, addedCategory);
        }

        /// <summary>
        /// Confirm Error when adding duplicate of existing category
        /// </summary>
        [Test]
        public void CategoryTest_DuplicateCategory()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string errorText = "Cannot Add A Duplicate Category";
            string categoryName = "Duplicate Category";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateCategory")));
            IWebElement txtBlogCategory = driver.FindElement(By.Id("txtBlogCategory"));
            txtBlogCategory.SendKeys(categoryName);

            IWebElement btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
            btnCreateCategory.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateCategory")));
            btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
            txtBlogCategory.SendKeys(categoryName);

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtBlogCategory-error")));
            IWebElement categoryError = driver.FindElement(By.Id("txtBlogCategory-error"));

            string errorReceived = categoryError.Text;

            Assert.AreEqual(errorText, errorReceived);
        }

        /// <summary>
        /// Confirm Successful deletion of unused categories
        /// </summary>
        [Test]
        public void CategoryTest_ClearCategories()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string categoryName = "Empty Category";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateCategory")));
            IWebElement txtBlogCategory = driver.FindElement(By.Id("txtBlogCategory"));
            txtBlogCategory.SendKeys(categoryName);

            IWebElement btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
            btnCreateCategory.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAdminPanel")));
            IWebElement btnAdminPanel = driver.FindElement(By.Id("btnAdminPanel"));
            btnAdminPanel.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnClearCategories")));
            IWebElement btnClearCategories = driver.FindElement(By.Id("btnClearCategories"));
            btnClearCategories.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAdminBlog")));
            btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            //get all select list options
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));
            IWebElement categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
            var options = categoryDropdown.FindElements(By.TagName("option"));

            string addedCategory = "";
            foreach (var option in options)
            {
                //find the select option with the correct category name
                if (option.Text == categoryName)
                {
                    addedCategory = option.Text;
                    break;
                }
            }

            Assert.AreEqual("", addedCategory);
        }
    }
}
