namespace Developer
{
    public class Clear : ConsoleModule
    {
        private void Awake()
        {
            command = "clear";
            arguments = 0;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            host.ClearBuffer();
            return true;
        }
    }
}