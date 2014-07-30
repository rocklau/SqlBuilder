namespace EasySql
{
    public static class AppSettings
    {
        public static string DbConnectionString { get { return GetConnectionString("DbConnectionString"); } }
      
        public static string GetConnectionString(string name)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;

        } 
        public static string Get(string name)
        {
            string set = System.Configuration.ConfigurationManager.AppSettings[name];

            if (set == null)
            {
                throw new AppSettingReadErrorException(name);
            }
            return set.ToString();
        }

    }
}