using SystemInformations = UnityEngine.SystemInfo;

namespace Developer
{
    public class SystemInfo : ConsoleModule
    {
        private void Awake()
        {
            command = "system_info";
            arguments = 0;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            host.Print("Current computer informations:");
            host.Print("Graphics card: " + SystemInformations.graphicsDeviceName);
            host.Print("Processor: " + SystemInformations.processorType);
            host.Print("RAM: " + SystemInformations.systemMemorySize + " MB");
            host.Print("Operating system: " + SystemInformations.operatingSystem + "\n ");
            return true;
        }
    }
}