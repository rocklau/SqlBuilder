using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using ComLib;
using System.Data.SqlClient;
using System.Threading;
using ComLib.Data;
using AutoMapper;
using EasySql.Db;

namespace EasySql
{

    public class SqlBuilder : IDisposable
    {
        #region initSqlBuilder
        // Private object with lazy instantiation
        private static readonly Lazy<SqlBuilder> instance =
            new Lazy<SqlBuilder>(
                delegate
                {
                    return new SqlBuilder();
                }
                //thread safety first
                , LazyThreadSafetyMode.PublicationOnly);



        // static instance property
        public static SqlBuilder Instance
        {
            get { return instance.Value; }
        }

        public SqlContainer SqlContainer { get; set; }
        public Database Db { get; set; }


        public SqlBuilder(SqlContainer sqlContainer)
        {
            SqlContainer = sqlContainer;

            CreateDatabase();
        }

        private void CreateDatabase()
        {
            Db = new Database(AppSettings.DbConnectionString);
        }
        private void CreateDatabase(IDatabase db)
        {
            Db = db as Database;
        }
        private void CreateDatabase(string connectionString)
        {
            Db = new Database(connectionString);
        }
        public SqlBuilder()
        {
            SqlContainer = new SqlContainer();
            CreateDatabase();
        }
        public SqlBuilder(IDatabase db)
        {
            SqlContainer = new SqlContainer();
            CreateDatabase(db);
        }
        public SqlBuilder(string connectionString)
        {
            SqlContainer = new SqlContainer();
            CreateDatabase(connectionString);
        }

        public void Update(string tableName, string[][] setfieldarg)
        {
            ReSet("");
           
            var updateSetSql = "";
            foreach (var setfield in setfieldarg)
            {
                updateSetSql += string.Format(" {0} = {1} ,"
                    , setfield[0]
                    , "@" + setfield[0]);
                AddPara(setfield[0], setfield[1]);
            };

            ReSql(string.Format("update {0} set {1}", tableName, updateSetSql).TrimEnd(','));

            SqlContainer.SqlEnd=" ;select @@ROWCOUNT;";
           
        }


        #endregion
        #region privateMethod
        private void ReSql(string sql)
        {
            SqlContainer.SqlStatement.Clear();
            SqlContainer.SqlStatement.Append(sql);
        }
      
        private void SymbolConditions(string symbol, string condi, string name, DbType dbtype, object val)
        {
            SymbolConditions(symbol, condi, "@", name, dbtype, val);

        }
        private void SymbolConditions(string symbol, string condi, string condiresult, string name, DbType dbtype, object val)
        {

            string par = name.Trim().Replace(".", "dot");
            if (SqlContainer.Condition.ToString().Contains(par + " "))
            {
                par += "_other";
            }
            // {AND} {T.Field} {>=} {@ParaName} 
            SqlContainer.Condition.AppendFormat((" {0} {1} {2} {3} "), symbol, name, condi, condiresult.Replace("@", "@" + par));
            SqlContainer.SqlParameter.Add(Db.BuildInParam(par, dbtype, val));

        }
        #endregion

        public void ReSet(string sql)
        {
            SqlContainer.SqlParameter.Clear();
            SqlContainer.SqlStatement.Clear();
            SqlContainer.Condition.Clear();
            SqlContainer.OrderStatement.Clear();
            SqlContainer.SqlStatement.Append(sql);
            SqlContainer.SqlEnd = "";
        }
        public void ReSet(string sql,string sqlend)
        {
            SqlContainer.SqlParameter.Clear();
            SqlContainer.SqlStatement.Clear();
            SqlContainer.Condition.Clear();
            SqlContainer.OrderStatement.Clear();
            SqlContainer.SqlStatement.Append(sql);
            SqlContainer.SqlEnd = sqlend;
        }

