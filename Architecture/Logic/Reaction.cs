namespace Agents.Architecture.Logic
{
    public class Instruction : List<object>
    {
    }

    public class Reaction
    {
        public List<Instruction> Instructions = new();

        public Reaction(List<Instruction> instructions) =>
            Instructions = instructions;
    }
}
