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
                result = "This alias has been already declared!";
                return false;
            }

            if(host.HasCommand(commandName))
            {
                result = "Given command is not correct";
                return false;
            }

            host.AddAlias(new Alias(aliasName, commandName));
            return true;
        }
    }
}
