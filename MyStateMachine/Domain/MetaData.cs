using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MyStateMachine.Domain
{
    public class ColumnMetaData
    {
        public string tableName { get; set; }
        public string colName { get; set; }
        public string colType { get; set; }
        public int maxLen { get; set; }
        public int scale { get; set; }
        public bool nullable { get; set; }

        public ColumnMetaData()
        {
        }

        public ColumnMetaData(Type type, PropertyInfo pi)
        {
            tableName = type.Name;
            colName = pi.Name;
            colType = pi.PropertyType.Name;
        }
    }

    public class ColumnMetaData<T> : ColumnMetaData
    {

        public IList<ColumnMetaData> Columns { get; set; }

        public ColumnMetaData()
        {
            Type[] types = this.GetType().GetGenericArguments();
            ConstructorInfo classConstructor = types[0].GetConstructor(Type.EmptyTypes);
            dynamic datum = classConstructor.Invoke(new object[] { });

            Columns = new List<ColumnMetaData>();

            var props = types[0].GetProperties();

            foreach (PropertyInfo pi in props)
            {
                Columns.Add(new ColumnMetaData(types[0], pi));
            }
        }
    }
}
