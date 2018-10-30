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
            //TestObtainGamePackagePathFromGameProject();
            Console.ReadLine();
        }

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
