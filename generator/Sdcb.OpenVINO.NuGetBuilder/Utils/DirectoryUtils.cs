namespace Sdcb.OpenVINO.NuGetBuilder.Utils;

public static class DirectoryUtils
{
    public static FileInfo SearchFileInCurrentAndParentDirectories(DirectoryInfo currentFolder, string fileName)
    {
        Stack<DirectoryInfo> stack = new();
        stack.Push(currentFolder);

        while (stack.Count > 0)
        {
            DirectoryInfo dir = stack.Pop();
            FileInfo r = new FileInfo(Path.Combine(dir.FullName, fileName));
            if (r.Exists)
            {
                return r;
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
