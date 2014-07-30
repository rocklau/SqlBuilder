using System;
using System.Runtime.Serialization;

namespace EasySql
{

    [Serializable]
    public class AppSettingReadErrorException : System.Exception
    {
        public AppSettingReadErrorException(string name)
            : base(name + " is not  set value in appSetting.")
        { }
    }
}