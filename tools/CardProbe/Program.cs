using Mono.Cecil;
using Mono.Cecil.Cil;

// usage:
//   CardProbe <sts2.dll> <gameDir>                -> rarity/cost/pool matrix
//   CardProbe <sts2.dll> <gameDir> --type:Bash    -> dump IL of one card
//   CardProbe <sts2.dll> <gameDir> --pool:Ironclad
var asmPath = Path.GetFullPath(args[0]);
var gameDir = Path.GetFullPath(args[1]);

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(gameDir);
using var asm = AssemblyDefinition.ReadAssembly(asmPath, new ReaderParameters { AssemblyResolver = resolver });
var module = asm.MainModule;

var cardModel = module.GetType("MegaCrit.Sts2.Core.Models.CardModel");
var poolModel = module.GetType("MegaCrit.Sts2.Core.Models.CardPoolModel");
if (cardModel is null || poolModel is null) { Console.WriteLine("types not found"); return 1; }

var baseCtor = cardModel.Methods.First(m => m.IsConstructor && m.Parameters.Count == 5);
var pCost = baseCtor.Parameters[0];   // canonicalEnergyCost
var pType = baseCtor.Parameters[1];   // CardType
var pRar = baseCtor.Parameters[2];    // CardRarity

if (args.Length > 2 && args[2].StartsWith("--il:"))
{
    var parts = args[2].Substring("--il:".Length).Split(':');
    var t = AllTypes(module).FirstOrDefault(x => x.Name == parts[0]);
    var meth = t?.Methods.FirstOrDefault(m => m.Name == parts[1]);
    if (meth?.Body is null) { Console.WriteLine("method not found"); return 2; }
    foreach (var ins in meth.Body.Instructions)
        Console.WriteLine($"  {ins.OpCode,-12} {FormatOperand(ins.Operand)}");
    return 0;
}

if (args.Length > 2 && args[2].StartsWith("--type:"))
{
    var wanted = args[2].Substring("--type:".Length);
    var t = AllTypes(module).FirstOrDefault(x => x.Name == wanted);
    if (t is null) { Console.WriteLine("type not found"); return 2; }
    Console.WriteLine($"type {t.FullName} : {t.BaseType?.FullName}");
    foreach (var a in t.CustomAttributes)
        Console.WriteLine($"  [{a.AttributeType.Name}]");
    Console.WriteLine("  methods:");
    foreach (var m in t.Methods)
        Console.WriteLine($"    {m.ReturnType.Name} {m.Name}({string.Join(", ", m.Parameters.Select(p => p.ParameterType.Name))})");
    foreach (var c in t.Methods.Where(x => x.IsConstructor))
    {
        var body = c.Body;
        if (body is null) continue;
        foreach (var ins in body.Instructions)
            Console.WriteLine($"  {ins.OpCode,-12} {FormatOperand(ins.Operand)}");
    }

    var genMethod = t.Methods.FirstOrDefault(m => m.Name == "GenerateAllCards");
    Console.WriteLine($"\n  --- GenerateAllCards body ---");
    if (genMethod?.Body is null) { Console.WriteLine("    (no body)"); }
    else foreach (var ins in genMethod.Body.Instructions)
        Console.WriteLine($"    {ins.OpCode,-12} {FormatOperand(ins.Operand)}");

    return 0;
}

// ---- card metadata from constructor constants ----
var cards = new List<(string Name, int Rarity, int Cost, int Type, string Pool)>();
var typeByName = new Dictionary<string, TypeDefinition>(StringComparer.Ordinal);
var poolOf = new Dictionary<string, string>(StringComparer.Ordinal);

foreach (var pt in AllTypes(module).Where(t => t.IsClass && !t.IsAbstract && DerivesFrom(t, poolModel)))
{
    var gen = pt.Methods.FirstOrDefault(m => m.Name == "GenerateAllCards");
    if (gen?.Body is null) continue;
    foreach (var ins in gen.Body.Instructions)
    {
        var cardTypeName = CardTypeOf(ins);
        if (cardTypeName is not null && !poolOf.ContainsKey(cardTypeName))
            poolOf[cardTypeName] = pt.Name;
    }
}

foreach (var t in AllTypes(module))
{
    if (!t.IsClass || t.IsAbstract || !DerivesFrom(t, cardModel)) continue;
    var caps = ReadCtorConstants(t, cardModel);
    if (caps.Count < 3) continue;
    cards.Add((t.Name, caps[2], caps[0], caps[1], poolOf.GetValueOrDefault(t.Name, "(none)")));
    typeByName[t.Name] = t;
}

string[] rarities = { "None", "Basic", "Common", "Uncommon", "Rare", "Ancient", "Event", "Token", "Status", "Curse", "Quest" };
string[] types = { "?", "Attack", "Skill", "Power", "Status?", "Curse?", "Quest?" };

Console.WriteLine($"cards parsed: {cards.Count}  (pooled: {cards.Count(c => c.Pool != "(none)")})\n");

foreach (var poolGroup in cards.GroupBy(c => c.Pool).OrderBy(g => g.Key))
{
    Console.WriteLine($"## {poolGroup.Key}  ({poolGroup.Count()})");
    var byRar = poolGroup.GroupBy(c => c.Rarity).OrderBy(g => g.Key);
    foreach (var rg in byRar)
    {
        var rn = rg.Key >= 0 && rg.Key < rarities.Length ? rarities[rg.Key] : "?";
        var costs = rg.GroupBy(c => c.Cost).OrderBy(g => g.Key)
                      .Select(g => $"c{g.Key}={g.Count()}");
        Console.WriteLine($"   {rn,-9} {string.Join("  ", costs)}   [{rg.Count()}]");
    }
    Console.WriteLine();
}

