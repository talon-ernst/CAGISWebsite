using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace CAGISUnitTests
{
    [TestFixture]
    public class SearchTests
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of search tests
        /// </summary>
        [SetUp]
        public void SetUpMethod()
        {
            driver = new EdgeDriver("../../../bin/Debug/netcoreapp3.1/");
            driver.Navigate().GoToUrl("https://localhost:44395/Administration");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string blogOneTitle = "Unique Title";
            string blogTwoTitle = "Different Title";
            string blogThreeTitle = "Category Blog";
            string description = "A test blog created for the sole purpose of testing blog creation.";
            string categoryOne = "Unique Category";
            string categoryTwo = "Different Category";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtUserName")));
            IWebElement txtUserName = driver.FindElement(By.Id("txtUserName"));
            txtUserName.SendKeys("Admin");
            IWebElement txtPassword = driver.FindElement(By.Id("txtPassword"));
            txtPassword.SendKeys("Admin2!");
            IWebElement btnLogIn = driver.FindElement(By.Id("btnLogIn"));
            btnLogIn.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAdminBlog")));
            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnBlogBack")));
            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            bool blogsAdded = false;
            foreach (IWebElement element in blogTitles)
            {
                //find the blog title with the correct title
                if (element.Text == blogOneTitle)
                {
                    blogsAdded = true;
                    break;
                }
            }

            //add test blogs
            if (!blogsAdded)
            {

                IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
                btnAddBlog.Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateBlog")));
                IWebElement txtBlogTitle = driver.FindElement(By.Id("txtBlogTitle"));
                txtBlogTitle.SendKeys(blogOneTitle);

                IWebElement txtBlogText = driver.FindElement(By.Id("txtBlogText"));
                txtBlogText.SendKeys(description);

                IWebElement txtBlogCategory = driver.FindElement(By.Id("txtBlogCategory"));
                txtBlogCategory.SendKeys(categoryOne);

                IWebElement btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
                btnCreateCategory.Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));

                var categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
                var selectElement = new SelectElement(categoryDropdown);

                selectElement.SelectByText(categoryOne);

                IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
                btnCreateBlog.Click();

                //second blog
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
                btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
                btnAddBlog.Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateBlog")));
                txtBlogTitle = driver.FindElement(By.Id("txtBlogTitle"));
                txtBlogTitle.SendKeys(blogTwoTitle);

                txtBlogText = driver.FindElement(By.Id("txtBlogText"));
                txtBlogText.SendKeys(description);

                txtBlogCategory = driver.FindElement(By.Id("txtBlogCategory"));
                txtBlogCategory.SendKeys(categoryTwo);

                btnCreateCategory = driver.FindElement(By.Id("btnCreateCategory"));
                btnCreateCategory.Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));

                categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
                var selectElementTwo = new SelectElement(categoryDropdown);

                selectElementTwo.SelectByText(categoryTwo);

                btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
                btnCreateBlog.Click();

                //second blog
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
                btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
                btnAddBlog.Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCreateBlog")));
                txtBlogTitle = driver.FindElement(By.Id("txtBlogTitle"));
                txtBlogTitle.SendKeys(blogThreeTitle);

                txtBlogText = driver.FindElement(By.Id("txtBlogText"));
                txtBlogText.SendKeys(description);

                categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
                var selectElementThree = new SelectElement(categoryDropdown);

                selectElementThree.SelectByText(categoryOne);

                btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
                btnCreateBlog.Click();
            }
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
        /// Confirm all titles contain search term
        /// </summary>
        [Test]
        public void SearchTest_SearchBlogsByTitle()
        {
            driver.Navigate().GoToUrl("https://localhost:44395/Blogs");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string searchPhrase = "Title";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtSearch")));
            IWebElement txtSearch = driver.FindElement(By.Id("txtSearch"));
            txtSearch.SendKeys(searchPhrase);

            IWebElement btnSearch = driver.FindElement(By.Id("btnSearch"));
            btnSearch.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            bool allTitlesValid = true;
            foreach (IWebElement element in blogTitles)
            {
                //ensure all blogs have title
                if (!element.Text.Contains(searchPhrase))
                {
                    allTitlesValid = false;
                    break;
                }
            }
            Assert.IsTrue(allTitlesValid);
        }

        /// <summary>
        /// Confirm blogs contain searched for category
        /// </summary>
        [Test]
        public void SearchTest_SearchBlogsByCategory()
        {
            driver.Navigate().GoToUrl("https://localhost:44395/Blogs");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string searchPhrase = "Different Category";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));
            var categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
            var selectElement = new SelectElement(categoryDropdown);

            selectElement.SelectByText(searchPhrase);

            IWebElement btnSearch = driver.FindElement(By.Id("btnSearch"));
            btnSearch.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            blogTitles[0].Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("txtCategory")));
            IWebElement txtCategory = driver.FindElement(By.ClassName("txtCategory"));

            Assert.AreEqual("Category: " + searchPhrase, txtCategory.Text);
        }

        /// <summary>
        /// Confirm blogs contain searched for category and title
        /// </summary>
        [Test]
        public void SearchTest_SearchBlogsByCategoryAndTitle()
        {
            driver.Navigate().GoToUrl("https://localhost:44395/Blogs");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string searchPhrase = "Category";
            string searchCategory = "Unique Category";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("categoryDropdown")));
            IWebElement txtSearch = driver.FindElement(By.Id("txtSearch"));
            txtSearch.SendKeys(searchPhrase);

            var categoryDropdown = driver.FindElement(By.Id("categoryDropdown"));
            var selectElement = new SelectElement(categoryDropdown);

            selectElement.SelectByText(searchCategory);

            IWebElement btnSearch = driver.FindElement(By.Id("btnSearch"));
            btnSearch.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            bool searchValid = true;
            foreach (IWebElement element in blogTitles)
            {
                //ensure all blogs have title
                if (!element.Text.Contains(searchPhrase))
                {
                    searchValid = false;
                    break;
                }
            }
            blogTitles[0].Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("txtCategory")));
            IWebElement txtCategory = driver.FindElement(By.ClassName("txtCategory"));

            if (txtCategory.Text != "Category: " + searchCategory)
            {
                searchValid = false;
            }

            Assert.IsTrue(searchValid);
        }
    }
}
