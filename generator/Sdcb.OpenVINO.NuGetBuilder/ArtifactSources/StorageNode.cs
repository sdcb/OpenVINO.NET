namespace Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

public record StorageNode
{
    public string Name { get; }

    public DateTime LastModified { get; }

    public long Size { get; }

    public FileTreeType Type { get; }

    public StorageNode[]? Children { get; }

    public override string ToString() => FullPath;

    public string FullPath { get; }

    public StorageNode? Parent { get; }

    public StorageNode(StorageNodeRaw raw, StorageNode? parent = null)
    {
        Name = raw.Name;
        LastModified = DateTime.Parse(raw.LastModified);
        Size = raw.Size;
        Type = raw.Type;
        Parent = parent;

        // Compute the FullPath
        FullPath = parent == null ? StorageNodeRoot.BaseUrl : parent.FullPath + "/" + Name;

        // Recursively create child StorageNode objects
        if (raw.Children != null)
        {
            Children = raw.Children
                .Select(childRaw => new StorageNode(childRaw, this)).ToArray();
        }
    }

    private IEnumerable<StorageNode> EnumerateItems(string targetPath, FileTreeType itemType, SearchOption searchOption)
    {
        if (Children == null) return Enumerable.Empty<StorageNode>();
        targetPath = targetPath.Trim('/');

        if (targetPath == "")
        {
            return searchOption switch
            {
                SearchOption.TopDirectoryOnly => Children.Where(x => x.Type == itemType),
                SearchOption.AllDirectories => Children.Where(x => x.Type == itemType)
                                                       .Concat(Children.Where(x => x.Type == FileTreeType.Directory)
                                                       .SelectMany(x => x.EnumerateItems(targetPath, itemType, searchOption))),
                _ => throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null),
            };
        }
        else
        {
            Dictionary<string, StorageNode> children = Children.ToDictionary(k => k.Name, v => v);
            string[] segments = targetPath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            if (children.TryGetValue(segments[0], out StorageNode? item))
            {
                return item.EnumerateItems(segments.Length > 1 ? segments[1] : "", itemType, searchOption);
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {targetPath} not found in {Name}.");
            }
        }
    }

    public IEnumerable<StorageNode> EnumerateDirectories(string targetPath = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateItems(targetPath, FileTreeType.Directory, searchOption);
    }

    public IEnumerable<StorageNode> EnumerateFiles(string targetPath = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateItems(targetPath, FileTreeType.File, searchOption);
    }
}
