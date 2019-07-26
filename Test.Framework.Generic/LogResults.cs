using System;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace GurukulaTestAutomationGeneric

{
	public static class LogResults
    {
        //the purpose of the logger class is to offer a single point of entry that will support the output to
        //whereever the enterprise or team decides that logging is best placed.
        //places where the output can be logged are a server database, a text file, csv, console output, or any combination
        //depending on how the logger class is written.
        //the calls to the logger class are independent of where and how the logs are stored. even if there is a decision later to
        //direct logging output to a different repository or location, the calls to logging does not change, rather the class for 
        //logging changes.
        public static int DebugLevel = 0;
        public static bool LogDebugMessages = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("LogDebugMessages"));
        public static TestContext testContext
        {
            set
            {
                //LoggerDAL.LoggerDAL.testName = ((TestContext)value).TestName;
                testcontext = value;
                //need to set the debug level here....
                DebugLevel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DebugLevel"));
                LogInfo("Debug Level = " + DebugLevel);
            }
            get { return testcontext; }
        }

        private static TestContext testcontext;
        public static bool LoadTest = false;
        private static bool configuratorSet = false;
        //two types of possible outputs, a database and a console
        public static bool DatabaseOutput = false;
        public static bool ConsoleOutput = true;
        public static bool TestPass
        {
            get { return testPass; }
            private set { testPass = value; }
        }
        private static bool testPass = true;

        public enum MESSAGETYPE
        {
            ERROR,
            WARNING,
            DEBUG,
            INFORMATION,
            PASS,
            FAIL
        }

        /// <summary>
        /// this method sets this intance's debug level. it can be called at any time to modify the debug output
        /// a lower value will output the fewest debug maessages. a higher value will print a greater number of debug messages
        /// </summary>
        /// <param name="level"></param>
        public static void SetDebugFilterLevel(int level)
        {
            DebugLevel = level;

        }

        /// <summary>
        /// Logs a single line item in the results set with a status of Error. will update the test run summary to FAIL
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogError(string message)
        {
            LogMessage(message, MESSAGETYPE.ERROR);
        }
        /// <summary>
        /// Logs a single line of detail of type Warning. oes not affect the outcome of the test status
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogWarning(string message)
        {
            LogMessage(message, MESSAGETYPE.WARNING);
        }
        /// <summary>
        /// Logs a message of type Debug. this message is inserted to log values and conditions during code execution to serach for logic problems
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogDebug(string message)
        {
            if (LogDebugMessages)
            {
                LogMessage(message, MESSAGETYPE.DEBUG);
            }
        }
        /// <summary>
        /// Logs a message of type Debug. this message is inserted to log values and conditions during code execution to serach for logic problems
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        /// <param name="filterlevel">determines if a debug message should log or not. compare this value to 
        /// the value set by the method SetDebugFilterLevel. A lower value (1) will log before a greater value (10)</param>
        public static void LogDebug(string message, int filterlevel)
        {
            if (LogDebugMessages)
            {
                LogMessage(message, MESSAGETYPE.DEBUG, filterlevel);
            }
        }
        /// <summary>
        /// Logs a message to the log details. purely informational.
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogInfo(string message)
        {
            LogMessage(message, MESSAGETYPE.INFORMATION);
        }
        /// <summary>
        /// Logs a message to the log details. signifies that this specific entry has passed a test.
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogPass(string message)
        {
            LogMessage(message, MESSAGETYPE.PASS);
        }
        /// <summary>
        /// Logs a fail message to the log details. signifies that this specific entry has passed a test.
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        public static void LogFail(string message)
        {
            LogMessage(message, MESSAGETYPE.FAIL);

        }
 
        /// <summary>
        /// this method will save the submitted text to a separate file and link to that file in the test results
        /// </summary>
        /// <param name="valuesToSave"></param>
        /// <param name="fileName">a file name to save the text as. Do not include a file extension--it will be set as txt.</param>
        public static void SaveFileToTestResults(String valuesToSave, String fileName)
        {
            String saveFileName = testContext.TestResultsDirectory + "\\" + fileName + ".txt";

            // Although Test Context class creates test results directory itself, IF condition is added to take precautionary measure

            if (!Directory.Exists(testContext.TestResultsDirectory))
            {
                Directory.CreateDirectory(testContext.TestResultsDirectory);
            }
            FileUtility.WriteTextToFile(valuesToSave, saveFileName);
            testContext.AddResultFile(saveFileName);
        }
        /// <summary>
        /// base method for logging details, sans debug level
        /// </summary>
        /// <param name="message">value to be logged</param>
        /// <param name="messagetype">type of message that is getting logged i.e. WARNING, DEBUG, ERROR, MESSAGE</param>
        private static void LogMessage(string message, MESSAGETYPE messagetype)
        {
            if (!LoadTest)
            {
                string outputMessage = String.Empty;

                if (ConsoleOutput)
                {
                    if (!configuratorSet)
                    {
                        configuratorSet = true;
                    }

                    outputMessage = DateTime.Now + "::" + testContext.TestName;
                    switch (messagetype)
                    {
                        case MESSAGETYPE.DEBUG:
                            outputMessage += "::DEBUG::";
                            Console.ForegroundColor = ConsoleColor.White;
                            break;

                        case MESSAGETYPE.ERROR:
                            outputMessage += "::ERROR::";
                            Console.ForegroundColor = ConsoleColor.Red;
                            testPass = false;
                            break;
                        case MESSAGETYPE.FAIL:
                            outputMessage += "::FAIL::";
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            testPass = false;
                            break;
                        case MESSAGETYPE.INFORMATION:
                            outputMessage += "::INFORMATION::";
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case MESSAGETYPE.PASS:
                            outputMessage += "::PASS::";
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case MESSAGETYPE.WARNING:
                            outputMessage += "::WARNING::";
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;

                    }

                }
                testContext.WriteLine(outputMessage + ": " + message);
            }

        }
        /// <summary>
        /// base method for logging details, with debug level
        /// </summary>
        /// <param name="message">value to be logged</param>
        /// <param name="messagetype">type of message that is getting logged i.e. WARNING, DEBUG, ERROR, MESSAGE</param>
        /// <param name="filterlevel">if the messagetype is DEBUG, then this will determine if a debug message is logged.</param>
        private static void LogMessage(string message, MESSAGETYPE messagetype, int filterlevel)
        {
            //this is where the logger determines if a debug message is logged or not.
            if (filterlevel <= DebugLevel)
            {
                LogMessage(message, messagetype);
            }
        }

        /// <summary>
        /// this closes the logging session. it statuses the test summary to a pass or fail.
        /// </summary>
        public static bool EndLoggingSession()
        {
            if (LoadTest)
            {
                return true;
            }
            else
            {
                if (testcontext.CurrentTestOutcome != UnitTestOutcome.Passed)
                {
                    testPass = false;
                }
                return testPass;
            }
            //send back the final status of the overall test run - false if the overall test fails, true if the overall test passes.

        }

        /// <summary>
        /// this method creates a folder and a csv file within it to log test results in the local C: Drive
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>

        public static void WriteTestResultsToCSV(string testName, string result, Exception exception = null)
        {
            try
            {
                string resultsFilePath = GetApplicationStartupPath() + "\\logging";

                string exceptionDetails = "";
                if (exception != null)
                {
                    exceptionDetails = exception.Message;
                    if (exception.InnerException != null)
                    {
                        exceptionDetails += "---Internal Exception:" + exception.InnerException.Message.ToString();
                    }
                    exceptionDetails = exceptionDetails.Replace("\r", "");
                    exceptionDetails = exceptionDetails.Replace("\n", "");
                }
                if (System.IO.Directory.Exists(resultsFilePath) == false)
                {
                    System.IO.Directory.CreateDirectory(resultsFilePath);

                }
                resultsFilePath = resultsFilePath + "\\";
                string fileName = Path.Combine(resultsFilePath + "TestResults_" + DateTime.Now.Day + "_" + DateTime.Now.Month + ".csv");
                string delimiter = ",";

                if (!System.IO.File.Exists(fileName))
                {

                    using (StreamWriter w = new StreamWriter(fileName, true))
                    {

                        string headers = "TestName" + delimiter + "Result" + delimiter + "Description/Comments" + delimiter + "Script Executed On";

                        w.WriteLine(headers);
                        string date = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString() + "  -  " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                        w.WriteLine(testName + delimiter + result + delimiter + exceptionDetails + delimiter + date);

                        w.Flush();
                        w.Close();
                    }

                }

                else
                {
                    using (StreamWriter w = new StreamWriter(fileName, true))
                    {
                        string date = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                        w.WriteLine(testName + delimiter + result + delimiter + exceptionDetails + delimiter + date);

                        w.Flush();
                        w.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because it is being used by another process"))
                    MessageBox.Show("Test Results file might be open. Please close the file and restart the test");
                throw ex;
            }

        }

        private static string GetApplicationStartupPath()
        {
            var applicationStartPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(applicationStartPath);
        }


    }
}
