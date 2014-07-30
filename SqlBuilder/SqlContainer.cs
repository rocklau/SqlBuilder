using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace EasySql
{
    public static class SqlOrderType
    {
        public static string Asc { get { return "ASC"; } }
        public static string Desc { get { return "DESC"; } }
    }
    


   
    public class SqlContainer
    {
        public List<DbParameter> SqlParameter { get; set; }
        public StringBuilder SqlStatement { get; set; }
        public StringBuilder Condition { get; set; }
        public StringBuilder OrderStatement { get; set; }
        public string SqlEnd = "";
        public SqlContainer()
        {
            SqlStatement = new StringBuilder();
            Condition = new StringBuilder();
            OrderStatement = new StringBuilder();
            SqlParameter = new List<DbParameter>();
        }
    }
}
