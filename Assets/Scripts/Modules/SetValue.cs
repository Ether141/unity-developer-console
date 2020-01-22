using UnityEngine;
using System.Reflection;
using System;

namespace Developer
{
    public class SetValue : ConsoleModule
    {
        [SerializeField] private string valueName = "";
        [SerializeField] private MonoBehaviour target;
        [SerializeField] private ValueType valueType = ValueType.Integer;

        private void Awake()
        {
            command = "set_" + valueName;
            arguments = 1;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            Type objType = target.GetType();
            FieldInfo field = objType.GetField(valueName);

            float floatVal = 0f;
            int intVal = 0;
            string stringVal = "";
            bool boolVal = false;
            bool isOk = false;

            if(field == null)
            {
                result = CommandResults.WRONG_COMMAND_MESS;
                return false;
            }

            if(valueType == ValueType.Integer)
            {
                isOk = int.TryParse(arguments[0], out intVal);
            }
            else if (valueType == ValueType.Float)
            {
                isOk = float.TryParse(arguments[0], out floatVal);
            }
            else if (valueType == ValueType.Boolean)
            {
                isOk = bool.TryParse(arguments[0], out boolVal);
            }
            else if (valueType == ValueType.String)
            {
                stringVal = arguments[0];
                isOk = true;
            }

            if (!isOk)
            {
                result = CommandResults.WRONG_ARGS_MESS;
                return false;
            }

            if (valueType == ValueType.Integer)
            {
                field.SetValue(target, intVal);
            }
            else if (valueType == ValueType.Float)
            {
                field.SetValue(target, floatVal);
            }
            else if (valueType == ValueType.Boolean)
            {
                field.SetValue(target, boolVal);
            }
            else if (valueType == ValueType.String)
            {
                field.SetValue(target, stringVal);
            }

            return true;
        }

        public enum ValueType
        {
            Integer, 
            Float, 
            String,
            Boolean
        }
    }
}