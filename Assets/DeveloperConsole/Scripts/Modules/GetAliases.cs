namespace Developer
{
    public class GetAliases : ConsoleModule
    {
        private void Awake()
        {
            command = "get_aliases";
            arguments = 0;
        }
        
        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            string list = host.GetAliasesList();
            host.Print(list);
            return true;
        }
    }
}
