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
        [SerializeField] private bool autoConnectModules = true;
        
        [Header("UI")]
        [SerializeField] private GameObject mainUI;
        [SerializeField] private TextMeshProUGUI buffer;
        [SerializeField] private TMP_InputField inputField;

        private bool isOpened = false;

        private List<string> lastCommands = new List<string>();
        private int lastCommandIndex = 0;

        private void Awake()
        {
            if(autoConnectModules)
                modules.AddRange(FindObjectsOfType<ConsoleModule>());
        }

        private void Update()
        {
            OpenCloseConsole();
            RestoreCommands();

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
            AddLastCommand(c.Trim());
            lastCommandIndex = lastCommands.Count;

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
                PrintError(CommandResults.WRONG_COMMAND_MESS);
                return;
            }

            if (module.arguments > 0)
            {
                if(words.Count != module.arguments)
                {
                    PrintError(CommandResults.WRONG_ARGS_MESS);
                    return;
                }
            }

            string executionResult = "";
            bool exectuedCorrectly = module.ExecuteCommand(this, ref executionResult, words.ToArray());

            if(!exectuedCorrectly)
            {
                PrintError(executionResult);
                return;
            }
            else if (exectuedCorrectly && !string.IsNullOrEmpty(executionResult))
            {
                Print(executionResult);
                return;
            }
        }

        private void AddLastCommand (string command)
        {
            lastCommands.Add(command);
        }

        private void RestoreCommands ()
        {
            if (lastCommands.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (lastCommandIndex <= 0)
                    {
                        lastCommandIndex = 0;
                    }
                    else
                    {
                        lastCommandIndex--;
                    }

                    inputField.text = lastCommands[lastCommandIndex];
                    inputField.MoveTextEnd(false);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (lastCommandIndex >= lastCommands.Count - 1)
                    {
                        lastCommandIndex = lastCommands.Count - 1;
                    }
                    else
                    {
                        lastCommandIndex++;
                    }

                    inputField.text = lastCommands[lastCommandIndex];
                    inputField.MoveTextEnd(false);
                }
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

        /// <summary>
        /// Prints given value as error.
        /// </summary>
        /// <param name="message"></param>
        public void PrintError(string message) => buffer.text += $"\n<color=red>{message}</color>";

        /// <summary>
        /// Prints given value as warning.
        /// </summary>
        /// <param name="message"></param>
        public void PrintWarning(string message) => buffer.text += $"\n<color=yellow>{message}</color>";
    }
}
