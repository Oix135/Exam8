using System.Globalization;

namespace Exam8
{

    internal class FolderWork
    {

        public string folderForClear = @"E:\Oix\Projects\Vent\FolderEx8ForDelete";
        private int minutes;
        private List<Resource> Resources;
        private long sizeBefore;
        private long sizeDeleted;
        private long sizeAfter;
        private DateTime dateCheck;
        public FolderWork()
        {
            Resources = new List<Resource>();
            Start();
        }

        private void Start()
        {
            Console.WriteLine("Укажите путь к папке");
            folderForClear = string.IsNullOrEmpty(Console.ReadLine()) ? folderForClear : Console.ReadLine();
            if (!Directory.Exists(folderForClear))
            {
                Console.WriteLine("Папки не существует!");
                Start();
            }

            Console.WriteLine("Выберите действие: ");
            Console.WriteLine($"Узнать размер папки {folderForClear} 1");
            Console.WriteLine($"Очистить папку      {folderForClear} 2");
            _ = int.TryParse(Console.ReadLine(), out int key);
            switch( key )
            {
                case 1:
                    {
                        sizeBefore = GetSize();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($"Размер папки {ComputeSize(sizeBefore)}");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }
                case 2:
                    {
                        dateCheck = DateTime.Now;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Укажите количество минут, в течении которых ресурсы папки {folderForClear} не использовались.\n" +
                            "Ресурсы будут удалены!");
                        _ = int.TryParse(Console.ReadLine(), out minutes);
                        sizeBefore = GetSize();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        ClearFolder();
                        sizeDeleted = Resources.Sum(a => a.Size);
                        sizeAfter = GetSize();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($"\nРазмер папки до очистки:    {ComputeSize(sizeBefore)}");
                        Console.WriteLine($"Размер папки после очистки:   {ComputeSize(sizeAfter)}");
                        Console.WriteLine($"Удалено файлов:               {Resources.Where(a => a.ResourceType == Type.File).Count()}");
                        Console.WriteLine($"Удалено папок:                {Resources.Where(a => a.ResourceType == Type.Folder).Count()}");
                        Console.WriteLine($"Освобождено:                  {ComputeSize(sizeDeleted)}");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }
            }
        }

        private long GetSize()
        {
            if (!Directory.Exists(folderForClear))
            {
                Console.WriteLine("Папки не существует!");
                return 0;
            }
            var directoryinfo = new DirectoryInfo(folderForClear);
            return directoryinfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length); 
        }

        private string ComputeSize(long size)
        {
            var strSize = size switch
            {
                < 1024 => $"{size} б",
                > 1024 and < 1048576 => $"{size / 1024} кб",
                > 1024 and < 1073741824 => $"{size / 1024 / 1024} мб",
                _ => $"{size / 1024 / 1024 / 1024} гб"
            };
            return strSize;
        }

        private void ClearFolder()
        {
            Resources.Clear();
            if (!Directory.Exists(folderForClear))
            {
                Console.WriteLine("Папки не существует!");
                return;
            }
            DeleteResources(folderForClear);

            if (Resources.Count > 0)
            {
                foreach (var resource in Resources)
                {
                    Console.WriteLine($"{(resource.ResourceType == Type.File ? "Файл" : "Папка")} {resource.Name} " +
                        $"{(resource.ResourceType == Type.File ? "удалён" : "удалена")} " +
                        $"дата использования {resource.LastAccess}");
                }
            }
            else
            {
                Console.WriteLine("Ни один ресурс не удалён!");
            }

        }
        private void DeleteResources(string folder)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(folder);
                var directories = directoryInfo.GetDirectories();
                var fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fi in fileInfos)
                {
                    if (File.GetLastAccessTime(fi.FullName) < dateCheck.AddMinutes(-minutes))
                    {
                        Resources.Add(new Resource { Name = fi.Name, ResourceType = Type.File, Size = fi.Length, 
                            LastAccess  = File.GetLastAccessTime(fi.FullName).ToString("G", CultureInfo.CurrentCulture) });
                        fi.Delete();
                    }
                }
                foreach (DirectoryInfo df in directories)
                {
                    DeleteResources(df.FullName);
                }
                //папка изменяет время поледнего доступа во время опроса...
                //будем считать, что пустые не нужны
                if (directoryInfo.GetDirectories().Length == 0 && 
                    directoryInfo.GetFiles().Length == 0 && 
                    directoryInfo.FullName != folderForClear)
                {
                    Resources.Add(new Resource { Name = directoryInfo.Name, ResourceType = Type.Folder, Size = 0,
                        LastAccess = directoryInfo.LastAccessTime.ToString("G", CultureInfo.CurrentCulture)
                    });
                    directoryInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
