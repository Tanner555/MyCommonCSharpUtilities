using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommonUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello From Common Utilities");            
            Console.ReadLine();
        }

        #region TestingMonthAYearUtility
        /// <summary>
        /// Test the Util with a formatted date
        /// IE: "5/9/2022"
        /// </summary>
        static void CreateNewMonthAYearUtilityTest(string dateFormatted)
        {
            var _monthAYearGroup = new MyMonthAYearGroupUtility(dateFormatted);
            Console.WriteLine($"Month and Year from {dateFormatted} is: {_monthAYearGroup.DateByMonth} / {_monthAYearGroup.DateByYear}");
            Console.WriteLine($"DayCount From Month/Year {"02/2022"} is {MyMonthAYearGroupUtility.GetDayCountFromMonthAYear(2, 2022)}");
        }

        #endregion

        #region TestingJSONSerialization
        static void TestingJSONSerialization()
        {
            JSONSerializerUtility.TestJSONSerialization();
        }
        #endregion

        #region TestingInvokeTimer
        static void TestingInvokeTimer()
        {
            Console.WriteLine("Hello From Invoke Timer...");

            bool _continueRunning = true;
            InvokeTimer.InvokeRepeating(1000f, TestTimerCallback);
            do
            {
                Console.WriteLine("Running Program...");
                var _key = Console.ReadKey();
                if (_key.Key == ConsoleKey.Escape)
                {
                    _continueRunning = false;
                }
                else if (_key.Key == ConsoleKey.S)
                {
                    InvokeTimer.CancelInvoke();
                }
            } while (_continueRunning);

            Console.WriteLine("See you later...");
        }

        static void TestTimerCallback()
        {
            Console.WriteLine("Testing Callback");
        }
        #endregion

        #region TestingObtainingGamePackagePath
        static void TestObtainGamePackagePathFromGameProject()
        {
            Console.WriteLine("Provide Your Game Project Path...");
            string _gameProjectPath = Console.ReadLine();
            if (!string.IsNullOrEmpty(_gameProjectPath) && Directory.Exists(_gameProjectPath))
            {
                Console.WriteLine("Trying To Obtain Game Package...");
                string _packagePath = ObtainPackagePathFromGameProject(_gameProjectPath, new DirectoryInfo(_gameProjectPath).Name);
                if (!string.IsNullOrEmpty(_packagePath))
                {
                    Console.WriteLine("Obtained Package Path at: " + _packagePath);
                }
                else
                {
                    Console.WriteLine("Couldn't Find Package Path...");
                }
            }
        }

        static string ObtainPackagePathFromGameProject(string _gameProjectPath, string _gameProjectName)
        {
            string _logpath = Path.Combine(_gameProjectPath, "Saved", "Logs", _gameProjectName + ".log");
            string _uatPackageLine = @"UATHelper: Packaging (Windows (64-bit)): Parsing command line: -ScriptsForProject=";
            string _archiveString = "-archivedirectory";
            return SimpleFileParserUtility.ObtainStringFromSharedFileContent(_logpath, _uatPackageLine, _archiveString, '"', '"');
        }
        #endregion
    }
}