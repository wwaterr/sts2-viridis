using System.Reflection;
using System.Runtime.InteropServices;

// usage: ApiDump <assembly> <resolverDirs> [--has-attr Name] [--all] [nameFilter...]
if (args.Length < 2)
{
    Console.WriteLine("usage: ApiDump <assembly> <resolverDirs> [--has-attr Name] [--all] [filter...]");
    return 1;
}

var target = Path.GetFullPath(args[0]);
var resolverDirs = Path.GetFullPath(args[1]).Split(';', StringSplitOptions.RemoveEmptyEntries);
var filters = args.Skip(2).ToList();

string? hasAttr = null;
var attrIdx = filters.IndexOf("--has-attr");
if (attrIdx >= 0 && attrIdx + 1 < filters.Count)
{
    hasAttr = filters[attrIdx + 1];
    filters.RemoveRange(attrIdx, 2);
}
var showAll = filters.Remove("--all");

var paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
foreach (var dir in resolverDirs)
    foreach (var f in Directory.GetFiles(dir, "*.dll"))
        paths.TryAdd(Path.GetFileName(f), f);
foreach (var f in Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll"))
    paths.TryAdd(Path.GetFileName(f), f);

var resolver = new PathAssemblyResolver(paths.Values);
using var mlc = new MetadataLoadContext(resolver);

Assembly asm;
try
{
    asm = mlc.LoadFromAssemblyPath(target);
}
catch (Exception ex)
{
    Console.WriteLine("load failed: " + ex.Message);
    return 2;
}

Console.WriteLine($"# {asm.GetName().Name} {asm.GetName().Version}");

var types = asm.GetTypes()
    .Where(t => hasAttr is not null
        ? t.GetCustomAttributesData().Any(a => a.AttributeType.Name.Contains(hasAttr, StringComparison.OrdinalIgnoreCase))
        : filters.Count == 0 || filters.Any(f => (t.FullName ?? t.Name).Contains(f, StringComparison.OrdinalIgnoreCase)))
    .Where(t => !t.Name.Contains("<>"))          // skip compiler-generated closures
    .OrderBy(t => t.FullName, StringComparer.Ordinal);

const BindingFlags F = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                       BindingFlags.Static | BindingFlags.DeclaredOnly;

foreach (var t in types)
{
    var kind = t.IsInterface ? "interface" : t.IsEnum ? "enum" : t.IsValueType ? "struct" : t.IsAbstract && t.IsSealed ? "static class" : t.IsAbstract ? "abstract class" : "class";
    var baseType = t.BaseType is null ? "" : " : " + Short(t.BaseType);
    Console.WriteLine($"\n=== {Acc(t)} {kind} {t.FullName}{baseType}");

    foreach (var a in t.GetCustomAttributesData())
        Console.WriteLine($"    [{a.AttributeType.Name}({string.Join(", ", a.ConstructorArguments.Select(c => c.Value))})]");

    foreach (var f in t.GetFields(F))
    {
        if (!showAll && !(f.IsPublic || f.IsFamily || f.IsFamilyOrAssembly)) continue;
        Console.WriteLine($"    {Acc(f)}{(f.IsStatic ? "static " : "")}{Short(f.FieldType)} {f.Name}");
    }

    foreach (var p in t.GetProperties(F).Where(p => p.GetMethod is not null))
    {
        var g = p.GetMethod!;
        if (!showAll && !(g.IsPublic || g.IsFamily || g.IsFamilyOrAssembly)) continue;
        var mods = Mods(p.GetMethod!) + Mods(p.SetMethod);
        Console.WriteLine($"    {Acc(g)}prop {mods}{Short(p.PropertyType)} {p.Name} {{ {(p.CanRead ? "get; " : "")}{(p.CanWrite ? "set; " : "")}}}");
    }

    foreach (var c in t.GetConstructors(F))
    {
        if (!showAll && !(c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly)) continue;
        Console.WriteLine($"    {Acc(c)}ctor {t.Name}({string.Join(", ", c.GetParameters().Select(Param))})");
    }

    foreach (var m in t.GetMethods(F).Where(m => !m.IsSpecialName).OrderBy(m => m.Name).ThenBy(m => m.GetParameters().Length))
    {
        if (!showAll && !(m.IsPublic || m.IsFamily || m.IsFamilyOrAssembly)) continue;
        Console.WriteLine($"    {Acc(m)}{Mods(m)}{Short(m.ReturnType)} {m.Name}({string.Join(", ", m.GetParameters().Select(Param))})");
    }
}

return 0;

static string Param(ParameterInfo p) => $"{Short(p.ParameterType)} {p.Name}{(p.HasDefaultValue ? " = " + (p.RawDefaultValue ?? "null") : "")}";

static string Mods(MethodInfo? m)
{
    if (m is null) return "";
    if (m.IsAbstract) return "abstract ";
    if (m.IsVirtual) return (m.Attributes & MethodAttributes.NewSlot) == 0 ? "override " : "virtual ";
    return "";
}

static string Acc(MemberInfo m)
{
    if (m is MethodBase mb)
    {
        if (mb.IsPublic) return "";
        if (mb.IsFamily) return "protected ";
        if (mb.IsFamilyOrAssembly) return "protected internal ";
        if (mb.IsAssembly) return "internal ";
        return "private ";
    }
    if (m is FieldInfo f)
    {
        if (f.IsPublic) return "";
        if (f.IsFamily) return "protected ";
        if (f.IsAssembly) return "internal ";
        return "private ";
    }
    if (m is Type t)
    {
        if (t.IsNestedPublic || t.IsPublic) return "";
        if (t.IsNestedFamily) return "protected ";
        return "internal ";
    }
    return "";
}

static string Short(Type t)
{
    if (t.IsGenericType)
    {
        var n = t.Name[..t.Name.IndexOf('`')];
        return $"{n}<{string.Join(", ", t.GetGenericArguments().Select(Short))}>";
    }
    return t.Name;
}