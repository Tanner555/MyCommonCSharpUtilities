using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCommonUtilities
{
    /// <summary>
    /// These Functions Come From the Plugin Installer For the USharp UE4 Plugin
    /// </summary>
    class MsbuildHelperUtility
    {
        public static bool BuildCs(string solutionPath, string projectPath, bool debug, bool x86, string customDefines)
        {
            string msbuildPath = FindMsBuildPath();

            if (string.IsNullOrEmpty(msbuildPath))
            {
                return false;
            }

            string config = debug ? "Debug" : "Release";
            string platform = x86 ? "x86" : "\"Any CPU\"";
            string fileArgs = "\"" + solutionPath + "\"" + " /p:Configuration=" + config + " /p:Platform=" + platform;
            if (!string.IsNullOrEmpty(projectPath))
            {
                // '.' must be replaced with '_' for /t
                string projectName = Path.GetFileNameWithoutExtension(projectPath).Replace(".", "_");

                // Skip project references just in case (this means projects should be built in the correct order though)
                fileArgs += " /t:" + projectName + " /p:BuildProjectReferences=false";
            }
            if (!string.IsNullOrEmpty(customDefines))
            {
                Debug.Assert(!customDefines.Contains(' '));
                fileArgs += " /p:DefineConstants=" + customDefines;
            }

            const string buildLogFile = "build.log";

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = msbuildPath,
                    Arguments = fileArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    int timeout = 60000;
                    bool built = process.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout);

                    File.AppendAllText(buildLogFile, "Build sln '" + solutionPath + "' proj '" + projectPath + "'" + Environment.NewLine);
                    File.AppendAllText(buildLogFile, string.Empty.PadLeft(100, '-') + Environment.NewLine);
                    File.AppendAllText(buildLogFile, output.ToString() + Environment.NewLine);
                    File.AppendAllText(buildLogFile, error.ToString() + Environment.NewLine + Environment.NewLine);

                    if (!built)
                    {
                        Console.WriteLine("Failed to wait for compile.");
                    }

                    return built && process.ExitCode == 0;
                }
            }
        }

        public static string FindMsBuildPath()
        {
            try
            {
                string baseMicrosoftKeyPath = @"SOFTWARE\WOW6432Node\Microsoft";
                string visualStudioRegistryKeyPath = baseMicrosoftKeyPath + @"\VisualStudio\SxS\VS7";

                //Try Obtaining the VS version of MSBuild First
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(visualStudioRegistryKeyPath))
                {
                    if (key != null)
                    {
                        string path = key.GetValue("15.0") as string;
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = Path.Combine(path, "MSBuild", "15.0", "Bin", "msbuild.exe");
                            if (File.Exists(path))
                            {
                                return path;
                            }
                        }
                    }
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBUILD\ToolsVersions\4.0"))
                {
                    string path = key.GetValue("MSBuildToolsPath") as string;
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Path.Combine(path, "msbuild.exe");
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
