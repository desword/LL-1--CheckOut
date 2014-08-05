using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace LL1
{
    class Utility
    {
        public static ArrayList regula_left = new ArrayList();//规则左部,E
        public static ArrayList regula_righ = new ArrayList();//规则右部,E+T|T
        public static ArrayList non_first;//Vn first set
        public static ArrayList non_follow;//Vn follow set
        public static string startCh;//开始符号
        public static ArrayList terminal = new ArrayList();//终结符集合  a|*|(

        public static ArrayList ll1_chart_rule = new ArrayList();//文法的每条规则,E->E+T 形式
        public static ArrayList ll1_chart_select = new ArrayList();//每条规则的select集合, a|*|( 

        public static DataTable dt;//LL1 预测分析表
    }
}
