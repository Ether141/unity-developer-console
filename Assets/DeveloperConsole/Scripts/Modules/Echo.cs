﻿namespace Developer
{
    public class Echo : ConsoleModule
    {
        private void Awake ()
        {
            command = "echo";
            arguments = -1;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            if(arguments.Length == 0)
            {
                result = CommandResults.WRONG_ARGS_MESS;
                return false;
            }

            try
            {
                string tmp = "";
                foreach (string s in arguments)
                    tmp += s + " ";
                host.Print(tmp);
            }
            catch
            {
                result = CommandResults.WRONG_ARGS_MESS;
                return false;
            }

            return true;
        }
    }
}
