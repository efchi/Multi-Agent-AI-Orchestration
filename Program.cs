using Azure.AI.OpenAI;
using Azure;
using Agents.Architecture.Domain;
using Agents.Architecture.Logic;
using Agents.Architecture.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agents
{
    public class Program
    {
        static EventWaitHandle ExitWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        internal static string? OpenAIKey { get; set; }

        public static void Main(string[] args)
        {
            Output.Initialize(ExitWaitHandle);
            Output.WriteLine("Multi-Agent AI Orchestration System: Started.");
            Output.WriteLine("Usage: sum 1 2 3 42");

            while (OpenAIKey == default)
            {
                Output.WriteLine("Please insert your OpenAI api key: ");
                OpenAIKey = Console.ReadLine()?.Trim();
            }

            var organizationName = "\"NaiveSummers\"";
            var goalStatement = "to sum list of numbers";

            var metaRules = Paths.ReadMetaRules();
            var epilogueRules = Paths.ReadEpilogueRules();
            var prologueRules = Paths.ReadPrologueRules()
                .Replace("[[O]]", organizationName)
                .Replace("[[G]]", goalStatement);

            var script = new Script(metaRules, prologueRules, epilogueRules);
            var process = new Process(script);

            var roleA = new Role("A", process);
            var roleB = new Role("B", process);
            var roleC = new Role("C", process);
            var roleD = new Role("D", process);
            process.AddRole(roleA, Paths.ReadRoleRules(roleA));
            process.AddRole(roleB, Paths.ReadRoleRules(roleB));
            process.AddRole(roleC, Paths.ReadRoleRules(roleC));
            process.AddRole(roleD, Paths.ReadRoleRules(roleD));

            // Lambdas for hard-coded agents.
            //var agentA = new HardCodedAgent("A", roleA, process, MockLambdas.RoleA);
            //var agentB = new HardCodedAgent("B", roleB, process, MockLambdas.RoleB);
            //var agentC = new HardCodedAgent("C", roleC, process, MockLambdas.RoleC);
            //var agentD = new HardCodedAgent("D", roleD, process, MockLambdas.RoleD);

            // Lambdas for OpenAI agents.
            var agentA = new LambdaAgent("A", roleA, process, OpenAILambdas.GenericRole);
            var agentB = new LambdaAgent("B", roleB, process, OpenAILambdas.GenericRole);
            var agentC = new LambdaAgent("C", roleC, process, OpenAILambdas.GenericRole);
            var agentD = new LambdaAgent("D", roleD, process, OpenAILambdas.GenericRole);
            
            process.AddAgent(agentA);
            process.AddAgent(agentB);
            process.AddAgent(agentC);
            process.AddAgent(agentD);

            process.Start();

            Thread.Sleep(500);
            Prompt(process);
        }

        public static void Prompt(Process process)
        {
            string command;
            do
            {
                try
                {
                    command = Console.ReadLine() ?? string.Empty;

                    if (command.StartsWith("enqueue "))
                    {
                        // Syntax: "enqueue {role.name} {message}".
                        // Use as a general input.

                        var arguments = command["enqueue ".Length..];
                        var index = arguments.IndexOf(' ');
                        var roleName = arguments[..index];
                        var content = arguments[(index + 1)..];

                        var role = process.GetRole(roleName);
                        role.Queue.Enqueue(new Message(contextId: default, content));
                    }
                    else if (command.StartsWith("sum "))
                    {
                        // Syntax: "sum 1 2 3".
                        // Use as a synonym of "enqueue A { list_of_items: [1, 2, 3] }".

                        var arguments = command["sum ".Length..];
                        var items = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray();

                        var role = process.GetRole("A");
                        var content = JsonConvert.SerializeObject(new { list_of_items = items });
                        var message = new Message(contextId: default, content);
                        role.Queue.Enqueue(message);
                    }
                    else if (command == "exit")
                    {
                        Output.WriteWarningLine("Prompt :: Exit :: Waiting for Output to terminate...");
                        ExitWaitHandle.Set();
                        Environment.Exit(0);
                    }
                    else
                        throw new UsageException("Unknown command");
                }
                catch (Exception ex)
                {
                    Output.WriteErrorLine($"Prompt :: Exception :: {ex.Message}");
                }
            }
            while (true);
        }
    }

    static class OpenAILambdas
    {
        static readonly OpenAIClient Client = new(Program.OpenAIKey);

        public static string GenericRole(Agent agent, Message message)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                ResponseFormat = ChatCompletionsResponseFormat.JsonObject,
                Temperature = 0.1f,
                Messages =
                {
                    new ChatRequestSystemMessage(agent.Role.RoleScript),
                    new ChatRequestUserMessage(message.Content),
                },
            };
            Response<ChatCompletions> response = Client.GetChatCompletions(chatCompletionsOptions);
            ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
            return responseMessage.Content;
        }
    }

    static class MockLambdas
    {
        // Usage:
        // enqueue A { list_of_items: [] }
        // enqueue A { list_of_items: [1] }
        // enqueue A { list_of_items: [1, 2] }
        public static string RoleA(Agent _, Message message)
        {
            JObject obj = JObject.Parse(message.Content);
            JToken list_of_items = obj.GetValue("list_of_items")!;
            int[] array = list_of_items.Values<int>().ToArray();

            Reaction reaction;
            if (array.Length == 0)
                reaction = new Reaction(new List<Instruction> {
                        new() { "forward", "D", new { result = 0 } }
                    });
            else if (array.Length == 1)
                reaction = new Reaction(new List<Instruction>
                    {
                        new() { "forward", "D", new { result = array[0] } }
                    });
            else
                reaction = new Reaction(new List<Instruction>
                    {
                        new() { "open" },
                        new() { "set", "list_of_items", list_of_items },
                        new() { "set", "intermediate_result", 0 },
                        new() { "forward", "B" }
                    });
            return JsonConvert.SerializeObject(reaction);
        }

        // Usage:
        // enqueue B { "state": { "list_of_items": [], "intermediate_result": 42 } }
        // enqueue B { "state": { "list_of_items": [1], "intermediate_result": 42 } }
        public static string RoleB(Agent _, Message message)
        {
            JObject obj = JObject.Parse(message.Content);
            JToken state = obj.GetValue("state")!;
            JToken list_of_items = state["list_of_items"]!;
            int[] array = list_of_items.Values<int>().ToArray();

            Reaction reaction;
            if (array.Length == 0)
            {
                var intermediate_result = state["intermediate_result"]!.Value<int>();
                reaction = new Reaction(new List<Instruction>
                    {
                        new() { "close" },
                        new() { "forward", "D", new { result = intermediate_result } }
                    });
            }
            else
            {
                var last_item = array[^1];
                reaction = new Reaction(new List<Instruction>
                    {
                        new() { "pop", "list_of_items" },
                        new() { "forward", "C", new { item = last_item } }
                    });
            }
            return JsonConvert.SerializeObject(reaction);
        }

        // Usage:
        // enqueue C { "state": { "intermediate_result": 42 }, "data": { "item": 1 } }
        public static string RoleC(Agent _, Message message)
        {
            JObject obj = JObject.Parse(message.Content);
            JToken state = obj.GetValue("state")!;
            JToken data = obj.GetValue("data")!;
            var intermediate_result = state["intermediate_result"]!.Value<int>();
            var item = data["item"]!.Value<int>();

            var reaction = new Reaction(new List<Instruction>
                {
                    new() { "set", "intermediate_result", intermediate_result + item },
                    new() { "forward", "B" }
                });
            return JsonConvert.SerializeObject(reaction);
        }

        // Usage:
        // enqueue D { "state": { "result": 42 } }
        public static string RoleD(Agent _, Message message)
        {
            JObject obj = JObject.Parse(message.Content);
            JToken data = obj.GetValue("data")!;
            var result = data["result"]!.Value<int>();

            var reaction = new Reaction(new List<Instruction>
                {
                    new() { "complete", new { result } }
                });
            return JsonConvert.SerializeObject(reaction);
        }
    }
}
