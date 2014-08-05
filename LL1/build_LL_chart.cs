using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace LL1
{
    class build_LL_chart
    {
        public static void buildchart()
        { 
            int Vn = Utility.regula_left.Count;
            ArrayList Vter = (ArrayList)Utility.terminal.Clone();

            Vter.Remove("@");
            int Vt = Vter.Count;

            int i, j;
            //dt.Columns.Add(new DataColumn("Vn\\Vt"));//表的开头一个元素表明表格内容
            //像(, ) 之类的都是DataColumn的关键字，目前没有找到合适的方法
            //因此，将列头抛弃，使用下面的每一行的数据，开始作为正式数据
            //1)依次填入非终结符数据
            for (i = 0; i <= Vt; i++)
            {                
                Utility.dt.Columns.Add(new DataColumn(""));//列头数据为空
            }
            DataRow dd = Utility.dt.NewRow();
            dd[0] = "";
            for (i = 0; i < Utility.dt.Columns.Count - 1; i++)//第一行数据为非终结符
            {
                dd[i+1] = Vter[i].ToString();
            }
            Utility.dt.Rows.Add(dd);
            //2)对每个终结符进行数据检索
            for (i = 0; i < Vn; i++)
            { 
                //1、将终结符对应的所有规则及其 select集合的索引找到
                string[] ssub = Utility.regula_righ[i].ToString().Split('|');
                int[] indexs = new int[ssub.Length];
                for ( j = 0; j < ssub.Length;j++ )
                {
                    string str = Utility.regula_left[i].ToString() + "->" + ssub[j];
                    indexs[j] = Utility.ll1_chart_rule.IndexOf(str);
                }
                //2、根据select集合中非终结符， 加入对应的规则内容
                string[] rows = new string[Vt + 1];
                rows[0] = Utility.regula_left[i].ToString();//行头放非终结符
                for (j = 0; j < indexs.Length; j++)
                {
                    string[] ssub2 = Utility.ll1_chart_select[indexs[j]].ToString().Split('|');
                    int in_vt;
                    foreach (string str in ssub2)
                    {                        
                        in_vt = Vter.IndexOf(str);
                        rows[in_vt+1] = Utility.ll1_chart_rule[indexs[j]].ToString();//非终结符个数+1 = 列数
                    }
                }
                //3、将每行数据填入表格中
                DataRow dr = Utility.dt.NewRow();
                for (j = 0; j < Utility.dt.Columns.Count; j++)
                {
                    dr[j] = rows[j];
                    if (rows[j] == "" || rows[j] == null)
                        dr[j] = "-";
                }
                Utility.dt.Rows.Add(dr);
            }
                     
        }
    }
}
