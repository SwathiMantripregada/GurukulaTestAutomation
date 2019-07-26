using System;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Configuration;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace RPRTestAutomationGeneric

{
	public class Driver
    {
        public static IWebDriver Instance { get; set; }

        /// <summary>
        /// this method returns the current instance of the selenium web driver. if there is not a current instance
        /// it will create a new instance and pass that back.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver GetDriver()
        {
            String browserType = ConfigurationManager.AppSettings.Get("Browser");

            switch (browserType.ToLower())
            {
                case "firefox":

                    Instance = new FirefoxDriver();
                    break;
                case "chrome":
                    Instance = new ChromeDriver();
                    break;
                case "ie":
                    Instance = new InternetExplorerDriver();
                    break;
            }

            return Instance;
        }

 

        public static void Initialize()
        {
            Instance = GetDriver();
            Instance.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }


        public static bool ElementByIdDisplayed(string elementName)  //, int timeoutInSeconds)
        {
            try
            {
                var wait = new WebDriverWait(Instance, TimeSpan.FromSeconds(30));

                var myElement = wait.Until(x => x.FindElement(By.Id(elementName)));

                while (!((myElement.Displayed) && (myElement.Enabled)))
                    Thread.Sleep(500);

                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public static void Close()
        {
            Instance.Quit();
            Instance = null;
        }
    }
}