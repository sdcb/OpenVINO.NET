using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

internal static class InferenceYmlUtils
{
    public static IReadOnlyList<string> LoadCharacterDict(string ymlPath)
    {
        return LoadList(ymlPath, "  character_dict:");
    }

    public static IReadOnlyList<string> LoadLabelList(string ymlPath)
    {
        return LoadList(ymlPath, "    label_list:");
    }

    private static IReadOnlyList<string> LoadList(string ymlPath, string keyLine)
    {
        List<string> labels = new();
        bool inList = false;
        foreach (string line in File.ReadLines(ymlPath))
        {
            if (line == keyLine)
            {
                inList = true;
                continue;
            }

            if (!inList)
            {
                continue;
            }

            if (line.StartsWith("  - ", StringComparison.Ordinal))
            {
                labels.Add(ParseYamlScalar(line.Substring(4)));
            }
            else if (line.StartsWith("    - ", StringComparison.Ordinal))
            {
                labels.Add(ParseYamlScalar(line.Substring(6)));
            }
            else if (line.Length > 0 && !char.IsWhiteSpace(line[0]))
            {
                break;
            }
        }

        if (labels.Count == 0)
        {
            throw new InvalidOperationException($"Unable to find YAML list '{keyLine.Trim()}' in {ymlPath}.");
        }

        return labels;
    }

    private static string ParseYamlScalar(string value)
    {
        if (value == "''")
        {
            return string.Empty;
        }

        if (value.Length >= 2 && value[0] == '\'' && value[value.Length - 1] == '\'')
        {
            return value.Substring(1, value.Length - 2).Replace("''", "'");
        }

        return value;
    }
}
