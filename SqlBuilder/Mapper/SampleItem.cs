using ComLib.Entities;
using EasySql.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySql 
{
   
    public class SampleItemViewModel
    { 
        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public string Abc { get; internal set; }
        public DateTime Date { get; internal set; }
    }
    /// <summary>
    /// RowMapper for ActionItem.
    /// </summary>
    public partial class SampleItemRowMapper : ModelMapper<SampleItemViewModel>
    {
        /// <summary>
        /// Creates a new entity from data of a reader.
        /// </summary>
        /// <param name="reader">Reader with data.</param>
        /// <param name="rowNumber">Row number to use.</param>
        /// <returns>Created instance of entity.</returns>
        public override SampleItemViewModel MapRow(IDataReader reader, int rowNumber)
        {
            SampleItemViewModel entity = new SampleItemViewModel();
            entity.ID = reader[SampleItem_Table.Id] == DBNull.Value ? 0 : (int)reader[SampleItem_Table.Id];
            entity.Name = reader[SampleItem_Table.Name] == DBNull.Value ? string.Empty : reader[SampleItem_Table.Name].ToString();
            entity.Abc = reader[SampleItem_Table.A].ToString() + "  " + reader[SampleItem_Table.B].ToString() + " (" + reader[SampleItem_Table.C].ToString() + ")";
            entity.Date = reader[SampleItem_Table.Date] == DBNull.Value ? DateTime.MinValue : (DateTime)reader[SampleItem_Table.Date];

            return entity;
        }
    }
}
