using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CAGISUnitTests
{
    [TestFixture]
    public class RegisterTests
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of tests with a new user on the register page
        /// </summary>
        [SetUp]
        public void SetUpMethod()
        {
            driver = new ChromeDriver("../../../bin/Debug/netcoreapp3.1/");
            driver.Navigate().GoToUrl("https://localhost:44338/");

            /*WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnRegister")));

            IWebElement btnRegister = driver.FindElement(By.Id("btnRegister"));
            btnRegister.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtUserName")));*/
        }

        [TearDown]
        public void TearDownMethod()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }

        
    }
}
