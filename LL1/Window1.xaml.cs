using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;

namespace LL1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public static bool isll;
        public Window1()
        {
            InitializeComponent();
            textBox1.Text = "==================欢迎使用LL1文法分析程序==================\n"
                + "待分析的文法存在于与程序同目录下的'input.txt'文件中，可以直接修改分析。";
            isJX.IsEnabled = false;
        }

        private void init()
        {
            Utility.regula_left.Clear();
            Utility.regula_righ.Clear();
            Utility.terminal.Clear();
            Utility.ll1_chart_rule.Clear();
            Utility.ll1_chart_select.Clear();
            Utility.dt = new DataTable();
            isll = false;
        }

        private void termi_init()//初始化终结符号
        {
            int i;
            string sright = "";
            for (i = 0; i < Utility.regula_righ.Count; i++)
            {
                sright += (Utility.regula_righ[i].ToString() + "|");                
            }
            string[] svt = { "@", "*", "/", "-", "+", "(", ")", };
            foreach (string st in svt)//匹配以上字符，增加到非终结符中
            {
                if (sright.IndexOf(st) != -1)
                {
                    Utility.terminal.Add(st);
                }
            }
            Regex r1 = new Regex("[a-z]");
            if (r1.IsMatch(sright))//如果存在小写字符的终结符，就依次搜索出来
            {
                for (i = 0; i < sright.Length; i++)
                {
                    if (sright[i] >= 'a' && sright[i] <= 'z')
                    {
                        Utility.terminal.Add(sright[i] + "");
                    }
                }
            }
            Utility.terminal.Add("#");//添加终止符号
        }

        private void btn_ana_Click(object sender, RoutedEventArgs e)
        {
            init();
            IsLL1.spli();
            IsLL1.eli_left();
            termi_init();
            Utility.non_first = (ArrayList)Utility.regula_left.Clone();//拷贝数组
            Utility.non_follow = (ArrayList)Utility.regula_left.Clone();
            IsLL1.cal_First();
            IsLL1.cal_Follow();
            string error = IsLL1.is_ll();
            output.Text = error;

            if (isll)
            {
                build_LL_chart.buildchart();
                dataGrid1.ItemsSource = Utility.dt.DefaultView;
                isJX.IsEnabled = true;
            }
            else
            {
                isJX.IsEnabled = false;
                dataGrid1.ItemsSource = null;
            }
                
        }

        private void isJX_Click(object sender, RoutedEventArgs e)
        {
            string gstr = in_string.Text;
            string oou = isJuxing.is_jx(gstr);
            output.Text = oou;
        }


    }
}
