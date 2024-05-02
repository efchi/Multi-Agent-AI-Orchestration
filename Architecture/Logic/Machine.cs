using System.Collections.Concurrent;
using System.Dynamic;
using Agents.Architecture.Domain;
using Agents.Architecture.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agents.Architecture.Logic
{
    public class Machine
    {
        public readonly Process Process;
        public ConcurrentDictionary<int, Context> Contexts = new();

        public Machine(Process process) =>
            Process = process;

        public void Run(Agent sender, int? contextId, string reaction)
        {
            var reactionObject = JsonConvert.DeserializeObject<Reaction>(reaction);
            var instructions = reactionObject!.Instructions;

            foreach (var instruction in instructions)
            {
                if (instruction.Count < 1)
                    continue; // No-op.

                var opCode = instruction[0] as string ?? 
                    throw new SyntaxException($"VM :: OpCode is not a JSON string ({instruction[0].GetType().Name})");
                
                switch (opCode)
                {
                    case "open": Open(); break;
                    case "close": Close(); break;
                    case "set": Set(instruction); break;
                    case "forward": Forward(instruction); break;
                    case "pop": Pop(instruction); break;
                    case "complete": Complete(instruction); break;

                    default: 
                        throw new SyntaxException($"VM :: Unknown Opcode ({opCode})");
                }
            }
            
            void Open()
            {
                int newContextId;
                do { newContextId = Common.NewContextId(); }
                while (Contexts.ContainsKey(newContextId));

                Contexts[newContextId] = new Context();
                contextId = newContextId;
                
                Output.WriteDebugLine($"VM :: Open :: New context opened with ID {contextId}");
            }

            void Close()
            {
                if (!contextId.HasValue)
                    throw new ContextException("VM :: Close :: No context was opened");

                if (!Contexts.ContainsKey(contextId.Value))
                    throw new ConcurrencyException($"VM :: Close :: Context {contextId} was already closed");

                Contexts.Remove(contextId.Value, out _);

                // It's important to set contextId to null after removing it from the dictionary,
                // because there can be other instructions in the same reaction, trying to access the
                // deleted context with the same Id.
                contextId = null; 

                Output.WriteDebugLine($"VM :: Close :: Context {contextId} closed.");
            }

            void Set(Instruction instruction)
            {
                if (!contextId.HasValue)
                    throw new ContextException("VM :: Set :: No context was opened");

                if (!Contexts.ContainsKey(contextId.Value))
                    throw new ConcurrencyException($"VM :: Set :: Context {contextId} was already closed");

                if (instruction.Count < 2)
                    throw new SyntaxException("VM :: Set :: Invalid Syntax. Missing field name");

                if (instruction.Count < 3)
                    throw new SyntaxException("VM :: Set :: Invalid Syntax. Missing value");

                var context = Contexts[contextId.Value];
                var fieldName = instruction[1] as string ?? throw new SyntaxException($"VM :: Set :: Target field is not a JSON string ({instruction[1].GetType().Name})");
                var fieldValue = instruction[2];
                context[fieldName] = fieldValue;

                Output.WriteDebugLine($"VM :: Set :: Context[{contextId}].{fieldName} = {JsonConvert.SerializeObject(fieldValue)}");
            }

            void Forward(Instruction instruction)
            {
                if (instruction.Count < 2)
                    throw new SyntaxException("VM :: Set :: Invalid Syntax. Missing target role");

                // Selecting the proper role to forward the message to.
                var targetRoleName = instruction[1] as string ?? throw new SyntaxException($"VM :: Forward :: Taregt role is not a JSON string ({instruction[1].GetType().Name})");
                var targetRole = Process.GetRole(targetRoleName);
                var targetRoleQueue = targetRole.Queue;

                // Extracting the parameters to be sent.
                var data = instruction.Count > 2 ? instruction[2] : null;

                // Selecting the proper context to attach to the message.
                ConcurrentDictionary<string, object>? state = contextId.HasValue ?
                    (Contexts.ContainsKey(contextId.Value) ? Contexts[contextId.Value] : throw new ConcurrencyException($"VM :: Forward :: Context {contextId} was already closed")) : null;

                // Building the message content (state, data).
                dynamic forwardedContent = new ExpandoObject();
                if (state != null)
                    forwardedContent.state = state;
                if (data != null)
                    forwardedContent.data = data;

                // Sending the message.
                var forwardContentString = JsonConvert.SerializeObject(forwardedContent);
                var forwardedMessage = new Message(contextId, forwardContentString);
                targetRoleQueue.Enqueue(forwardedMessage);

                Output.WriteDebugLine($"VM :: Forward :: Message sent from Agent {sender.Name} to Role {targetRoleName}");
            }

            void Pop(Instruction instruction)
            {
                if (!contextId.HasValue)
                    throw new ContextException("VM :: Pop :: No context was opened");

                if (!Contexts.ContainsKey(contextId.Value))
                    throw new ConcurrencyException($"VM :: Pop :: Context {contextId} was already closed");

                if (instruction.Count < 2)
                    throw new SyntaxException("VM :: Pop :: Invalid Syntax. Missing field name");

                var context = Contexts[contextId.Value];
                var fieldName = instruction[1] as string ?? throw new SyntaxException($"VM :: Pop :: Target field is not a JSON string ({instruction[1].GetType().Name})");
                var fieldValue = context[fieldName];
                var array = fieldValue as JArray ?? throw new SyntaxException($"VM :: Pop :: {fieldName} is not a JSON array ({fieldValue.GetType().Name})");
                array.RemoveAt(array.Count - 1);
                
                Output.WriteDebugLine($"VM :: Pop :: Context[{contextId}].{fieldName} = {JsonConvert.SerializeObject(array)}");
            }

            void Complete(Instruction instruction)
            {
                if (instruction.Count < 2)
                    throw new SyntaxException("VM :: Complete :: Invalid Syntax. Missing result");

                var result = instruction[1];

                Output.WriteLine($"VM :: Workflow completed with result: {JsonConvert.SerializeObject(result)}", 
                    ConsoleColor.Black, ConsoleColor.Green);
            }
        }
    }
}
