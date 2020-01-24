namespace Developer
{
    public class SaveLogs : ConsoleModule
    {
        private void Awake()
        {
            command = "save_logs";
            arguments = 0;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            host.SaveLogs();
            result = "Saved all logs correctly";
            return true;
        }
    }
}