using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace CAGISUnitTests
{
    [TestFixture]
    public class EmployeeTests
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of employee tests
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
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnEmployees")));
            IWebElement btnEmployees = driver.FindElement(By.Id("btnEmployees"));
            btnEmployees.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddEmployee")));
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
        /// Confirm successful user creation
        /// </summary>
        [Test, Order(0)]
        public void TestEmployee_SuccessfulUserCreation()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement btnSubmitUser;
            IWebElement txtPassword;
            IWebElement txtConfirmPassword;
            IWebElement txtEmail;

            IWebElement btnNewEmployee = driver.FindElement(By.Id("btnAddEmployee"));
            btnNewEmployee.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            string userName = "";
            bool isFirstTestUser = false;
            //allows for repeat testing
            for (int i = 0; i < 100; i++)
            {
                IWebElement txtUserName = driver.FindElement(By.Id("txtUserName"));
                txtUserName.SendKeys(Keys.Control + "a");
                txtUserName.SendKeys(Keys.Delete);
                txtUserName.SendKeys("Test" + i.ToString());

                txtEmail = driver.FindElement(By.Id("txtEmail"));
                txtEmail.SendKeys(Keys.Control + "a");
                txtEmail.SendKeys(Keys.Delete);
                //email to force an error after the first user
                txtEmail.SendKeys("Test0@gmail.com");

                txtPassword = driver.FindElement(By.Id("txtPassword"));
                txtPassword.SendKeys("Test1!");
                txtConfirmPassword = driver.FindElement(By.Id("txtConfirmPassword"));
                txtConfirmPassword.SendKeys("Test1!");

                btnSubmitUser = driver.FindElement(By.Id("btnSubmitUser"));
                btnSubmitUser.Click();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                try
                {
                    IWebElement error = driver.FindElement(By.Id("txtUserName-error"));
                    //if no errors, then break
                    if (error.Text == "")
                    {
                        userName = "Test" + i.ToString();
                        break;
                    }
                }
                //break out of loop to prevent error if first user
                catch
                {
                    userName = "Test" + i.ToString();
                    isFirstTestUser = true;
                    break;
                }
            }

            if (!isFirstTestUser)
            {
                txtEmail = driver.FindElement(By.Id("txtEmail"));
                txtEmail.SendKeys(Keys.Control + "a");
                txtEmail.SendKeys(Keys.Delete);
                txtEmail.SendKeys(userName + "@gmail.com");

                txtPassword = driver.FindElement(By.Id("txtPassword"));
                txtPassword.SendKeys("Test1!");
                txtConfirmPassword = driver.FindElement(By.Id("txtConfirmPassword"));
                txtConfirmPassword.SendKeys("Test1!");

                btnSubmitUser = driver.FindElement(By.Id("btnSubmitUser"));
                btnSubmitUser.Click();
            }

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnAddEmployee")));


            //find all employees on screen
            IReadOnlyList<IWebElement> employeeNames = driver.FindElements(By.Id("lblUserName"));
            IWebElement employeeFound = null;
            foreach (IWebElement element in employeeNames)
            {
                //find the employee element with the desired username
                if (element.Text == userName)
                {
                    employeeFound = element;
                    break;
                }
            }

            Assert.AreEqual(userName, employeeFound.Text);
        }

        /// <summary>
        /// Confirm successful removal of an employee
        /// </summary>
        /*[Test, Order(1)]
        public void TestEmployee_RemoveEmployee()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //find all users on screen
            IReadOnlyList<IWebElement> employeeNames = driver.FindElements(By.Id("lblUserName"));
            string userToRemove = "";
            int counter = 0;
            foreach (IWebElement element in employeeNames)
            {
                //find the employee element with the desired username
                if (element.Text != "Admin")
                {
                    userToRemove = element.Text;
                    break;
                }
                counter++;
            }

            //find all remove checkboxes on screen
            IReadOnlyList<IWebElement> checkBoxes = driver.FindElements(By.Id("chkRemove"));

            IWebElement selectButton = checkBoxes[counter];
            selectButton.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnUpdateList")));
            IWebElement btnUpdateList = driver.FindElement(By.Id("btnUpdateList"));
            btnUpdateList.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnAddEmployee")));


            //find all users on screen
            IReadOnlyList<IWebElement> employeeNames2 = driver.FindElements(By.Id("lblUserName"));
            bool userFound = false;
            foreach (IWebElement element in employeeNames2)
            {
                //find the employee element with the desired username
                if (element.Text == userToRemove)
                {
                    userFound = true;
                    break;
                }
            }

            Assert.AreEqual(false, userFound);
        }*/

        /// <summary>
        /// Confirm successful password change for admin
        /// </summary>
        [Test, Order(2)]
        public void TestEmployee_ChangePassword()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string changePasswordPath = "https://localhost:44395/Administration/ChangePassword/4a5d7ef2-713a-47af-af8e-f91750f404f9";
            string newPassword = "Admin1!";

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //find all users on screen
            IReadOnlyList<IWebElement> employeePasswords = driver.FindElements(By.Id("btnChangePassword"));
            IWebElement btnChangePassword = null;
            foreach (IWebElement element in employeePasswords)
            {
                string elementPath = element.GetAttribute("href");
                //find the employee element with the desired username
                if (elementPath == changePasswordPath)
                {
                    btnChangePassword = element;
                    break;
                }
            }

            btnChangePassword.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtNewPassword")));
            IWebElement txtNewPassword = driver.FindElement(By.Id("txtNewPassword"));
            txtNewPassword.SendKeys(newPassword);

            IWebElement txtConfirmPassword = driver.FindElement(By.Id("txtConfirmPassword"));
            txtConfirmPassword.SendKeys(newPassword);

            IWebElement btnUpdatePassword = driver.FindElement(By.Id("btnUpdatePassword"));
            btnUpdatePassword.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnLogOut")));
            IWebElement btnLogOut = driver.FindElement(By.Id("btnLogOut"));
            btnLogOut.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl("https://localhost:44395/Administration");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtUserName")));
            IWebElement txtUserName = driver.FindElement(By.Id("txtUserName"));
            txtUserName.SendKeys("Admin");
            IWebElement txtPassword = driver.FindElement(By.Id("txtPassword"));
            txtPassword.SendKeys(newPassword);
            IWebElement btnLogIn = driver.FindElement(By.Id("btnLogIn"));
            btnLogIn.Click();

            bool logInSuccess; 
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnEmployees")));
                //admin logged in successfully
                logInSuccess = true;
            }
            //if admin element isn't visible, then fail
            catch
            {
                logInSuccess = false;
            }

            Assert.AreEqual(true, logInSuccess);
        }
    }
}
