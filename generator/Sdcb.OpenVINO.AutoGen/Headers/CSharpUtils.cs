using CppSharp.AST;
using Csa = CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers;

internal static class CSharpUtils
{
    public static string TypeTransform(Csa.Type type)
    {
        return type switch
        {
            PointerType p => $"{TypeTransform(p.Pointee)}*",
            BuiltinType bi => PrimitiveTypeToCSharp(bi.Type),
            TypedefType tdef => TypeTransform(tdef.Declaration.Type),
            TagType tag => tag.Declaration.Name,
            _ => type.ToString(),
        };
    }

    public static string PrimitiveTypeToCSharp(PrimitiveType type)
    {
        // help me implement this
        return type switch
        {
            PrimitiveType.Bool => "bool",
            PrimitiveType.Char => "byte",
            PrimitiveType.Double => "double",
            PrimitiveType.Float => "float",
            PrimitiveType.Int => "int",
            PrimitiveType.Long => "int",
            PrimitiveType.LongDouble => "double",
            PrimitiveType.LongLong => "long",
            PrimitiveType.Short => "short",
            PrimitiveType.SChar => "sbyte",
            PrimitiveType.UChar => "byte",
            PrimitiveType.UInt => "uint",
            PrimitiveType.ULong => "uint",
            PrimitiveType.ULongLong => "ulong",
            PrimitiveType.UShort => "ushort",
            PrimitiveType.Void => "void",
            _ => throw new NotImplementedException(),
        };
    }
}
