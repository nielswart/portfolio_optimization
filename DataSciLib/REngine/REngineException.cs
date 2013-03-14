using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.REngine
{
    [Serializable]
    public class REngineException : Exception
    {
        public new string Message { get; private set; }
        public new Exception InnerException { get; private set; }

        public REngineException(string message)
        {
            this.Message = message;
        }

        public REngineException(string message, Exception innerException)
        {
            this.Message = message;
            this.InnerException = innerException;
        }

    }
}
