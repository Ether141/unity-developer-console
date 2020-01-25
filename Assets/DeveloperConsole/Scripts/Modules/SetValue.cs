using UnityEngine;
using System.Reflection;
using System;

namespace Developer
{
    public class SetValue : ConsoleModule
    {
        [SerializeField] private Value[] values;

        [Serializable]
        public struct Value
        {
            public MonoBehaviour target;
            public string valueName;
            public ValueType valueType;
        }

        private void Awake()
        {
            command = "set";
            arguments = 2;
        }

        public override bool ExecuteCommand(DeveloperConsoleProvider host, ref string result, string[] arguments)
        {
            MonoBehaviour target = null;
            string valueName = "";
            ValueType valueType = ValueType.Integer;

            foreach(var val in values)
            {
                if(arguments[0] == val.valueName)
                {
                    target = val.target;
                    valueName = val.valueName;
                    valueType = val.valueType;
                }
            }

            if(target == null)
            {
                result = CommandResults.WRONG_ARGS_MESS;
                return false;
            }

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
                isOk = int.TryParse(arguments[1], out intVal);
            }
            else if (valueType == ValueType.Float)
            {
                isOk = float.TryParse(arguments[1], out floatVal);
            }
            else if (valueType == ValueType.Boolean)
            {
                isOk = bool.TryParse(arguments[1], out boolVal);
            }
            else if (valueType == ValueType.String)
            {
                stringVal = arguments[1];
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