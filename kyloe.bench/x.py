

expr = "1 + 2 * 3 % 6 / 8 (4%5) & 1"

args = "a: i64, b: i64, c: i64, d: i64, e: i64"
body = """{
    var temp = a * b * 3;
    
    if temp == 0 {
        return 0;
    } elif temp == 36 {
        println('36');
    }

    var res = e * (c - d) / temp;
    return res;
}"""

with open("LargeCode.cs", "w") as file:

    file.write("namespace Kyloe.Benchmarks {\n")
    file.write("    internal static class LargeCode {\n")
    file.write("        public static readonly string code = @\"")
    for i in range(0, 1000):
        file.write(f"var g{i} = {expr};\n")

    for i in range(0, 1000):
        file.write(f"func f{i}({args}) -> i64 {body}\n")
    file.write("\";\n")
    file.write("    }\n")
    file.write("}\n")


