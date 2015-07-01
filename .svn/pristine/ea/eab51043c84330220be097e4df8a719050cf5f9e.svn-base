using System;
using System.Collections.Generic;
using System.Text;
using baseclass;
using System.Data;

namespace sqlbackup.Core
{
    class SysVisitor
    {
        private SysVisitor() { }
        private static SysVisitor visit = null;
        public static SysVisitor Current
        {
            get
            {
                if (visit == null)
                    visit = new SysVisitor();

                return visit;
            }
        }

        public int of_SetMySysSet(string as_type, string as_item, string as_value)
        {
            MyConfig.DbName = "config.db";
            int li_row = 0;
            string ls_sql = "select count(*) from mysysset where itemtype='" + as_type + "' and itemname='" + as_item + "'";
            li_row = MyConfig.ExecuteScalarNum(ls_sql);
            n_create_sql lnv_sql = new n_create_sql("mysysset", MyConfig.DbName);
            lnv_sql.of_AddCol("itemtype", as_type);
            lnv_sql.of_AddCol("itemname", as_item);
            lnv_sql.of_AddCol("itemvalue", as_value);
            int li_rc = 0;
            if (li_row <= 0)
                li_rc = lnv_sql.of_execute();
            if (li_row > 0)
                li_rc = lnv_sql.of_execute("itemtype=@itemtype and itemname=@itemname", "@itemtype=" + as_type, "@itemname=" + as_item);
            return li_rc;
        }

        public string of_GetMySysSet(string as_type, string as_item)
        {
            MyConfig.DbName = "config.db";
            string ls_value = "";
            string ls_sql = "select itemvalue from mysysset where itemtype='" + as_type + "' and itemname='" + as_item + "'";
            ls_value = MyConfig.ExecuteScalar(ls_sql);
            return ls_value;
        }

        public List<string> ReadKeyValues(string section)
        {
            List<string> list = new List<string>();
            string ls_sql = "select itemname,itemvalue from mysysset where itemtype='" + section + "'";

            DataTable ldt = new DataTable();
            ldt = MyConfig.ExecuteDataTable(ls_sql);
            string temp, itemname, itemvalue;
            for (int i = 0; i < ldt.Rows.Count; i++)
            {
                itemname = ldt.Rows[i]["itemName"] == null ? "" : ldt.Rows[i]["itemName"].ToString();
                itemvalue = ldt.Rows[i]["itemvalue"] == null ? "" : ldt.Rows[i]["itemvalue"].ToString();

                temp = itemname + "=" + itemvalue;
                
                list.Add(temp);
            }
            return list;
        }
    }
}
