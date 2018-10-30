using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCommonUtilities
{
    class FileFinderUtility
    {
        public delegate void GenEventHandler(int _stat);
        public event GenEventHandler OnFilesReadUpdate;
        public event GenEventHandler OnFilesFoundUpdate;
        public event GenEventHandler OnDirectoriesFoundUpdate;

        public int filesRead
        {
            get { return _filesRead; }
            protected set
            {
                _filesRead = value;
                if (OnFilesReadUpdate != null) OnFilesReadUpdate(_filesRead);
            }
        }
        public int filesFound
        {
            get { return _filesFound; }
            protected set
            {
                _filesFound = value;
                if (OnFilesFoundUpdate != null) OnFilesFoundUpdate(_filesFound);
            }
        }
        public int directoriesFound
        {
            get { return _directoriesFound; }
            protected set
            {
                _directoriesFound = value;
                if (OnDirectoriesFoundUpdate != null) OnDirectoriesFoundUpdate(_directoriesFound);
            }
        }

        int _filesRead = 0;
        int _filesFound = 0;
        int _directoriesFound = 0;

        string[] _fileList = new string[0];

        public FileFinderUtility()
        {
            InitializeFinder();
        }

        void InitializeFinder()
        {
            filesRead = 0;
            filesFound = 0;
            directoriesFound = 0;
            _fileList = new string[0];
        }

        //Can be called by any class
        public async Task<string[]> ReadFromDirectory(string _dir, Func<string, bool> _filePathCondition = null)
        {
            InitializeFinder();
            if (Directory.Exists(_dir))
            {
                return await GetAllFilesAsync(_dir, _filePathCondition);
            }
            else
            {
                return null;
                //throw new SystemException($"Cannot find file from: ${_dir}");
            }
        }

        //Can be called by any class
        public Task<string[]> GetReadFromDirTask(string _dir, Func<string, bool> _filePathCondition = null)
        {
            InitializeFinder();
            if (Directory.Exists(_dir))
            {
                //return await GetAllFilesAsync(_dir, _filePathCondition);
                return GetAllFilesWrapper(_dir, _filePathCondition);
            }
            else
            {
                return null;
                //throw new SystemException($"Cannot find file from: ${_dir}");
            }
        }

        private Task<string[]> GetAllFilesWrapper(string _dir, Func<string, bool> _filePathCondition = null)
        {
            return Task.Factory.StartNew(() =>
            GetAllFiles(_dir, _filePathCondition), CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
        }

        private string[] GetAllFiles(string _dir, Func<string, bool> _filePathCondition = null)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(_dir))
                {
                    filesRead++;
                    if (_filePathCondition != null && _filePathCondition(f))
                    {
                        filesFound++;
                        files.Add(f);
                    }
                }
                foreach (string d in Directory.GetDirectories(_dir))
                {
                    directoriesFound++;
                    //Thread.Sleep(500);
                    files.AddRange(GetAllFiles(d, _filePathCondition));
                }
            }
            catch (System.Exception excpt)
            {
                FileFinderLog(excpt.Message, true);
            }

            return files.ToArray();
        }

        private async Task<string[]> GetAllFilesAsync(string _dir, Func<string, bool> _filePathCondition = null)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(_dir))
                {
                    filesRead++;
                    if (_filePathCondition != null && _filePathCondition(f))
                    {
                        filesFound++;
                        files.Add(f);
                    }
                }
                foreach (string d in Directory.GetDirectories(_dir))
                {
                    directoriesFound++;
                    //Thread.Sleep(500);
                    files.AddRange(await GetAllFilesAsync(d, _filePathCondition));
                }
            }
            catch (System.Exception excpt)
            {
                await FileFinderLog(excpt.Message, true);
            }

            return files.ToArray();
        }

        protected async virtual Task FileFinderLog(string _msg, bool _exception = false)
        {
            //Override To Implement Logging
        }
    }
}
