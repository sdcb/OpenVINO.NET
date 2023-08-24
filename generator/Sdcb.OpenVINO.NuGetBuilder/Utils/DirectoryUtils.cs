namespace Sdcb.OpenVINO.NuGetBuilder.Utils;

public static class DirectoryUtils
{
    public static string SearchFileByParents(DirectoryInfo currentFolder, string fileName)
    {
        Stack<DirectoryInfo> stack = new();
        stack.Push(currentFolder);

        while (stack.Count > 0)
        {
            DirectoryInfo dir = stack.Pop();
            string path = Path.Combine(dir.FullName, fileName);
            if (File.Exists(path))
            {
                return path;
            }

            DirectoryInfo? parent = dir.Parent;
            if (parent != null)
            {
                stack.Push(parent);
            }
        }

        throw new FileNotFoundException($"{fileName} can't found in {currentFolder.FullName}.");
    }
}
