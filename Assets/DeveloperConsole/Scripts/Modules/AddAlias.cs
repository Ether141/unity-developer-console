namespace Developer
{
    public class AddAlias : ConsoleModule
    {
        private void Awake()
        {
            command = "add_alias";
            arguments = 2;
        }
        
        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            string aliasName = arguments[0];
            string commandName = arguments[1];

            if(host.HasAlias(aliasName))
            {
                host.RemoveAlias(aliasName);
            }

            host.AddAlias(new Alias(aliasName, commandName));
            return true;
        }
    }
}
