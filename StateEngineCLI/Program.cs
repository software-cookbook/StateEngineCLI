using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StateEngineCLI
{
    internal class Program 
    {
        private static bool reachedToTerminalState = false;

        static void Main(string[] args)
        {
            var fileContent = System.IO.File.ReadAllText("StateEngineConfig.json");
            var config = JsonSerializer.Deserialize<StateEngineConfig>(fileContent);

            var engine = new StateEngine(config, OnStatuChanged);

            do
            {
                Console.WriteLine( $"\nCurrent status :{engine.GetCurrentState()}");

                var availableActions = engine.GetAvailableActions();

                Console.WriteLine("Available actions :\n");
                foreach (var action in availableActions)
                {
                    Console.WriteLine($"* {action}");
                }

                Console.Write("Enter Next Action:");
                var nextAction = Console.ReadLine();

                engine.ApplyAction(nextAction);

            } while (!reachedToTerminalState);
        }

        public static void OnStatuChanged(string previousStatus, string currentStatus, bool isTerminalState)
        {
            Console.WriteLine($"Status changed from {previousStatus} to {currentStatus}");
            if (isTerminalState)
            {
                Console.WriteLine("Reached to terminal state");
            }
            reachedToTerminalState = isTerminalState;
        }
    }
}