        public void Replace(string id, string val)
        {
            ReSql(SqlContainer.SqlStatement.ToString().Replace("{" + id + "}", val));
        }
        public string GetSql()
        {
            var condition = SqlContainer.Condition.ToString();
            var sql = "";
            if (SqlContainer.SqlStatement.ToString().ToLower().Contains("{where}"))
            {
                sql = SqlContainer.SqlStatement.ToString().Replace("{where}", condition.Trim() == "" ? " 1=1 " : condition);
            }
            else if (SqlContainer.SqlStatement.ToString().ToLower().Contains("where"))
            {
                sql = SqlContainer.SqlStatement.ToString() + condition;
            }
            else if (condition.Trim().ToLower().Contains("and") && condition.Trim().ToLower().Substring(0, 3) == "and")
            {
                sql = String.Format("{0} WHERE {1} ", SqlContainer.SqlStatement.ToString(), condition.Trim().Remove(0, 3));
            }
            else if (condition.Length > 0)
            {
                sql = String.Format("{0} WHERE {1} ", SqlContainer.SqlStatement.ToString(), condition);
            }
            else
            {
                sql = SqlContainer.SqlStatement.ToString();
            }
            var order = SqlContainer.OrderStatement.ToString();
            if (sql.ToLower().Contains(" order "))
            {
                order = "";
            }
            return sql + order +SqlContainer.SqlEnd;
        }
        #region SqlMachine
        public void AndQuickSearchSql(string name, string letter = "All")
        {
            if (letter == "0-9")
            {
                letter = "[0-9]";
            }
            if (letter != "All")
                AndRightLikeSql(name.Trim().Replace(",", ""), letter);
        }
        public void OrQuickSearchSql(string name, string letter = "All")
        {
            if (letter == "0-9")
            {
                letter = "[0-9]";
            }
            if (letter != "All")
                OrRightLikeSql(name.Trim().Replace(",", ""), letter);
        }

        public void QuickSearchSql(string name, string letter = "All")
        {
            if (letter == "0-9")
            {
                letter = "[0-9]";
            }
            if (letter != "All")
                RightLikeSql(name.Trim().Replace(",", ""), letter);
        }
        public void AndNotRemoveSql(string name = "Removed")
        {
            AndSql();
            ParenLeftSql();
            IsNullSql(name);
            OrEqualTrueSql(name);
            ParenRightSql();

        }

