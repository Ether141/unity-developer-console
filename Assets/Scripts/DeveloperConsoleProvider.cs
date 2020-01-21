using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace Developer
{
    /// <summary>
    /// Main class which provides access to developer console.
    /// </summary>
    public class DeveloperConsoleProvider : MonoBehaviour
    {
        [Header("Main settings")]
        [SerializeField] private KeyCode keyToOpenClose = KeyCode.Tilde;
        [Space]
        [SerializeField] private List<ConsoleModule> modules = new List<ConsoleModule>();
        
        [Header("UI")]
        [SerializeField] private GameObject mainUI;
        [SerializeField] private TextMeshProUGUI buffer;
        [SerializeField] private TMP_InputField inputField;

        private bool isOpened = false;

        private void Update()
        {
            OpenCloseConsole();

            if (Input.GetKeyDown(KeyCode.Return) && inputField.text.Length > 0)
                ExecuteCommand(inputField.text);
        }

        private void OpenCloseConsole()
        {
            if (Input.GetKeyDown(keyToOpenClose))
            {
                mainUI.SetActive(!isOpened);
                isOpened = !isOpened;

                if (isOpened)
                {
                    inputField.ActivateInputField();
                }
            }
        }

        /// <summary>
        /// Executes given command.
        /// </summary>
        /// <param name="c"></param>
        public void ExecuteCommand (string c)
        {
            Print(c);

            List<string> words = new List<string>();
            words.AddRange(c.Trim().Split(' '));

            string command = words[0];
            words.RemoveAt(0);

            inputField.text = "";
            inputField.ActivateInputField();

            ConsoleModule module = null;
            bool correctCommand = false;

            foreach(ConsoleModule m in modules)
            {
                if (m.CompareCommand(command))
                {
                    module = m;
                    correctCommand = true;
                    break;
                }
            }

            if (!correctCommand)
            {
                Print($"<color=red>{CommandResults.WRONG_COMMAND_MESS}</color>");
                return;
            }

            string executionResult = "";
            bool exectuedCorrectly = module.ExecuteCommand(this, ref executionResult, words.ToArray());

            if(!exectuedCorrectly)
            {
                Print($"<color=red>{executionResult}</color>");
                return;
            }
        }

        /// <summary>
        /// Adds new <seealso cref="ConsoleModule"/> to console.
        /// </summary>
        /// <param name="moduleToAdd"></param>
        public void AddModule(ConsoleModule moduleToAdd) => modules.Add(moduleToAdd);

        /// <summary>
        /// Prints given value.
        /// </summary>
        /// <param name="message"></param>
        public void Print(string message) => buffer.text += "\n" + message;
    }
}
