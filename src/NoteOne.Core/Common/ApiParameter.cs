using System;
using System.Collections.Generic;

namespace NoteOne_Core.Common
{
    public class ApiParameter
    {
        public ApiParameter()
        {
            Parameters = new Dictionary<int, Func<object>>();
        }

        public int ParameterCount
        {
            get { return Parameters.Count; }
        }

        public Dictionary<int, Func<object>> Parameters { get; set; }

        public object GetParameter(int key)
        {
            if (Parameters != null)
                return Parameters[key]();
            else return null;
        }


        public object[] GetParameters()
        {
            var parameters = new object[ParameterCount];
            for (int i = 0; i < ParameterCount; i++)
            {
                parameters[i] = GetParameter(i);
            }
            return parameters;
        }
    }
}