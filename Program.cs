using System;
using System.IO;

namespace Task3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the path to the Folder: ");
            string folderPath = Console.ReadLine();
            long size = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    size = CheckSize(folderPath);
                }
                else
                {
                    Console.WriteLine($"Folder TestFolder not found");
                }
            }
            catch (Exception ex)
            { Console.WriteLine($"Error: {ex.Message}"); }

            long freedSize = 0;
            int filesDeleted = 0;

            CleanFolder(folderPath, ref filesDeleted, ref freedSize);
            
            long newsize = 0;
            try
            {
                    newsize = CheckSize(folderPath);
                   
            }
            catch (Exception ex)
            { Console.WriteLine($"Error: {ex.Message}"); }

            Console.WriteLine();
            Console.WriteLine($"Folder {folderPath} exists, size: {size} bites");
            Console.WriteLine($"Folder {folderPath} exists, new size: {newsize} bites");
            Console.WriteLine($"Number of files deleted: {filesDeleted}");
            Console.WriteLine($"Space freed: {freedSize} bytes");

        }
        public static long CheckSize(string folderPath)
        {
            long size = 0;
            try
            {
                // Получить все файлы в папке и суммировать их размеры
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    Console.WriteLine($"{file}");
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        Console.WriteLine(fi.Length);
                        size += fi.Length;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Can not get size of {file}: {ex.Message}");
                    }
                }

                // Получить все вложенные папки и рекурсивно суммировать их размеры
                string[] directories = Directory.GetDirectories(folderPath);
                foreach (string directory in directories)
                {
                    size += CheckSize(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with procceding directory {folderPath}: {ex.Message}");
            }

            return size;
        }

        public static void CleanFolder(string folderPath, ref int filesDeleted, ref long freedSize)
        {
            DateTime unusedTime = DateTime.Now - TimeSpan.FromMinutes(30);
            string[] filePaths = Directory.GetFiles(folderPath);

            foreach (string filePath in filePaths)
            {
                try
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                    Console.WriteLine($"File: {filePath}, Last Access Time: {lastWriteTime}");
                    if (lastWriteTime < unusedTime)
                    {
                        Console.WriteLine($"Deleting file: {filePath}");
                        FileInfo fi = new FileInfo(filePath);
                        freedSize += fi.Length;
                        File.Delete(filePath);
                        Console.WriteLine();
                        filesDeleted++;
                    }
                    else
                    {
                        Console.WriteLine($"{filePath} was used within 30 mins.");
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete file: {filePath}. Error: {ex.Message}");
                }
            }

            string[] directoryPaths = Directory.GetDirectories(folderPath);

            foreach (string directoryPath in directoryPaths)
            {
                try
                {
                   CleanFolder(directoryPath, ref filesDeleted, ref freedSize);
                    Console.WriteLine($"Folder: {directoryPath}, last access time: {Directory.GetLastWriteTime(directoryPath)}");
                    if (Directory.GetFiles(directoryPath).Length == 0 && Directory.GetDirectories(directoryPath).Length == 0) // время в папке все равно считает с последнего доступа к папке, а не факту изменения 
                   {
                        DateTime lastWriteTime = Directory.GetLastWriteTime(directoryPath);
                        
                       if (lastWriteTime < unusedTime)
                       {
                           Console.WriteLine($"Deleting directory: {directoryPath}");
                           Directory.Delete(directoryPath, true);
                           Console.WriteLine();
                       }
                       else
                       {
                           CleanFolder(directoryPath, ref filesDeleted, ref freedSize);
                       }
                   }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete directory: {directoryPath}. Error: {ex.Message}");
                }
            }
        }
    }
}


