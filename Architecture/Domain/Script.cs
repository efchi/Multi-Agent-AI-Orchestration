namespace Agents.Architecture.Domain
{
    public class Script
    {
        readonly string Meta;                                   // Meta Rules. 
        readonly string Prologue;                               // Process Rules: Start.
        readonly string Epilogue;                               // Process Rules: End.
        readonly Dictionary<Role, string> RoleRules = new();    // Role Rules: Role A, Role B, etc.

        public Script(string meta, string prologue, string epilogue) =>
            (Meta, Prologue, Epilogue) = (meta, prologue, epilogue);

        internal string AddRole(Role role, string roleRules)
        {
            RoleRules.Add(role, roleRules);

            var roleScript = $"{Meta}\n\n{Prologue}\n\n{RoleRules[role]}\n\n{Epilogue}";
            return roleScript;
        }
    }
}
