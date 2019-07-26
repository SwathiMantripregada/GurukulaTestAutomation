using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace GurukulaTestAutomationGeneric
{
    public class FileUtility
    {
        public static string GetDataFromFile(string filePath)
        {
            string data = "";

            using (Stream s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader sr = new StreamReader(s);
                data = sr.ReadToEnd();
            }

            return data;
        }

        public static string GetDataFromFileBeingLockedByOtherProcess(string filePath)
        {
            string data = "";

            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (StreamReader sr = new StreamReader(stream))
            {
                data = sr.ReadToEnd();
            }

            return data;
        }

        public static string GetDataFromStream(Stream stream)
        {
            string data = "";

            using (StreamReader sr = new StreamReader(stream))
            {
                data = sr.ReadToEnd();
            }

            return data;
        }

        public static String ReadDataFileFromEmbeddedResource(String sFilename)
        {
            String fileContents = String.Empty;

            Assembly assembly = Assembly.Load(new AssemblyName("Test.Automation.Repository"));
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("Test.Automation.Repository.Data." + sFilename)))
            {
                fileContents = reader.ReadToEnd();
            }
            return fileContents;
        }

        public static Stream GetStreamFromFile(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public static string GetDataAddedAfterLastKnownStateFromMultipleFiles(string filePath, string fileNamePartialText, out String currentFileName, string previousFileName = null)
        {
            string logFileDirectory = filePath;
            string logFilePath = null;
            string fileContent = string.Empty;
            var directory = new DirectoryInfo(logFileDirectory);
            var logFile = (from f in directory.GetFiles()
                           where f.Name.Contains(fileNamePartialText)
                           orderby f.Name descending
                           select f).First();
            currentFileName = logFile.Name;

            if (previousFileName != null && previousFileName != currentFileName)
            {
                logFilePath = logFileDirectory + previousFileName;
                fileContent = (File.Exists(logFilePath)) ? FileUtility.GetDataFromFileBeingLockedByOtherProcess(logFilePath) : string.Empty;
            }
            logFilePath = logFileDirectory + currentFileName;
            fileContent += (File.Exists(logFilePath)) ? FileUtility.GetDataFromFileBeingLockedByOtherProcess(logFilePath) : string.Empty;
            return fileContent;
        }


        /// <summary>
        /// This method is a simpler approach to dumping text into a file
        /// </summary>
        /// <param name="textToSave"></param>
        /// <param name="fileName"></param>
        public static void WriteTextToFile(String textToSave, String fileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(textToSave);
                    writer.Close();
                }
            }
            catch
            {
                //log an error or throw an error?
            }
        }
        public static string ConverToANSIEncoding(string encodedFilePath)
        {
            var encodedUTF8File = new StreamReader(encodedFilePath);

            var info = new FileInfo(encodedFilePath);

            string encodedANSIFilePath = string.Format("{0}\\{1}_ANSI{2}",
                                                       info.Directory,
                                                       Path.GetFileNameWithoutExtension(info.FullName),
                                                       info.Extension);

            var encodedANSIFile =
                new StreamWriter(encodedANSIFilePath, false, Encoding.Default);

            encodedANSIFile.Write(encodedUTF8File.ReadToEnd());
            encodedUTF8File.Close();
            encodedANSIFile.Close();

            return encodedANSIFilePath;
        }

        public static Stream ConvertToANSIStream(Stream UTFEncodedData)
        {
            StreamReader UTFReader = new StreamReader(UTFEncodedData);

            StreamWriter ANSIWriter = new StreamWriter(new MemoryStream(), Encoding.Default);

            ANSIWriter.Write(UTFReader.ReadToEnd());
            ANSIWriter.Flush();

            return ANSIWriter.BaseStream;
        }

        public static string GetPath()
        {
            string xmlESBConfigPath1 = System.AppDomain.CurrentDomain.BaseDirectory;

            String[] a = xmlESBConfigPath1.Split('\\');

            string path = "";
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == "TestResults")
                {
                    break;
                }
                {
                    path = path + a[i] + "\\";
                }

            }
            return path;
        }

        public string GetLatestFileCreated(string path)
        {
            var directory = new DirectoryInfo(path);
            return directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First().Name;
        }

        public static void FileExtractAndDrop(string nameSpace, string internalFilePath, string resourceName, string dropLocation, string folderName)
        {
            //nameSpace = the namespace of your project, located right above your class' name;
            //folderName= specific folder for client
            //outDirectory = where the file will be extracted to;
            //internalFilePath = the name of the folder inside visual studio which the files are in;
            //resourceName = the name of the file;
            Assembly assembly = Assembly.Load(new AssemblyName(nameSpace));
            string outDirectory = @dropLocation + "\\" + folderName;
            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            {
                using (BinaryReader r = new BinaryReader(s))
                {
                    using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter w = new BinaryWriter(fs))
                        {
                            w.Write(r.ReadBytes((int)s.Length));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Picks the lately created file in Output folder and returns file name
        /// </summary>
        /// <param name ="folderPath">Folder path from where recently moved file name is needed</param>        
        /// <returns></returns>
        public static string GetRecentlyMovedFile(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            var outputFile = directory.GetFiles("*.txt").OrderByDescending(f => f.LastWriteTime).First();
            return outputFile.Name;
        }

        public String ReadExpectedDataFileFromEmbeddedResource(String sFilename)
        {
            String query = String.Empty;

            Assembly assembly = Assembly.Load(new AssemblyName("Test.Automation.Repository"));
            String[] resources = assembly.GetManifestResourceNames();
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("Test.Automation.Repository.Data." + sFilename)))
            {
                query = reader.ReadToEnd();
            }
            return query;
        }


    }
}

