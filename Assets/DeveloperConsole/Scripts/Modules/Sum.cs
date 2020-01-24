namespace Developer
{
    public class Sum : ConsoleModule
    {
        private void Awake()
        {
            command = "sum";
            arguments = 2;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            int a, b;

            try
            {
                a = int.Parse(arguments[0]);
                b = int.Parse(arguments[1]);
            }
            catch
            {
                result = CommandResults.WRONG_ARGS_MESS;
                return false;
            }

            host.Print((a + b).ToString());
            return true;
        }
    }
}
