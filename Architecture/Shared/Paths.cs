using Agents.Architecture.Domain;
using System.Reflection;

namespace Agents.Architecture.Shared
{
    public static class Paths
    {
        public static readonly string DataPath =
            Path.Combine(
                Directory.GetParent(                            // sln
                Directory.GetParent(                            // bin
                Directory.GetParent(                            // Debug
                Directory.GetParent(                            // net6.0
                    Assembly.GetEntryAssembly()!.Location       // .dll
                )!.FullName)!.FullName)!.FullName)!.FullName,
                ".data");

        public static string ReadMetaRules() =>
            File.ReadAllText(Path.Combine(DataPath, ".0.meta.txt"));

        public static string ReadPrologueRules() =>
            File.ReadAllText(Path.Combine(DataPath, ".1.prologue.txt"));

        public static string ReadEpilogueRules() =>
            File.ReadAllText(Path.Combine(DataPath, ".2.epilogue.txt"));

        public static string ReadRoleRules(Role role) =>
            File.ReadAllText(Path.Combine(DataPath, $".role.{role.Name}.txt"));
    }
}
