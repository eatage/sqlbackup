using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using baseclass;
using sqlbackup.Core;

namespace sqlbackup
{
    public partial class BackupTime : Form
    {
        private DataTable idt_tb = new DataTable();

        public BackupTime()
        {
            InitializeComponent();
        }
        private void BackupTime_Load(object sender, EventArgs e)
        {
            BindDataGridView();
        }

        private void BindDataGridView()
        {
            MyConfig.DbName = "config.db";
            List<string> aa = SysVisitor.Current.ReadKeyValues("backuptime");
            idt_tb = ListToDataTable(aa);

            dataGridView1.DataSource = idt_tb;
        }


        private void button1_Click(object sender, EventArgs e)//保存
        {
            if (dataGridView1.RowCount > 0)
            {
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)//因启用编辑，底部多一行
                {
                    string ls_time = "", ls_type = "";
                    try
                    {
                        ls_time = (string)dataGridView1[1, i].Value;
                        ls_type = (string)dataGridView1[2, i].Value;
                    }
                    catch { }
                    if (string.IsNullOrEmpty(ls_time) || string.IsNullOrEmpty(ls_type))
                    {
                        MessageBox.Show("备份时间与备份方式不能为空！");
                        return;
                    }
                    if (ls_time.IndexOf(':') == -1)
                    {
                        MessageBox.Show("时间格式不正确");
                        return;
                    }
                    if (ls_time.Length != 5)
                    {
                        MessageBox.Show("时间格式不正确");
                        return;
                    }
                    SysVisitor.Current.of_SetMySysSet("backuptime", ls_time, ls_type);
                }
                BindDataGridView();
                GyShow.of_Show("保存成功!");
            }
        }
        /// <summary>
        /// 将List 形如 time1=10:00,2 转换成DataTable time key way
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private DataTable ListToDataTable(List<string> list)
        {
            System.String[] str = list.ToArray();//List<string>list转成string[]list
            DataTable dt = new DataTable();
            dt.Columns.Add("NO");
            dt.Columns.Add("itemname");
            dt.Columns.Add("itemvalue");
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    string[] bb = list[i].Split('=');
                    DataRow da = dt.NewRow();
                    da[0] = i + 1;
                    da[1] = bb[0].ToString();
                    da[2] = bb[1].ToString();
                    dt.Rows.Add(da);
                }
                catch
                {
                    //ini中存在不合格式的项
                }
            }
            return dt;
        }
        private void button2_Click(object sender, EventArgs e)//取消
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)//删除行
        {
            int row = 0;
            row = dataGridView1.CurrentCell.RowIndex;
            if (dataGridView1.RowCount > 0)
            {
                string ls_time = (string)dataGridView1.Rows[row].Cells[1].Value;
                string ls_type = (string)dataGridView1.Rows[row].Cells[2].Value;
                if (string.IsNullOrEmpty(ls_time) || string.IsNullOrEmpty(ls_type))
                {
                    MessageBox.Show("未选中行或选中了空行！");
                    return;
                }
                string ls_sql = @"delete from mysysset where itemtype='backuptime' and itemname='" + ls_time + "' and itemvalue='" + ls_type + "'";
                int li_row = MyConfig.ExecuteScalarNum(ls_sql);
                BindDataGridView();
                MessageBox.Show("删除成功！");
                return;
            }
        }

    }
}
