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
            sqlbuilder.ReSet();
        }
        [TestCleanup]
        public void TestCleanup()
        {
            sqlbuilder.Dispose();
        }
        //[TestMethod]
        //public void TestAppendStrSQL()
        //{

        //    string expected = "select * from sampleitem where id=1";
        //    const string sql = "select * from sampleitem";
        //    const string sqlsub2 = "id=1 ";
        //    string actual;
        //    sqlbuilder.ReSet(sql);
        //    sqlbuilder.AppendStrSQL(sqlsub2);
        //    actual = sqlbuilder.GetSql().Trim().ToLower().Replace("  ", " ");

        //    Assert.AreEqual(expected, actual);
        //}


        [TestMethod]
        public void SelectPageTest()
        {
            int expected = 1;

            sqlbuilder.ReSet();

            var count = 0;
            //1.add where
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            //2.execute separate page method
            var result = sqlbuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>
                (
                //total records
                out count,
                //page index
                0,
                //page size
                10,
                //primary key
                SampleItem_Table.Id,
                //display colums
                "*",
                //set table
                SampleItem_Table.TableName,
                //order by
                SampleItem_Table.Id,
                SqlOrderType.Asc
               ); 

            var actual = result.FirstOrDefault().ID;

            Assert.AreEqual(expected, actual);


        }
        [TestMethod]
        public void SelectAllTest()
        {
            sqlbuilder.ReSet();
            //select * from sampleitem where id=1 mapping to SampleItemViewModel by customize map
            var expected = 1;
            //1.set table
            sqlbuilder.Select(SampleItem_Table.TableName);
            //2.add where
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            //3.execute
            var actual = sqlbuilder.Execute<SampleItemViewModel, SampleItemRowMapper>()
                        .FirstOrDefault().ID;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SelectOneFieldTest()
        {
            sqlbuilder.ReSet();
            //select id from sampleitem where id=1 
            var expected = "1";
            sqlbuilder.Select(SampleItem_Table.TableName, new[] { SampleItem_Table.Id });
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            var actual = sqlbuilder.ExecuteScalarText();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SelectTwoFieldsTest()
        {
            sqlbuilder.ReSet();
            //select id,name from sampleitem where id=1  using AutoReflection
            var expected = 1;
            sqlbuilder.Select(SampleItem_Table.TableName, new[] { SampleItem_Table.Id, SampleItem_Table.Name });
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            //  var actual = sqlbuilder.Execute<SampleItem>().FirstOrDefault().Id;
            var actual = sqlbuilder.Execute<SampleItemViewModel>().FirstOrDefault().ID;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void UpdateTest()
        {
            sqlbuilder.ReSet();
            //update sampleitem set name='test' where id = 1
            var expected = "1";
            //1.add set fields
            sqlbuilder.AddPara(SampleItem_Table.Name, "test2");

            sqlbuilder.Update(SampleItem_Table.TableName);
            //2.add where 
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
            //3.execute
            var actual = sqlbuilder.ExecuteScalarText();

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void InsertTest()
        {

            sqlbuilder.ReSet();
           
            var expected = "1";
            //1.set insert fields
            sqlbuilder.AddPara(SampleItem_Table.Id, 2);
            sqlbuilder.AddPara(SampleItem_Table.Name, "test");
            sqlbuilder.AddPara(SampleItem_Table.A, "aaa");
            sqlbuilder.AddPara(SampleItem_Table.B, "bbb");
            sqlbuilder.AddPara(SampleItem_Table.C, "ccc");
            sqlbuilder.AddPara(SampleItem_Table.Date, DateTime.Now);

            //2.execute
            var actual = sqlbuilder.ExecuteInsert(SampleItem_Table.TableName);
            if (expected == actual)
            {
                DeleteTest();
            }
            Assert.AreEqual(expected, actual);


        }
        public void DeleteTest()
        {
            sqlbuilder.ReSet();
            var expected = "1";
            //1.add where
            sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 2);
            //2.execute
            var actual = sqlbuilder.ExecuteDelete(SampleItem_Table.TableName);

            Assert.AreEqual(expected, actual);
        }
    }
}
