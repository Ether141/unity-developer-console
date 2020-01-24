using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.IO;

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
        [SerializeField] private List<Alias> aliases = new List<Alias>();
        [SerializeField] private bool autoConnectModules = true;
        
        [Header("UI")]
        [SerializeField] private GameObject mainUI;
        [SerializeField] private TextMeshProUGUI buffer;
        [SerializeField] private TMP_InputField inputField;

        [Header("Tips")]
        [SerializeField] private bool showTips = true;
        [SerializeField] [Tooltip("If true, script automatically sets tips from current modules.")] private bool autoSetTips = true;
                         public List<string> tips = new List<string>();
        [Space]
        [SerializeField] private Image tipBackground;
        [SerializeField] private TextMeshProUGUI tipUI;

        [Header("Logs")]
        [SerializeField] private bool autoSaveLogs = false;
        [SerializeField] private string logsPath = "";
                         private List<string> logs = new List<string>();

        private bool isOpened = false;

        private List<string> lastCommands = new List<string>();
        private int lastCommandIndex = 0;

        private void Start()
        {
            if(autoConnectModules)
                modules.AddRange(FindObjectsOfType<ConsoleModule>());

            if(autoSetTips)
            {
                foreach(ConsoleModule module in modules)
                    tips.Add(module.command);

                foreach (Alias alias in aliases)
                    tips.Add(alias.alias);
            }

            if (!Directory.Exists(logsPath))
                logsPath = Path.Combine(Application.dataPath, "developerconsolelogs.txt");
            else
                logsPath = Path.Combine(logsPath, "developerconsolelogs.txt");
        }

        private void Update()
        {
            OpenCloseConsole();
            RestoreCommands();

            if (Input.GetKeyDown(KeyCode.Tab))
                GetTip(true);

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
            if (string.IsNullOrEmpty(c))
                return;

            Print(c);

            if (autoSaveLogs)
                SaveLogs();

            lastCommands.Add(c.Trim());
            lastCommandIndex = lastCommands.Count;

            List<string> words = new List<string>();
            words.AddRange(c.Trim().Split(' '));

            string command = words[0];
            words.RemoveAt(0);

            inputField.text = "";
            inputField.ActivateInputField();

            ConsoleModule module = null;

            foreach(ConsoleModule m in modules)
            {
                if (m.CompareCommand(command))
                {
                    module = m;
                    break;
                }
            }

            if(module == null)
            {
                string alias = "";

                for (int i = 0; i < aliases.Count; i++)
                {
                    if(command == aliases[i].alias)
                    {
                        alias = aliases[i].associatedCommand;
                        break;
                    }
                }

                if (alias != "")
                {
                    foreach (ConsoleModule m in modules)
                    {
                        if (m.CompareCommand(alias))
                        {
                            module = m;
                            break;
                        }
                    }
                }
            }

            if (module == null)
            {
                PrintError(CommandResults.WRONG_COMMAND_MESS);
                return;
            }

            if (module.arguments >= 0)
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

        public void GetTip(bool setText = false)
        {
            if (!showTips)
            {
                RectTransform rect_ = tipBackground.GetComponent<RectTransform>();
                rect_.sizeDelta = new Vector2(rect_.sizeDelta.x, 0);
                tipUI.text = "";
                return;
            }

            string txt = inputField.text.Trim();
            List<string> tipLines = new List<string>();
            int lngt = 0;
            RectTransform rect = tipBackground.GetComponent<RectTransform>();
            tipUI.text = "";

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);

            for (int i = 0; i < tips.Count; i++)
            {
                string tip = "";

                if (tips[i].Length > txt.Length)
                    lngt = txt.Length;
                else
                    lngt = tips[i].Length;

                for (int j = 0; j < lngt; j++)
                {
                    if (txt[j] == tips[i][j])
                    {
                        tip = tips[i];
                    }

                    if (txt[j] != tips[i][j])
                    {
                        tip = "";
                        break;
                    }
                }

                if (txt.Length > tips[i].Length)
                    tip = "";

                if (tip != "")
                    tipLines.Add(tip);
            }

            foreach (var item in tipLines)
            {
                tipUI.text += item + "\n";
                rect.sizeDelta += new Vector2(0, 25f);
            }

            if (setText)
            {
                if (tipLines.Count > 0)
                {
                    inputField.text = tipLines[0];
                    inputField.caretPosition = tipLines[0].Length;
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
                    GetTip();
                }
            }
        }

        /// <summary>
        /// Saves all logs from this console.
        /// </summary>
        public void SaveLogs ()
        {
            if (!Directory.Exists(logsPath))
                logsPath = Path.Combine(Application.dataPath, "developerconsolelogs.txt");
            else
                logsPath = Path.Combine(logsPath, "developerconsolelogs.txt");

            if (!File.Exists(logsPath))
                File.WriteAllLines(logsPath, logs.ToArray());
            else
                File.AppendAllLines(logsPath, logs.ToArray());
        }

        /// <summary>
        /// Return true if given alias exists in aliases list.
        /// </summary>
        /// <param name="aliasToCheck"></param>
        /// <returns></returns>
        public bool HasAlias (string aliasToCheck) => aliases.Where(elem => elem.alias == aliasToCheck).Count() > 0;

        /// <summary>
        /// Returns true if given command exists.
        /// </summary>
        /// <param name="commandToCheck"></param>
        /// <returns></returns>
        public bool HasCommand(string commandToCheck) => !modules.Any(elem => elem.command == commandToCheck);

        /// <summary>
        /// Adds given alias to aliases list.
        /// </summary>
        /// <param name="aliasToAdd"></param>
        public void AddAlias(Alias aliasToAdd)
        {
            aliases.Add(aliasToAdd);
            tips.Add(aliasToAdd.alias);
        }

        /// <summary>
        /// Removes given alias.
        /// </summary>
        /// <param name="aliasToRemove"></param>
        public void RemoveAlias (string aliasToRemove)
        {
            if (!HasAlias(aliasToRemove))
                return;

            aliases.RemoveAll(item => item.alias == aliasToRemove);
        }

        /// <summary>
        /// Returns formatted all aliases list.
        /// </summary>
        /// <returns></returns>
        public string GetAliasesList ()
        {
            string list = "Total aliases count: " + aliases.Count;

            for(int i = 0; i < aliases.Count; i++)
                list += $"\n{aliases[i].alias} => {aliases[i].associatedCommand}";

            list += "\n ";
            return list;
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
        public void Print(string message)
        {
            buffer.text += "\n" + message;
            logs.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + message);
        }

        /// <summary>
        /// Prints given value as error.
        /// </summary>
        /// <param name="message"></param>
        public void PrintError(string message)
        {
            buffer.text += $"\n<color=red>{message}</color>";
            logs.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + message);
        }

        /// <summary>
        /// Prints given value as warning.
        /// </summary>
        /// <param name="message"></param>
        public void PrintWarning(string message)
        {
            buffer.text += $"\n<color=yellow>{message}</color>";
            logs.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + message);
        }

        /// <summary>
        /// Clears buffer.
        /// </summary>
        public void ClearBuffer() => buffer.text = "";
    }
}
