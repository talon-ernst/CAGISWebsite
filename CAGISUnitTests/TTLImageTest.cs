using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

/* ******************************************
Unfortunately, due to the different file paths in everyone's computer, these tests only work for the individual who created them.
Any other user would need to change the pathing in the images.
****************************************** */
namespace CAGISUnitTests
{
    [TestFixture]
    class TTLImageTest
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of image tests
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
        /// Confirm Unsuccessful upload due to wrong type
        /// </summary>
        [Test]
        public void ImageTest_UploadWrongType()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string filePath = "C:\\Users\\Landon\\Desktop\\ThirdYearConestoga\\2_Capstone\\Project\\CAGISWebsite\\CAGISWebsite\\wwwroot\\Images\\UploadedContent\\README.txt";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload")));
            IWebElement imgImageUpload = driver.FindElement(By.Id("imgImageUpload"));
            imgImageUpload.SendKeys(filePath);

            IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
            btnCreateBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload-error")));
            IWebElement uploadError = driver.FindElement(By.Id("imgImageUpload-error"));

            Assert.AreEqual("Invalid image file, must select a *.jpeg, *.jpg, *.gif, or *.png file.", uploadError.Text);
        }

        /// <summary>
        /// Confirm Unsuccessful upload due to wrong size
        /// </summary>
        [Test]
        public void ImageTest_UploadWrongSize()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string filePath = "C:\\Users\\Landon\\Desktop\\ThirdYearConestoga\\2_Capstone\\Project\\CAGISWebsite\\CAGISWebsite\\wwwroot\\Images\\UnitTestContent\\BigImage.jpg";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload")));
            IWebElement imgImageUpload = driver.FindElement(By.Id("imgImageUpload"));
            imgImageUpload.SendKeys(filePath);

            IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
            btnCreateBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload-error")));
            IWebElement uploadError = driver.FindElement(By.Id("imgImageUpload-error"));

            Assert.AreEqual("File is too big, please upload image with a size less than 5MB.", uploadError.Text);
        }

        /// <summary>
        /// Confirm Unsuccessful upload due to image being too small
        /// </summary>
        [Test]
        public void ImageTest_UploadSmallImage()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string filePath = "C:\\Users\\Landon\\Desktop\\ThirdYearConestoga\\2_Capstone\\Project\\CAGISWebsite\\CAGISWebsite\\wwwroot\\Images\\UnitTestContent\\TINY.png";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload")));
            IWebElement imgImageUpload = driver.FindElement(By.Id("imgImageUpload"));
            imgImageUpload.SendKeys(filePath);

            IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
            btnCreateBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgImageUpload-error")));
            IWebElement uploadError = driver.FindElement(By.Id("imgImageUpload-error"));

            Assert.AreEqual("Image too small, please upload an image 100x100 or bigger.", uploadError.Text);
        }

        /// <summary>
        /// Confirm Successful upload of image
        /// </summary>
        [Test, Order(0)]
        public void ImageTest_UploadSuccessful()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string filePath = "C:\\Users\\Landon\\Desktop\\ThirdYearConestoga\\2_Capstone\\Project\\CAGISWebsite\\CAGISWebsite\\wwwroot\\Images\\WebsiteImages\\WebsiteIconBlog.png";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtBlogTitle")));
            IWebElement txtBlogTitle = driver.FindElement(By.Id("txtBlogTitle"));
            txtBlogTitle.SendKeys("Image Test Blog");

            IWebElement txtBlogText = driver.FindElement(By.Id("txtBlogText"));
            txtBlogText.SendKeys("A test blog created for the sole purpose of testing image creation.");

            IWebElement imgImageUpload = driver.FindElement(By.Id("imgImageUpload"));
            imgImageUpload.SendKeys(filePath);

            IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
            btnCreateBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));

            //find all blogs on screen
            IReadOnlyList<IWebElement> blogImages = driver.FindElements(By.Id("imgBlogImage"));
            string addedBlogImage = "";
            foreach (IWebElement element in blogImages)
            {
                IReadOnlyList<IWebElement> elementImage = element.FindElements(By.TagName("img"));
                foreach (IWebElement image in elementImage)
                {
                    string sourceImage = image.GetAttribute("src");
                    //find the blog title with the correct title
                    if (sourceImage == "https://localhost:44395/Images/UploadedContent/WebsiteIconBlog.png")
                    {
                        addedBlogImage = sourceImage;
                        break;
                    }
                }
                if (addedBlogImage != "")
                    break;
            }
            Assert.AreEqual("https://localhost:44395/Images/UploadedContent/WebsiteIconBlog.png", addedBlogImage);
        }

        /// <summary>
        /// Confirm Successful removal of an image from a blog
        /// </summary>
        [Test, Order(1)]
        public void ImageTest_RemoveImageSuccessful()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnBlogBack")));

            //find all blogs on screen
            IReadOnlyList<IWebElement> blogImages = driver.FindElements(By.Id("imgBlogImage"));
            string addedBlogImage = "";
            int counter = 0;
            foreach (IWebElement element in blogImages)
            {
                IReadOnlyList<IWebElement> elementImage = element.FindElements(By.TagName("img"));
                foreach (IWebElement image in elementImage)
                {
                    string sourceImage = image.GetAttribute("src");
                    //find the blog title with the correct title
                    if (sourceImage == "https://localhost:44395/Images/UploadedContent/WebsiteIconBlog.png")
                    {
                        addedBlogImage = sourceImage;
                        break;
                    }
                    if (addedBlogImage != "")
                        break;
                    counter++;
                }
            }
            IReadOnlyList<IWebElement> editButtons = driver.FindElements(By.Id("btnEditBlog"));
            editButtons[counter].Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnRemoveImage")));
            IWebElement btnRemoveImage = driver.FindElement(By.Id("btnRemoveImage"));
            btnRemoveImage.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("imgBlogImage")));
            IWebElement imgBlogImage = driver.FindElement(By.Id("imgBlogImage"));


            Assert.AreEqual("https://localhost:44395/Images/WebsiteImages/Cagis_logo_colour.png", imgBlogImage.GetAttribute("src"));
        }
    }
}
