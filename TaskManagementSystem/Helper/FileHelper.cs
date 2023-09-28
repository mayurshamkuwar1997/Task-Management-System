namespace TaskManagementSystem.Helper
{
    public static class FileHelper
    {
        public static void SaveFile(string data, string path)
        {
            //Save json file 
            using (StreamWriter sw = new StreamWriter(@path))
            {
                sw.WriteLine(data);
            }
        }

        public static string GetFile(string path)
        {
            //Read json file
            return File.ReadAllText(@path);
        }
    }
}
