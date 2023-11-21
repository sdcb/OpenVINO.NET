using Sdcb.OpenVINO.AutoGen.Headers.Generators;
using System.Security;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public class DoxygenTags
{
    private readonly Dictionary<string, DoxygenTag[]> _tags;

    public DoxygenTags() { _tags = new(); }
    public DoxygenTags(Dictionary<string, DoxygenTag[]> tags) { _tags = tags; }
    public DoxygenTags(string brief)
    {
        _tags = new Dictionary<string, DoxygenTag[]>()
        {
            ["brief"] = new[] { new DoxygenTag("brief", null, brief) }
        };
    }

    public string? Group => _tags.TryGetValue("ingroup", out DoxygenTag[]? vals) ? vals[0].Comment : null;
    public string? Brief => _tags.TryGetValue("brief", out DoxygenTag[]? vals) ? vals[0].Comment : null;
    public string? Returns => _tags.TryGetValue("return", out DoxygenTag[]? vals) ? vals[0].Comment : null;
    public DoxygenTag[] Params => _tags.TryGetValue("param", out DoxygenTag[]? vals) ? vals : Array.Empty<DoxygenTag>();

    public IEnumerable<string> ToBriefComment() => ToComment(Brief, "summary");
    public IEnumerable<string> ToReturnsComment() => ToComment(Returns, "returns");
    public IEnumerable<string> ToParamsComment(IReadOnlyList<RealFuncParam> funcParams)
    {
        DoxygenTag[] paramTags = Params;
        for (int i = 0; i < paramTags.Length; i++)
        {
            if (funcParams[i].IsVariadic)
            {
                DoxygenTag tag = paramTags[i];
                foreach (string line in ToComment(tag.Comment, "remarks"))
                {
                    yield return line;
                }
            }
            else
            {
                DoxygenTag tag = paramTags[i];
                foreach (string line in ToComment(tag.Comment, $"param name=\"{funcParams[i].NameUnescaped}\"", "param"))
                {
                    yield return line;
                }
            }
        }
    }

    private static IEnumerable<string> ToComment(string? value, string startTagName, string? endTagName = null)
    {
        endTagName ??= startTagName;

        if (value != null)
        {
            string[] lines = value.Split('\n');
            if (lines.Length == 1)
            {
                yield return $"/// <{startTagName}>{SecurityElement.Escape(value)}</{endTagName}>";
            }
            else
            {
                yield return $"/// <{startTagName}>";
                foreach (string line in lines)
                {
                    yield return $"/// <para>{SecurityElement.Escape(line)}</para>";
                }
                yield return $"/// </{endTagName}>";
            }
        }
    }

    public static DoxygenTags Parse(string? comment)
    {
        if (comment == null) return new DoxygenTags();
        if (comment.StartsWith("//!< ")) return new DoxygenTags(comment["//!< ".Length..]);

        string[] lines = comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        List<DoxygenTag> tags = new();
        DoxygenTag? currentTag = null;

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

                var tagParts = trimmedLine[3..].Split(new[] { ' ' }, 2);

                var name = tagParts[0];

                if (tagParts.Length > 1)
                {
                    var commentAndMaybeValue = tagParts[1];

                    if (name == "param")
                    {
                        var valueAndComment = commentAndMaybeValue.Split(new[] { ' ' }, 2);
                        currentTag = new DoxygenTag(name, valueAndComment[0], valueAndComment[1]);
                    }
                    else
                    {
                        currentTag = new DoxygenTag(name, null, commentAndMaybeValue);
                    }
                }
                else
                {
                    currentTag = new DoxygenTag(name, null, string.Empty);
                }
            }
            else if (trimmedLine.StartsWith("*"))
            {
                if (currentTag != null && trimmedLine.Length > 1)
                {
                    // 对于连续的行，添加到当前标签的评论
                    currentTag = currentTag with { Comment = currentTag.Comment + "\n" + trimmedLine[2..].TrimEnd() };
                }
            }
        }

        if (currentTag != null)
        {
            // 添加最后的标签
            tags.Add(currentTag);
        }

        return new(tags.GroupBy(x => x.Name).ToDictionary(k => k.Key, v => v.ToArray()));
    }
}

public record DoxygenTag(string Name, string? Value, string Comment);