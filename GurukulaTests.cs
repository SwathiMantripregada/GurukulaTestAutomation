using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPRTestAutomation;
using System;
using GurukulaTestAutomationGeneric;
using System.Threading;
using System.Configuration;


namespace Gurukula.Website
{
	public class GurukulaTests
	{
		[TestInitialize]
		public void Init()
		{
			LogResults.testContext = TestContext;
			Driver.Initialize();
			LoginPage.GoTo();
			LoginPage.LoginAs(ConfigurationManager.AppSettings.Get("UserName")).WithPassword(ConfigurationManager.AppSettings.Get("Password")).Login();
			
		}


		[TestCleanup]
		public void Cleanup()
		{
			try
			{
				MenuSelector.SelectMenu("TopLink_SignOut");
				LogResults.LogPass("Signed Out successfully");
			}

			catch (Exception ex)
			{
				LogResults.LogFail("Failed to Sign Out" + ex);
			}

			Driver.Close();
			Assert.IsTrue(LogResults.EndLoggingSession());

			Thread.Sleep(7000);


		}


		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}
		private TestContext testContextInstance;
	}
}
