using UnityEngine;

namespace Developer
{
    public class ConsoleModule : MonoBehaviour
    {
        public string command;
        public int arguments = 0;

        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="result"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public virtual bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            result = "";
            return false;
        }

        /// <summary>
        /// Returns true if given text matches to this command, otherwise returns false.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CompareCommand (string text)
        {
            if (text == command)
                return true;

            return false;
        }
    }
}
