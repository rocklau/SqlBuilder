using System;
using EasySql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComLib.Data;
using System.Linq;
using System.Collections.Generic;
using EasySql.Db;

namespace SqlBuilderTest
{

    [TestClass]
    public class SqlBuilderUnitTest
    { 
        SqlBuilder sqlbuilder;
        [TestInitialize]
        public void TestInitial()
        {

            sqlbuilder = new SqlBuilder(AppSettings.GetConnectionString("SampleDatabaseEntities"));

        }
        [TestCleanup]
        public void TestCleanup()
        {
            sqlbuilder.Dispose(); 
        }
        [TestMethod]
        public void TestAppendStrSQL()
        {

            string expected = "select * from sampleitem where id=1";
            const string sql = "select * from sampleitem";
            const string sqlsub2 = "id=1 ";
            string actual;
            sqlbuilder.ReSet(sql);
            sqlbuilder.AppendStrSQL(sqlsub2);
            actual = sqlbuilder.GetSql().Trim().ToLower().Replace("  ", " ");

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void SamplePageDataTest()
        {
            sqlbuilder.ReSet();

            var count = 0;
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            var result = sqlbuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>
                (out count, 0, 10, SampleItem_Table.Id, "*", SampleItem_Table.TableName, SampleItem_Table.Id, SqlOrderType.Asc
               );
            int expected = 1;
            var actual = result.FirstOrDefault().ID;

            Assert.AreEqual(expected, actual);


        }
        [TestMethod]
        public void SampleItemUpdateTest()
        {
           
                var expected = "1";
                var setfield = new[] { SampleItem_Table.Name, "test2" };
                var setfieldarg = new[] { setfield };
                sqlbuilder.Update(SampleItem_Table.TableName, setfieldarg);

                sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

                sqlbuilder.GetSql();
                var actual = sqlbuilder.ExecuteScalarText();

                Assert.AreEqual(expected, actual);
             
        }
      
    }
}