Console.WriteLine("## rarity x cost, whole game (excl. status/curse/quest/token)");
var playable = cards.Where(c => c.Rarity is >= 1 and <= 5).ToList();
Console.WriteLine("        " + string.Join("  ", Enumerable.Range(0, 5).Select(i => $"cost{i}")) + "  (X/var)");
foreach (var r in Enumerable.Range(1, 5))
{
    var row = Enumerable.Range(0, 5).Select(cost => playable.Count(c => c.Rarity == r && c.Cost == cost).ToString().PadLeft(3));
    var unknown = playable.Count(c => c.Rarity == r && (c.Cost < 0 || c.Cost > 4));
    Console.WriteLine($"  {rarities[r],-9}" + string.Join("  ", row) + $"   ({unknown})");
}

if (args.Length > 2 && args[2].StartsWith("--pool:"))
{
    var f = args[2].Substring("--pool:".Length);
    Console.WriteLine($"\n## cards in pool {f}");
    foreach (var c in cards.Where(c => c.Pool.Contains(f, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.Rarity).ThenBy(c => c.Cost))
    {
        var gen = typeByName.TryGetValue(c.Name, out var td) ? ReadOwnBool(td, "CanBeGeneratedInCombat") : null;
        var genStr = gen is null ? "def" : (gen.Value ? "Y" : "N");
        Console.WriteLine($"   {rarities[c.Rarity],-9} cost={c.Cost,-3} type={c.Type} gen={genStr,-3} {(c.Cost < 0 ? "(X)" : "")}{c.Name}");
    }
}

return 0;

static IEnumerable<TypeDefinition> AllTypes(ModuleDefinition m) => m.Types.SelectMany(Flatten);

static IEnumerable<TypeDefinition> Flatten(TypeDefinition t)
{
    yield return t;
    foreach (var n in t.NestedTypes) foreach (var x in Flatten(n)) yield return x;
}

static bool DerivesFrom(TypeDefinition t, TypeDefinition baseType)
{
    var cur = t.BaseType;
    while (cur is not null)
    {
        if (cur.FullName == baseType.FullName) return true;
        cur = cur.Resolve()?.BaseType;
    }
    return false;
}

// constants passed to CardModel::.ctor(cost, type, rarity, target, showInLibrary)
static List<int> ReadCtorConstants(TypeDefinition type, TypeDefinition cardModel)
{
    var result = new List<int>();
    foreach (var c in type.Methods.Where(x => x.IsConstructor && x.Parameters.Count == 0))
    {
        if (c.Body is null) continue;
        var ins = c.Body.Instructions;
        for (int i = 0; i < ins.Count; i++)
        {
            if (ins[i].OpCode.Code != Code.Call) continue;
            if (ins[i].Operand is not MethodReference mr) continue;
            if (mr.DeclaringType.FullName != cardModel.FullName || mr.Name != ".ctor") continue;

            var vals = new List<int>();
            for (int j = i - 1; j >= 0 && vals.Count < 5; j--)
            {
                var v = ConstantOf(ins[j]);
                if (v is not null) vals.Insert(0, v.Value);
            }
            if (vals.Count >= 3) return vals;
        }
    }
    return result;
}

static int? ConstantOf(Instruction ins) => ins.OpCode.Code switch
{
    Code.Ldc_I4 => (int)ins.Operand,
    Code.Ldc_I4_S => (sbyte)ins.Operand,
    Code.Ldc_I4_0 => 0,
    Code.Ldc_I4_1 => 1,
    Code.Ldc_I4_2 => 2,
    Code.Ldc_I4_3 => 3,
    Code.Ldc_I4_4 => 4,
    Code.Ldc_I4_5 => 5,
    Code.Ldc_I4_6 => 6,
    Code.Ldc_I4_7 => 7,
    Code.Ldc_I4_8 => 8,
    Code.Ldc_I4_M1 => -1,
    _ => null
};

static string FormatOperand(object? o) => o switch
{
    null => "",
    GenericInstanceMethod gim => gim.ElementMethod.DeclaringType.Name + "::" + gim.ElementMethod.Name + "<" +
                                 string.Join(",", gim.GenericArguments.Select(a => a.Name)) + ">",
    MethodReference mr => mr.DeclaringType.Name + "::" + mr.Name,
    FieldReference fr => fr.DeclaringType.Name + "::" + fr.Name,
    TypeReference tr => tr.Name,
    _ => o.ToString() ?? ""
};

// 只读类型自己声明的重写（继承基类默认返回 null = 用基类默认值）
static bool? ReadOwnBool(TypeDefinition type, string prop)
{
    var p = type.Properties.FirstOrDefault(x => x.Name == prop);
    var m = p?.GetMethod;
    if (m?.Body is null) return null;
    foreach (var ins in m.Body.Instructions)
    {
        if (ins.OpCode.Code == Code.Ldc_I4_0) return false;
        if (ins.OpCode.Code == Code.Ldc_I4_1) return true;
    }
    return null;
}

// GenerateAllCards 里是 ModelDb::Card<T>() 泛型调用，T 就是卡类型
static string? CardTypeOf(Instruction ins)
{
    if (ins.Operand is GenericInstanceMethod gim && gim.ElementMethod.Name == "Card" && gim.GenericArguments.Count == 1)
        return gim.GenericArguments[0].Name;
    if (ins.OpCode.Code == Code.Newobj && ins.Operand is MethodReference mr &&
        mr.DeclaringType.FullName.StartsWith("MegaCrit.Sts2.Core.Models.Cards."))
        return mr.DeclaringType.Name;
    return null;
}