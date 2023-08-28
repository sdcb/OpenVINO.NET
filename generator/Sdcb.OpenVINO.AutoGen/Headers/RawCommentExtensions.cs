using CppSharp.AST;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public static class RawCommentExtensions
{
    public static string[] ToSummaryCode(this RawComment? comment)
    {
        string[]? lines = comment?.FullComment.Blocks
            .OfType<ParagraphComment>()
            .Select(x => x.Content.OfType<TextComment>()
                .Select(x => x.Text.Trim())
                .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}"))
            .ToArray();

        if (lines == null)
        {
            return Array.Empty<string>();
        }
        else if (lines.Length == 1)
        {
            return new string[] { $"/// <summary>{lines[0]}</summary>" };
        }
        else
        {
            return new string[] { "/// <summary>" }
                .Concat(lines.Select(x => $"/// <para>{x}</para>"))
                .Concat(new string[] { "/// </summary>" })
                .ToArray();
        }
    }

    public static string[] ToBriefCode(this RawComment? comment)
    {
        string? brief = comment?.BriefText;

        if (brief != null)
        {
            return new string[] { $"/// <summary>{brief}</summary>" };
        }
        else
        {
            return Array.Empty<string>();
        }
    }
}
