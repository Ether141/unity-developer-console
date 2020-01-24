namespace Developer
{
    public class RemoveAlias : ConsoleModule
    {
        private void Awake()
        {
            command = "remove_alias";
            arguments = 1;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            string aliasName = arguments[0];

            if (!host.HasAlias(aliasName))
            {
                result = "This alias does not exist!";
                return false;
            }

            host.RemoveAlias(aliasName);
            return true;
        }
    }
}
