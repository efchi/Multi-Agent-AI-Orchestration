using Agents.Architecture.Logic;
using Agents.Architecture.Shared;

namespace Agents.Architecture.Domain
{
    public class Process
    {        
        public readonly Script Script;
        public readonly Machine Machine;
        public readonly IList<Role> Roles = new List<Role>();
        public readonly IList<Agent> Agents = new List<Agent>();

        public Process(Script script) =>
            (Script, Machine) = (script, new(this));

        public bool RoleExists(string roleName) =>
            Roles.Any(role => role.Name == roleName);

        public Role GetRole(string roleName) =>
            Roles.Single(role => role.Name == roleName);

        public void AddRole(Role role, string roleRules)
        {
            if (RoleExists(role.Name))
                throw new DomainException($"Process :: AddRole :: Role {role.Name} already exists");

            var roleScript = Script.AddRole(role, roleRules);
            role.SetScript(roleScript);
            Roles.Add(role);
        }

        public void AddAgent(Agent agent) =>
            Agents.Add(agent);

        public void RemoveAgent(Agent agent) =>
            Agents.Remove(agent);

        // Start all agents in the process.
        public void Start()
        {
            foreach (var agent in Agents)
                agent.Start();
        }
    }
}
