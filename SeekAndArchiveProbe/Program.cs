using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.IO.Compression;


namespace SeakAndArchive
{
    class Program
    {
        static List<FileInfo> FoundFiles;

        static string directoryName;
        static string fileName;

        public static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        static void Main(string[] args)
        {
            fileName = args[0];
            directoryName = args[1];
            FoundFiles = new List<FileInfo>();

            //examine if the given directory exists at all 
            DirectoryInfo rootDir = new DirectoryInfo(directoryName);
            if (!rootDir.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }
            else
            {
                //search recursively for the mathing files
                RecursiveSearch(FoundFiles, fileName, rootDir);
                //list the found files
                Console.WriteLine("Found {0} files.", FoundFiles.Count);
                foreach (FileInfo fil in FoundFiles)
                {
                    Console.WriteLine("{0}", fil.FullName);
                }
                
            }

            ConfigureFileWatcher();
        }

        static void RecursiveSearch(List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory)
        {
            foreach (FileInfo fil in currentDirectory.GetFiles())
            {
                if (fil.Name == fileName)
                    foundFiles.Add(fil);
            }

            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, fileName, dir);
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void ConfigureFileWatcher()
        {
            //string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            //if (args.Length != 2)
            //{
            //    // Display the proper way to call the program.
            //    Console.WriteLine("Usage: Watcher.exe (directory)");
            //    return;
            //}

            foreach (FileInfo fileinfo in FoundFiles)
            {
                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = fileinfo.DirectoryName;
                /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                // Only watch file with name from the Foundfiles list.
                watcher.Filter = fileinfo.Name;
                // Add event handlers.
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                //watcher.Created += new FileSystemEventHandler(OnChanged);
      
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                watchers.Add(watcher);
            }


            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            FileSystemWatcher actualWatcher = (FileSystemWatcher)source;

            if (e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed)
            {
                watchers.Remove(actualWatcher);
            }

            string path = actualWatcher.Path + @"\" + "archive" +  DateTime.Now.ToShortTimeString();
            CreateDirectory(path);

            string startPath = actualWatcher.Path;
            string zipPath = path + @"\" + DateTime.Now.ToShortTimeString() + "result.zip";


            ZipFile.CreateFromDirectory(startPath, zipPath);

            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            int counter = 1;
            FileSystemWatcher actualWatcher = (FileSystemWatcher)source;
            string path = actualWatcher.Path + @"\" + "archive" + counter.ToString();
            CreateDirectory(path);

            string startPath =actualWatcher.Path;
            string zipPath = path + @"\" + "resultAt" + counter.ToString() + ".zip" ;
            

            ZipFile.CreateFromDirectory(startPath, zipPath);

            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);

            counter++;
        }

        public static void CreateDirectory(string path){
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    //Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }

}

