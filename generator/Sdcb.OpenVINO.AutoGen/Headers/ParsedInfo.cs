using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public record ParsedInfo(HashSet<string> Symbols, TranslationUnit[] Units);