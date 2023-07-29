using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DemoBigFile.Extensions
{
    public static class GenericToDataTable
    {
        public static DataTable ToDataTable<T>(this List<T> items, List<string> intProperties)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    if (intProperties.Contains(Props[i].Name))
                    {
                        var value = Props[i].GetValue(item);
                        values[i] = value != null ? (int)value : value;
                    }
                    //else if (decimalProperties.Contains(Props[i].Name))
                    //    values[i] = (decimal)Props[i].GetValue(item);
                    else
                        values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
