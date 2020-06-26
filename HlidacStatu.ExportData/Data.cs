using System;
using System.Linq;
using System.Collections.Generic;

namespace HlidacStatu.ExportData
{
    public partial class Data
    {

        public Data(IEnumerable<dynamic> rows)
        {
            if (rows == null)
                throw new ArgumentNullException("rows");
            if (rows.Count() == 0)
                throw new ArgumentOutOfRangeException("rows", "Contains no data");
            
            var first = rows.First();


            Dictionary<string, object> values = Values(first);

            this.Columns = new List<Column>();
            foreach (var col in values)
            {
                this.Columns.Add(new Column()
                {
                     Name = col.Key,
                     ValueType = col.Value == null ? null : col.Value.GetType()
                });
            }
            this.Rows = new List<Row>();
            foreach (var row in rows)
            {
                Dictionary<string, object> vals = Values(row);
                if (this.Columns.Any(m => m.ValueType == null))
                {
                    foreach (var col in this.Columns.Where(m=>m.ValueType == null))
                    {
                        if (vals[col.Name] != null)
                            col.ValueType = vals[col.Name].GetType();
                    }
                }

                Row r = new Row();
                foreach (var col in this.Columns)
                {
                    if (vals.ContainsKey(col.Name) == false)
                        r.Values.Add(null);
                    else
                    {
                        object v = vals[col.Name];
                        if (v == null)
                            r.Values.Add(null);
                        else
                        {
                            Type t = vals[col.Name].GetType();
                            r.Values.Add(Convert.ChangeType(vals[col.Name], t));
                        }
                    }
                }
                this.Rows.Add(r);
            }

        }

        private static Dictionary<string, object> Values(dynamic obj)
        {
            if (obj.GetType() == typeof(System.Dynamic.ExpandoObject))
                return ((IDictionary<String, Object>)obj)
                        .ToDictionary(k => k.Key, k => k.Value);
            else
                return ((object)obj)
                        .GetType()
                        .GetProperties()
                        .ToDictionary(p => p.Name, p => p.GetValue(obj));

        }
        public Data(IEnumerable<Column> columns, IEnumerable<Row> rows)
        {
            this.Columns = columns.ToList();
            this.Rows = rows.ToList();
        }

        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; set; }


    }
}
