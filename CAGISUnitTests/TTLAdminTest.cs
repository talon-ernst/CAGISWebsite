using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace CAGISUnitTests
{
    [TestFixture]
    public class AdminTests
    {
        IWebDriver driver;

        /// <summary>
        /// Create starting points of admin tests
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
        /// Confirm Successful addition of blog
        /// </summary>
        [Test, Order(0)]
        public void AdminTest_AddBlog()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string blogTitle = "Title of Test Blog";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));
            IWebElement btnAddBlog = driver.FindElement(By.Id("btnAddBlog"));
            btnAddBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtBlogTitle")));
            IWebElement txtBlogTitle = driver.FindElement(By.Id("txtBlogTitle"));
            txtBlogTitle.SendKeys(blogTitle);

            IWebElement txtBlogText = driver.FindElement(By.Id("txtBlogText"));
            txtBlogText.SendKeys("A test blog created for the sole purpose of testing blog creation.");

            IWebElement btnCreateBlog = driver.FindElement(By.Id("btnCreateBlog"));
            btnCreateBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddBlog")));

            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            string addedBlogTitle = "";
            foreach (IWebElement element in blogTitles)
            {
                //find the blog title with the correct title
                if (element.Text == blogTitle)
                {
                    addedBlogTitle = element.Text;
                    break;
                }
            }

            Assert.AreEqual(blogTitle, addedBlogTitle);
        }

        /// <summary>
        /// Confirm Successful deletion of blog
        /// </summary>
        [Test, Order(1)]
        public void AdminTest_DeleteBlog()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string blogTitle = "Title of Test Blog";

            IWebElement btnAdminBlog = driver.FindElement(By.Id("btnAdminBlog"));
            btnAdminBlog.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnBlogBack")));

            //find all blogs on screen
            IReadOnlyList<IWebElement> blogTitles = driver.FindElements(By.Id("lblBlogTitle"));
            int counter = 0;
            foreach (IWebElement element in blogTitles)
            {
                //find the blog title with the correct title
                if (element.Text == blogTitle)
                {
                    break;
                }
                counter++;
            }

            //find all delete buttons on screen
            IReadOnlyList<IWebElement> deleteBlogs = driver.FindElements(By.Id("btnDeleteBlog"));

            IWebElement btnDeleteBlog = deleteBlogs[counter];
            btnDeleteBlog.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnBlogBack")));


            //find all blog titles on screen
            IReadOnlyList<IWebElement> blogTitles2 = driver.FindElements(By.Id("lblBlogTitle"));
            bool blogFound = false;
            foreach (IWebElement element in blogTitles2)
            {
                //find the title element with the desired title
                if (element.Text == blogTitle)
                {
                    blogFound = true;
                    break;
                }
            }

            Assert.AreEqual(false, blogFound);
        }

        /// <summary>
        /// Confirm Successful addition of Did You Know
        /// </summary>
        [Test, Order(2)]
        public void AdminTest_AddFact()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string factTitle = "Title of Test Did You Know?";

            IWebElement btnAdminDYK = driver.FindElement(By.Id("btnAdminDYK"));
            btnAdminDYK.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddDYK")));
            IWebElement btnAddDYK = driver.FindElement(By.Id("btnAddDYK"));
            btnAddDYK.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtDYKTitle")));
            IWebElement txtDYKTitle = driver.FindElement(By.Id("txtDYKTitle"));
            txtDYKTitle.SendKeys(factTitle);

            IWebElement txtDYKText = driver.FindElement(By.Id("txtDYKText"));
            txtDYKText.SendKeys("A test Did You Know created for the sole purpose of testing Did You Know creation.");

            IWebElement btnCreateDYK = driver.FindElement(By.Id("btnCreateDYK"));
            btnCreateDYK.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddDYK")));

            //find all Did You Knows on screen
            IReadOnlyList<IWebElement> factTitles = driver.FindElements(By.Id("lblDYKTitle"));
            string addedFactTitle = "";
            foreach (IWebElement element in factTitles)
            {
                //find the fact title with the correct title
                if (element.Text == factTitle)
                {
                    addedFactTitle = element.Text;
                    break;
                }
            }

            Assert.AreEqual(factTitle, addedFactTitle);
        }

        /// <summary>
        /// Confirm Successful deletion of Did You Know
        /// </summary>
        [Test, Order(3)]
        public void AdminTest_DeleteFact()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string factTitle = "Title of Test Did You Know?";

            IWebElement btnAdminDYK = driver.FindElement(By.Id("btnAdminDYK"));
            btnAdminDYK.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnFactBack")));

            //find all Did you knows on screen
            IReadOnlyList<IWebElement> factTitles = driver.FindElements(By.Id("lblDYKTitle"));
            int counter = 0;
            foreach (IWebElement element in factTitles)
            {
                //find the Did you know title with the correct title
                if (element.Text == factTitle)
                {
                    break;
                }
                counter++;
            }

            //find all delete buttons on screen
            IReadOnlyList<IWebElement> deleteFacts = driver.FindElements(By.Id("btnDeleteDYK"));

            IWebElement btnDeleteFact = deleteFacts[counter];
            btnDeleteFact.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnFactBack")));


            //find all Did you know titles on screen
            IReadOnlyList<IWebElement> factTitles2 = driver.FindElements(By.Id("lblDYKTitle"));
            bool factFound = false;
            foreach (IWebElement element in factTitles2)
            {
                //find the title element with the desired title
                if (element.Text == factTitle)
                {
                    factFound = true;
                    break;
                }
            }

            Assert.AreEqual(false, factFound);
        }

        /// <summary>
        /// Confirm Successful addition of activity
        /// </summary>
        [Test, Order(4)]
        public void AdminTest_AddActivity()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string activityTitle = "Title of Test Activity";

            IWebElement btnAdminActivities = driver.FindElement(By.Id("btnAdminActivities"));
            btnAdminActivities.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddActivity")));
            IWebElement btnAddActivity = driver.FindElement(By.Id("btnAddActivity"));
            btnAddActivity.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtActivityTitle")));
            IWebElement txtActivityTitle = driver.FindElement(By.Id("txtActivityTitle"));
            txtActivityTitle.SendKeys(activityTitle);

            IWebElement txtActivityText = driver.FindElement(By.Id("txtActivityText"));
            txtActivityText.SendKeys("A test activity created for the sole purpose of testing activity creation.");

            IWebElement btnCreateActivity = driver.FindElement(By.Id("btnCreateActivity"));
            btnCreateActivity.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddActivity")));

            //find all blogs on screen
            IReadOnlyList<IWebElement> activityTitles = driver.FindElements(By.Id("lblActivityTitle"));
            string addedActivityTitle = "";
            foreach (IWebElement element in activityTitles)
            {
                //find the blog title with the correct title
                if (element.Text == activityTitle)
                {
                    addedActivityTitle = element.Text;
                    break;
                }
            }

            Assert.AreEqual(activityTitle, addedActivityTitle);
        }

        /// <summary>
        /// Confirm Successful deletion of activity
        /// </summary>
        [Test, Order(5)]
        public void AdminTest_DeleteActivity()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string activityTitle = "Title of Test Activity";

            IWebElement btnAdminActivities = driver.FindElement(By.Id("btnAdminActivities"));
            btnAdminActivities.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnActivityBack")));

            //find all activities on screen
            IReadOnlyList<IWebElement> activityTitles = driver.FindElements(By.Id("lblActivityTitle"));
            int counter = 0;
            foreach (IWebElement element in activityTitles)
            {
                //find the activity title with the correct title
                if (element.Text == activityTitle)
                {
                    break;
                }
                counter++;
            }

            //find all delete buttons on screen
            IReadOnlyList<IWebElement> deleteActivities = driver.FindElements(By.Id("btnDeleteActivity"));

            IWebElement btnDeleteActivity = deleteActivities[counter];
            btnDeleteActivity.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnActivityBack")));


            //find all activity titles on screen
            IReadOnlyList<IWebElement> activityTitles2 = driver.FindElements(By.Id("lblActivityTitle"));
            bool activityFound = false;
            foreach (IWebElement element in activityTitles2)
            {
                //find the title element with the desired title
                if (element.Text == activityTitle)
                {
                    activityFound = true;
                    break;
                }
            }

            Assert.AreEqual(false, activityFound);
        }

        /// <summary>
        /// Confirm Successful addition of contest
        /// </summary>
        [Test, Order(6)]
        public void AdminTest_AddContest()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string contestTitle = "Title of Test Contest";

            IWebElement btnAdminContests = driver.FindElement(By.Id("btnAdminContests"));
            btnAdminContests.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnAddContest")));
            IWebElement btnAddContest = driver.FindElement(By.Id("btnAddContest"));
            btnAddContest.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtContestTitle")));
            IWebElement txtContestTitle = driver.FindElement(By.Id("txtContestTitle"));
            txtContestTitle.SendKeys(contestTitle);

            IWebElement txtContestText = driver.FindElement(By.Id("txtContestText"));
            txtContestText.SendKeys("A test contest created for the sole purpose of testing contest creation.");

            IWebElement txtEmail = driver.FindElement(By.Id("txtEmail"));
            txtEmail.SendKeys("contest@CAGIS.ca");

            IWebElement dateContestStart = driver.FindElement(By.Id("dateContestStart"));
            dateContestStart.SendKeys("00202210101010a");//long string representing date

            IWebElement dateContestEnd = driver.FindElement(By.Id("dateContestEnd"));
            dateContestEnd.SendKeys("00202310101010a");//long string representing date

            IWebElement btnCreateContest = driver.FindElement(By.Id("btnCreateContest"));
            btnCreateContest.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnContestBack")));

            //find all contests on screen
            IReadOnlyList<IWebElement> contestTitles = driver.FindElements(By.Id("lblContestTitle"));
            string addedContestTitle = "";
            foreach (IWebElement element in contestTitles)
            {
                //find the contest title with the correct title
                if (element.Text == contestTitle)
                {
                    addedContestTitle = element.Text;
                    break;
                }
            }

            Assert.AreEqual(contestTitle, addedContestTitle);
        }

        /// <summary>
        /// Confirm Successful deletion of contest
        /// </summary>
        [Test, Order(7)]
        public void AdminTest_DeleteContest()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string contestTitle = "Title of Test Contest";

            IWebElement btnAdminContests = driver.FindElement(By.Id("btnAdminContests"));
            btnAdminContests.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnContestBack")));

            //find all conetests on screen
            IReadOnlyList<IWebElement> contestTitles = driver.FindElements(By.Id("lblContestTitle"));
            int counter = 0;
            foreach (IWebElement element in contestTitles)
            {
                //find the contest title with the correct title
                if (element.Text == contestTitle)
                {
                    break;
                }
                counter++;
            }

            //find all delete buttons on screen
            IReadOnlyList<IWebElement> deleteContests = driver.FindElements(By.Id("btnDeleteContest"));

            IWebElement btnDeleteContest = deleteContests[counter];
            btnDeleteContest.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.SwitchTo().Alert().Accept();

            wait.Until(ExpectedConditions.ElementExists(By.Id("btnContestBack")));


            //find all blog titles on screen
            IReadOnlyList<IWebElement> contestTitles2 = driver.FindElements(By.Id("lblContestTitle"));
            bool contestFound = false;
            foreach (IWebElement element in contestTitles2)
            {
                //find the title element with the desired title
                if (element.Text == contestTitle)
                {
                    contestFound = true;
                    break;
                }
            }

            Assert.AreEqual(false, contestFound);
        }
    }
}
