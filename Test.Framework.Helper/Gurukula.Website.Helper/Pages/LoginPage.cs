using System;
using OpenQA.Selenium;
using System.Configuration;
using GurukulaTestAutomationGeneric;

namespace Test.Framework.Helper.Gurukula.Website.Helper.Pages
{
	public class LoginPage
	{
		public static void GoTo()
		{
			String url = ConfigurationManager.AppSettings.Get("TestURL");
			Driver.Instance.Navigate().GoToUrl(url);
			Driver.Instance.Manage().Window.Maximize();
		}

		public static LoginCommand LoginAs(string userName)
		{
			return new LoginCommand(userName);
		}
	}

	public class LoginCommand
	{
		private readonly string userName;
		private string password;

		public LoginCommand(string userName)
		{
			this.userName = userName;
		}

		public LoginCommand WithPassword(string password)
		{
			this.password = password;
			return this;
		}

		public void Login()
		{
			try
			{

				var loginInput = Driver.Instance.FindElement(By.Name("SignInEmail"));
				loginInput.Clear();
				loginInput.SendKeys(userName);

				var passwordInput = Driver.Instance.FindElement(By.Name("SignInPassword"));
				passwordInput.SendKeys(password);

				var loginButton = Driver.Instance.FindElement(By.Id("SignInBtn"));
				loginButton.Click();

				try
				{
					if (Driver.Instance.FindElement(By.CssSelector("div.rpr-dialog-content-outer")).Enabled)
					{
						Driver.Instance.FindElement(By.LinkText("Close")).Click();
					}
					LogResults.LogInfo("Closed the duplicate user login window");
				}

				catch
				{
					LogResults.LogPass("Signed In successfully");
				}
			}

			catch
			{
				LogResults.LogFail("Failed to Login");
			}

		}
	}
}

	}
}
