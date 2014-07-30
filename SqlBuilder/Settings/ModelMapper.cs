using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComLib.Entities;
using System.Data;
using AutoMapper;

namespace EasySql
{
    public class ModelMapper<T> : EntityRowMapper<T>, IEntityRowMapper<T>
    {
        public override T MapRow(IDataReader reader, int rowNumber)
        {
            return AutoMapper.Mapper.DynamicMap<T>(reader);
        }
    }
}
