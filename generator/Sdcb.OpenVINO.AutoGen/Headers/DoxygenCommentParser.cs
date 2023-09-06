namespace Sdcb.OpenVINO.AutoGen.Headers;

public class DoxygenCommentParser
{
    public static IReadOnlyList<DoxygenCommentTag> Parse(string? comment)
    {
        if (comment == null) return new List<DoxygenCommentTag>();

        var lines = comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var tags = new List<DoxygenCommentTag>();
        DoxygenCommentTag? currentTag = null;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("/**") || trimmedLine == "*/")
            {
                continue;
            }

            if (trimmedLine.StartsWith("* @"))
            {
                if (currentTag != null)
                {
                    // 添加之前的标签
                    tags.Add(currentTag);
                }

                var tagParts = trimmedLine.Substring(3).Split(new[] { ' ' }, 2);

                var name = tagParts[0];

                if (tagParts.Length > 1)
                {
                    var commentAndMaybeValue = tagParts[1];

                    if (name == "param")
                    {
                        var valueAndComment = commentAndMaybeValue.Split(new[] { ' ' }, 2);
                        currentTag = new DoxygenCommentTag(name, valueAndComment[0], valueAndComment[1]);
                    }
                    else
                    {
                        currentTag = new DoxygenCommentTag(name, null, commentAndMaybeValue);
                    }
                }
                else
                {
                    currentTag = new DoxygenCommentTag(name, null, string.Empty);
                }
            }
            else if (trimmedLine.StartsWith("*"))
            {
                if (currentTag != null && trimmedLine.Length > 2)
                {
                    // 对于连续的行，添加到当前标签的评论
                    currentTag = currentTag with { Comment = currentTag.Comment + "\n" + trimmedLine.Substring(3).TrimEnd() };
                }
            }
        }

        if (currentTag != null)
        {
            // 添加最后的标签
            tags.Add(currentTag);
        }

        return tags;
    }
}

public record DoxygenCommentTag(string Name, string? Value, string Comment);

public static class DoxygenCommentTagsExtensions
{
    public static string? GetBrief(this IReadOnlyList<DoxygenCommentTag> tags)
    {
        return tags.FirstOrDefault(x => x.Name == "brief")?.Comment;
    }

    public static string? GetGroup(this IReadOnlyList<DoxygenCommentTag> tags)
    {
        return tags.FirstOrDefault(x => x.Name == "ingroup")?.Comment;
    }
}