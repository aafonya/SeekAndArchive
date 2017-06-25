using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SeakAndArchive
{
    class Program
    {
        static List<FileInfo> FoundFiles;

        static void Main(string[] args)
        {
            string fileName = args[0];
            string directoryName = args[1];
            FoundFiles = new List<FileInfo>();

            //examine if the given directory exists at all 
            DirectoryInfo rootDir = new DirectoryInfo(directoryName);
            if (!rootDir.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            } else
            {
                //search recursively for the mathing files
                RecursiveSearch(FoundFiles, fileName, rootDir);
                //list the found files
                Console.WriteLine("Found {0} files.", FoundFiles.Count);
                foreach (FileInfo fil in FoundFiles)
                {
                    Console.WriteLine("{0}", fil.FullName);
                }
                Console.ReadKey();
            }


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

        
           
    }
}
