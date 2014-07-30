using System;
using System.Runtime.Serialization;

namespace EasySql
{
    [Serializable]
    public class SqlStatmentException : System.Exception
    {
        public SqlStatmentException(string msg)
            : base("SqlStatmentException:" + msg)
        { }
    }
}