        /// <summary>
        /// Sql (
        /// </summary>
        public void ParenLeftSql()
        {
            SqlContainer.Condition.Append(" ( ");

        }
        public void ParenRightSql()
        {
            SqlContainer.Condition.Append(" ) ");

        }
        public void OrEqualTrueSql(string name)
        {
            SqlContainer.Condition.AppendFormat(" OR {0} = 1 ", name);

        }
        public void OrEqualSql(string name, object val)
        {
            SymbolConditions("OR", "=", name, DbType.String, val);

        }
        public void AndSql()
        {
            SqlContainer.Condition.Append(" AND ");

        }
        public void EqualIntSql(string name, int val)
        {
            SymbolConditions(" ", "=", name, DbType.Int32, val);

        }
        public void EqualSql(string name, object val)
        {
            SymbolConditions(" ", "=", name, DbType.String, val);

        }
        public void IsNotNullSql(string name)
        {

            SqlContainer.Condition.AppendFormat("  {0} IS NOT NULL ", name);

        }
        public void AndIsNotNullSql(string name)
        {

            SqlContainer.Condition.AppendFormat("AND {0} IS NOT NULL ", name);

        }
        public void OrIsNotNullSql(string name)
        {

            SqlContainer.Condition.AppendFormat("or {0} IS NOT NULL ", name);

        }
        public void IsNullSql(string name)
        {

            SqlContainer.Condition.AppendFormat("  {0} IS NULL ", name);

        }
        public void AndIsNullSql(string name)
        {

            SqlContainer.Condition.AppendFormat("AND {0} IS NULL ", name);

        }
        public void LikeSql(string name, string val)
        {
            SymbolConditions(" ", "LIKE", "'%'+ @ + '%'", name, DbType.String, val);
        }
        public void LeftLikeSql(string name, string val)
        {
            SymbolConditions(" ", "LIKE", "'%'+ @", name, DbType.String, val);
        }
        public void RightLikeSql(string name, string val)
        {
            SymbolConditions(" ", "LIKE", "@ + '%'", name, DbType.String, val);
        }
        public void AndLikeSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("AND", "LIKE", "'%'+ @ + '%'", name, dbtype, val);
        }
        public void AndLikeSql(string name, object val)
        {

            SymbolConditions("AND", "LIKE", "'%'+ @ + '%'", name, DbType.String, val);
        }
        public void ORLikeSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("OR", "LIKE", "'%'+ @ + '%'", name, dbtype, val);
        }
        public void ORLikeSql(string name, object val)
        {

            SymbolConditions("OR", "LIKE", "'%'+ @ + '%'", name, DbType.String, val);
        }
        //////////////////////////////
        public void OrRightLikeSql(string name, object val)
        {
            SymbolConditions("OR", "LIKE", "''+ @ + '%'", name, DbType.String, val);
        }
        public void AndRightLikeSql(string name, object val)
        {

            SymbolConditions("AND", "LIKE", "''+ @ + '%'", name, DbType.String, val);
        }
        public void AndInSql(string name, string val)
        {
            val = val.Replace(",,", ",").Trim(',');
            CheckInts(val);
            SqlContainer.Condition.Append(" AND " + name + " IN (" + val + ") ");
        }
        public void AndInSql(string name, int[] val)
        {
            SqlContainer.Condition.Append(" AND " + name + " IN (" + LongToString(val) + ") ");
        }

        public void AndNotInSql(string name, string val)
        {
            val = val.Replace(",,", ",").Trim(',');
            CheckInts(val);
            SqlContainer.Condition.Append(" AND " + name + " not IN (" + val + ") ");
        }
        public void AndNotInSql(string name, int[] val)
        {
            SqlContainer.Condition.Append(" AND " + name + " not IN (" + LongToString(val) + ") ");
        }
        public void AndMoreOrEqualSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("AND", ">=", name, dbtype, val);

        }
        public void AndEqualSql(string name, object val)
        {
            SymbolConditions("AND", "=", name, DbType.String, val);

        }
        public void AndEqualIntSql(string name, int val)
        {
            SymbolConditions("AND", "=", name, DbType.Int32, val);

        }
        public void AndNotEqualSql(string name, object val)
        {
            SymbolConditions("AND", "<>", name, DbType.String, val);

        }
        public void AndNotEqualIntSql(string name, int val)
        {
            SymbolConditions("AND", "<>", name, DbType.Int32, val);

        }
        public void AndEqualSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("AND", "=", name, dbtype, val);

        }
        public void AndLessOrEqualSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("AND", "<=", name, dbtype, val);
        }
        public void AndLessSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("AND", "<", name, dbtype, val);
        }
        public void OrLessOrEqualSql(string name, DbType dbtype, object val)
        {
            SymbolConditions("OR", "<=", name, dbtype, val);
        }
        public void AndTimeMoreLessEqualSql(string name, DateTime start, DateTime end)
        {
            AndMoreOrEqualSql(name, DbType.DateTime, start);
            AndLessOrEqualSql(name, DbType.DateTime, end);
        }
        private static void CheckInts(string val)
        {
            List<int> al = new List<int>();
            string[] arr = val.Split(',');
            int v = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (!Int32.TryParse(arr[i], out v))
                {
                    throw new SqlStatmentException("SQL statement can not contain  '" + arr[i] + "' . may be  it's value is error type. Value is '" + val + "'");

                }

            }
        }
        public static string LongToString(int[] val)
        {
            string s = "";
            for (int i = 0; i < val.Length - 1; i++)
            {
                s += val[i] + ",";
            }
            s += val[val.Length - 1];
            return s;

        }
        public void OrInSql(string name, string val)
        {
            val = val.Replace(",,", ",").Trim(',');
            CheckInts(val);
            SqlContainer.Condition.Append(" Or " + name + " IN (" + val + ") ");

        }



        public void AppendStrSQL(string strSQL)
        {
            SqlContainer.Condition.Append(" " + strSQL + " ");
        }
        public void AddPara(string name, string val)
        {
            SqlContainer.SqlParameter.Add(Db.BuildInParam(name, DbType.String, val));
        }
        public void AddPara(string name, int val)
        {
            SqlContainer.SqlParameter.Add(Db.BuildInParam(name, DbType.Int32, val));
        }
        public void AddPara(string name, long val)
        {
            SqlContainer.SqlParameter.Add(Db.BuildInParam(name, DbType.Int64, val));
        }
        public DataTable Execute(out int count)
        {
            var tb = Db.ExecuteDataTableText(GetSql(), SqlContainer.SqlParameter.ToArray());
            count = tb.Rows.Count;
            return tb;
        }
        public DataTable Execute()
        {

            return Db.ExecuteDataTableText(GetSql(), SqlContainer.SqlParameter.ToArray());

        }
        public IList<T> Execute<T>()
        {
            return Db.Query<T>(GetSql(), CommandType.Text, SqlContainer.SqlParameter.ToArray(), new ModelMapper<T>());
        }
        public IList<T> Execute<T, TMapper>()
        {

            return Db.Query<T>(GetSql(), CommandType.Text, SqlContainer.SqlParameter.ToArray(), Activator.CreateInstance(typeof(TMapper)) as ModelMapper<T>);
        }
        public void ExecuteReader(Action<IDataReader> action)
        {
            Db.ExecuteReaderText(GetSql(), action, SqlContainer.SqlParameter.ToArray());

        }

        public string ExecuteScalarText()
        {
            var text = Db.ExecuteScalarText(GetSql(), SqlContainer.SqlParameter.ToArray());
            return text == null ? "" : text.ToString();

        }

        public string ExecuteScalarText(string sql)
        {
            List<DbParameter> para = new List<DbParameter>();
            var text = Db.ExecuteScalarText(sql, para.ToArray());
            return text == null ? "" : text.ToString();
        }
        public string ExecuteScalarTextById(string sql, string name, string val)
        {
            List<DbParameter> para = new List<DbParameter>();
            para.Add(Db.BuildInParam(name, DbType.Int32, val));
            return Db.ExecuteScalarText(sql, para.ToArray()).ToString();

        }
        public string ExecuteScalarTextByString(string sql, string name, string val)
        {
            List<DbParameter> para = new List<DbParameter>();
            para.Add(Db.BuildInParam(name, DbType.String, val));
            return Db.ExecuteScalarText(sql, para.ToArray()).ToString();

        }
        public DataTable PageData(out int count, int pageIndex, int pageSize, string key, string fields, string tables, string sort, string order)
        {
            var where = PageCreateSql(pageIndex, pageSize, key, fields, tables, sort, order);

            int rowcount = 0;

            var dt = Execute(out rowcount);

            count = TotalTableRows(key, fields, tables, where, rowcount);
            return dt;
        }
        public IList<T> PageData<T, TMapper>(out int count, int pageIndex, int pageSize, string key, string fields, string tables, string sort, string order)
        {
            var where = PageCreateSql(pageIndex, pageSize, key, fields, tables, sort, order);

            var dt = Execute<T, TMapper>();

            count = TotalTableRows(key, fields, tables, where, dt.Count);

            return dt;

        }

        private int TotalTableRows(string key, string fields, string tables, string where, int rowcount)
        {
            int count;
            count = 0;
            if (rowcount > 0)
            {
                SqlContainer.SqlStatement = new StringBuilder(String.Format("SELECT count({3}) FROM (SELECT {0} FROM {1} {2}) as tbltbl", fields, tables, where, key));
                string strcount = ExecuteScalarText();
                if (!string.IsNullOrEmpty(strcount))
                {
                    count = Convert.ToInt32(strcount);
                }

            }
            return count;
        }

        private string PageCreateSql(int pageIndex, int pageSize, string key, string fields, string tables, string sort, string order)
        {

            string where = "  WHERE 1=1 ";
            List<DbParameter> wherepara = SqlContainer.SqlParameter;
            if (SqlContainer.Condition != null && SqlContainer.Condition.Length > 0 && !SqlContainer.Condition.ToString().ToLower().Contains(" where "))
            {
                where = where + SqlContainer.Condition.ToString();
                SqlContainer.Condition.Clear();
            }

            SqlContainer.SqlParameter.Add(Db.BuildInParam("startRowIndex", DbType.Int32, pageIndex * pageSize));
            SqlContainer.SqlParameter.Add(Db.BuildInParam("maximumRows", DbType.Int32, pageSize));

            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SET ROWCOUNT @maximumRows ; ");

            sql.AppendLine("WITH OrderedRows As");
            sql.AppendLine("(");

            sql.AppendLine(String.Format("SELECT tbltbl.*, ROW_NUMBER() OVER ( Order By tbltbl.{3} {4}) as RowNum FROM  (SELECT {0} FROM {1} {2}) as tbltbl ", fields, tables, where, string.IsNullOrEmpty(sort) ? key : sort, order));

            sql.AppendLine(")");

            sql.AppendLine("SELECT   * FROM OrderedRows Where RowNum > @startRowIndex ");
            sql.AppendLine(String.Format(" Order By {0} {1}", string.IsNullOrEmpty(sort) ? key : sort, order));
            SqlContainer.SqlStatement = sql;
            return where;
        }




        public void ExecuteNonQueryText()
        {
            Db.ExecuteNonQueryText(GetSql(), SqlContainer.SqlParameter.ToArray());

        }
        public void ExecuteNonQueryText(string sql, string name, string val)
        {
            List<DbParameter> para = new List<DbParameter>();
            para.Add(Db.BuildInParam(name, DbType.String, val));
            Db.ExecuteNonQueryText(sql, para.ToArray());

        }

        public void OrderByAsc(string name)
        {
            SqlContainer.OrderStatement.Append(" ORDER BY " + name + " ASC ");

        }
        public void OrderByDesc(string name)
        {
            SqlContainer.OrderStatement.Append(" ORDER BY " + name + " DESC ");
        }
        #endregion


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            var conn = this.Db.GetConnection();
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();

            }
            this.Db = null;

        }





    }
}
