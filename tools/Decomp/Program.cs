using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

// usage: Decomp <assembly> <outDir> <gameDir> <namespace prefix> [more prefixes...]
if (args.Length < 4)
{
    Console.WriteLine("usage: Decomp <assembly> <outDir> <gameDir> <ns-prefix> [ns-prefix...]");
    return 1;
}

string asmPath = Path.GetFullPath(args[0]);
string outDir = Path.GetFullPath(args[1]);
string gameDir = Path.GetFullPath(args[2]);
string[] prefixes = args.Skip(3).ToArray();

var resolver = new UniversalAssemblyResolver(asmPath, throwOnError: false, targetFramework: "net9.0");
resolver.AddSearchDirectory(gameDir);
resolver.AddSearchDirectory(Path.GetDirectoryName(asmPath)!);

var settings = new DecompilerSettings
{
    ThrowOnAssemblyResolveErrors = false,
    UseSdkStyleProjectFormat = true,
};

var decompiler = new CSharpDecompiler(asmPath, resolver, settings);
var types = decompiler.TypeSystem.MainModule.TypeDefinitions
    .Where(t => t.Name.IndexOf('<') < 0 && !t.Name.StartsWith("$"))
    .Where(t => prefixes.Any(p => t.Namespace == p || t.Namespace.StartsWith(p + ".", StringComparison.Ordinal)))
    .ToList();

Console.WriteLine($"types to decompile: {types.Count}");
int ok = 0, fail = 0;
foreach (var t in types)
{
    try
    {
        string src = decompiler.DecompileTypeAsString(new FullTypeName(t.FullName));
        string dir = Path.Combine(outDir, t.Namespace.Replace('.', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, t.Name + ".cs"), src);
        ok++;
        if (ok % 200 == 0) Console.WriteLine($"  {ok} done");
    }
    catch (Exception ex)
    {
        fail++;
        if (fail <= 5) Console.WriteLine($"  FAIL {t.FullName}: {ex.GetType().Name}: {ex.Message}");
    }
}
Console.WriteLine($"decompiled {ok}, failed {fail}");
return 0;