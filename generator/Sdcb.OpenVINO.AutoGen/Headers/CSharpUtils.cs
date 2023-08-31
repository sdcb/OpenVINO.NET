using CppSharp.AST;
using Csa = CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers;

internal static class CSharpUtils
{
    public static string CSharpKeywordTransform(string syntax) => syntax switch
    {
        _ when _csharpKeywords.Contains(syntax) => "@" + syntax,
        _ => syntax
    };

    private static readonly HashSet<string> _csharpKeywords = ("abstract,as,base,bool,break,byte,case," +
                "catch,char,checked,class,const,continue,decimal,default,delegate,do," +
                "double,else,enum,event,explicit,extern,false,finally,fixed,float,for," +
                "foreach,goto,if,implicit,in,int,interface,internal,is,lock,long,namespace," +
                "new,null,object,operator,out,override,params,private,protected,public," +
                "readonly,ref,return,sbyte,sealed,short,sizeof,stackalloc,static,string," +
                "struct,switch,this,throw,true,try,typeof,uint,ulong,unchecked,unsafe," +
                "ushort,using,virtual,void,volatile,while").Split(',').ToHashSet();

    public static string TypeTransform(Csa.Type type)
    {
        return type switch
        {
            PointerType p => p.Pointee switch
            {
                var x when x is AttributedType attr && attr.Modified.Type is FunctionType func => TypeTransform(func),
                _ => $"{TypeTransform(p.Pointee)}*"
            },
            BuiltinType bi => PrimitiveTypeToCSharp(bi.Type),
            TypedefType tdef => tdef.Declaration.Name switch
            {
                "size_t" => "nint", 
                "wchar_t" => "char",
                _ => TypeTransform(tdef.Declaration.Type),
            },
            TagType tag => tag.Declaration.Name,
            AttributedType attr => TypeTransform(attr.Modified.Type),
            FunctionType func => $"delegate*<{string.Join(",", func.Parameters.Select(p => TypeTransform(p.Type)))}, {TypeTransform(func.ReturnType.Type)}>",
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
