using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace System.Data
{
    public static class DataTableExtensions
    {

        public static DataTable AddNoneSelectItem(this DataTable dt, object key, object val)
        {
            try
            {
                DataRow newRow;
                newRow = dt.NewRow();
                newRow[0] = key;
                newRow[1] = val;
                dt.Rows.InsertAt(newRow, 0);
            }
            catch (Exception e)
            {

                throw e;
            }
            return dt;
        }
    }
}