using System.IO;
using System;
using System.Text.RegularExpressions;

public class FSIOHelper 
{

    private static string currentDirectory = string.Empty;

    public static string GetCurrentDirectory()
    {
        if (string.IsNullOrEmpty(currentDirectory))
        {
#if UNITY_EDITOR
            currentDirectory = Environment.CurrentDirectory;
#elif UNITY_STANDALONE
			string fullFileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase ;
			int lastIndex = fullFileName.LastIndexOf("/");
			currentDirectory = fullFileName.Substring(8, lastIndex - 8);
			
			DirectoryInfo dir = Directory.GetParent(currentDirectory).Parent;
			currentDirectory = dir.FullName;
#else
			string fullFileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase ;
            int lastIndex = fullFileName.LastIndexOf("/");
            currentDirectory = fullFileName.Substring(8, lastIndex - 8); 
#endif
        }

        return currentDirectory;
    }

    public static string GetAbsolutePath(string path)
    {
        Regex reg = new Regex(@"^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)");
        if (!reg.IsMatch(path))
            path = Path.Combine(FSIOHelper.GetCurrentDirectory(), path);
        return path;
    }

    public static bool IsAbsolutePath(string path)
    {
        Regex reg = new Regex(@"^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)");
        if (!reg.IsMatch(path))
            return false;
        return true;
    }

    public static bool IsHaveSuffix(string absolutePath,string[] suffix)
    {
        string fileName = Path.GetFileName(absolutePath);
        for (int i = 0; i < suffix.Length; i++)
        {
            if (fileName.Contains(suffix[i]))
                return true;
        }

        return false;
    }
   
}
