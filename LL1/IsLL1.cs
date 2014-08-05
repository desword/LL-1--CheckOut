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

namespace LL1
{
    class IsLL1
    {
        //分离规则，并进行左右部的映射
        public static void spli()
        {
            FileStream fs ;

            fs= new FileStream(Directory.GetCurrentDirectory() + "\\input.txt", FileMode.Open);

                
            StreamReader m_streamReader = new StreamReader(fs);

            string strLine = m_streamReader.ReadLine();
            bool isOne = true;
            while (strLine != null && strLine != "")
            {
                string[] say = Regex.Split(strLine, "->", RegexOptions.IgnoreCase);
                Utility.regula_left.Add(say[0]);
                if (isOne)//初始化开始符号
                {
                    Utility.startCh = say[0];
                    isOne = false;
                } 
                Utility.regula_righ.Add(say[1]);                
                strLine = m_streamReader.ReadLine();
            }
            m_streamReader.Close();
            m_streamReader.Dispose();
            fs.Close();
            fs.Dispose();
        }

        //消除左递归
        public static void eli_left()
        {
            int i;
            for (i = 0; i < Utility.regula_left.Count; i++)
            {
                string[] ssub = Utility.regula_righ[i].ToString().Split('|');
                ArrayList cauleft = new ArrayList();//存放造成左递归的字符串
                ArrayList rest = new ArrayList();//存放剩余的字符串
                char left = Utility.regula_left[i].ToString()[0];//左部的字符
                //扫描每条规则，分析是否存在左递归
                foreach( string ss in ssub )
                {                    
                    //Console.WriteLine(Utility.regula_left[i].ToString() + ","+ss.ToString());
                    //如果左部相等，则将此字符串加入cauleft中
                    if (left == ss.ToString()[0])
                    {
                        cauleft.Add(ss);
                    }
                    else
                    {
                        rest.Add(ss);
                    }
                }

                //存在左递归
                if (cauleft.Count > 0)
                {
                    string new_righ="";
                    foreach (string ss in cauleft)//E+T-- > E'-> +TE'
                    {
                        //Console.WriteLine("casu" + ss);
                        string st = ss.Remove(0, 1);//移除第一个字符，即left字符
                        new_righ += (st + left + "'|");                                            
                    }
                    new_righ += "@";
                    //Console.WriteLine(left + ":" + new_righ);
                    Utility.regula_left.Add(left + "'");//E'
                    Utility.regula_righ.Add(new_righ);//+TE'

                    string new_righ2 = "";
                    foreach (string ss in rest)//T -->  E->TE'
                    {
                        //Console.WriteLine("rest" + ss);
                        new_righ2 += (ss + left + "'|");
                    }
                    new_righ2 = new_righ2.Remove(new_righ2.Length - 1, 1);// ex：如果rest.count==0，文法有问题
                    Utility.regula_righ[i] = new_righ2;                    
                }                
            }
            /*
            for (j = 0; j < Utility.regula_left.Count; j++)
            {
                Console.WriteLine("left:" + Utility.regula_left[j].ToString() + ",righ:" + Utility.regula_righ[j].ToString());
            }*/
            
        }
        
        //计算每个非终结符的First
        public static void cal_First()
        {
            ArrayList ca_fir = (ArrayList)Utility.regula_left.Clone();
            
            //对每个非终结符进行计算
            int i;
            while (ca_fir.Count > 0)
            {
                i = ca_fir.Count - 1;
                string left = ca_fir[i].ToString();
                int index = Utility.regula_left.IndexOf(left);//对于非终结符的索引
                if (Utility.non_first[index].ToString() != left)
                {
                    ca_fir.RemoveAt(ca_fir.Count - 1);
                    continue;//如果当前非终结符已经first过，就跳过到下一个
                }

                Console.WriteLine("" + i + "," + Utility.regula_left.Count + "\n");
                string right = Utility.regula_righ[i].ToString();
                string[] ssub = right.Split('|');
                
                bool f = false;
                foreach (string ss in ssub)//右部元素，判断是否有非终结符,
                {
                    string tmp_left = ss[0] + "";
                    if (ss.Length>1 &&  ss[1] == '\'') tmp_left += "'";//判断是否可能是E'这种两个位置的字符串
                    int new_index = Utility.regula_left.IndexOf(tmp_left);//寻找新的右部的非终结符的索引
                    if (ss[0] >= 'A' && ss[0] <= 'Z'
                        && Utility.non_first[new_index].ToString() == tmp_left)//判断是否已经first过
                    {
                        f = true;
                        left = tmp_left;
                        break;
                    }
                }
                if (!f)//如果没有非终结符，或者终结符都first过
                {
                    string str_ff = "";//存放first集合元素
                    foreach (string ss in ssub)//依次将元素加入到对应的fisrt集合中
                    {
                        if ((ss[0] >= 'a' && ss[0] <= 'z') || ss[0] == '@' || ss[0] == '*'
                            || ss[0] == '/' || ss[0] == '-' || ss[0] == '+' || ss[0] == '(' || ss[0] == ')')//为终结符，或者为空                            
                        {
                            str_ff += (ss[0] + "|");
                            continue;
                        }
                        string left_tmp = ss[0] + "";
                        if (ss.Length>1 && ss[1] == '\'') left_tmp += "'";//判断是否可能是E'这种两个位置的字符串
                        int tmp_index = Utility.regula_left.IndexOf(left_tmp);//寻找右部非终结符的索引
                        str_ff += (Utility.non_first[tmp_index].ToString() + "|");//将对应的first字符串复制进去                        
                    }
                    str_ff = str_ff.Remove(str_ff.Length - 1, 1);//移除最后的 '|'
                    Utility.non_first[index] = str_ff;
                    ca_fir.RemoveAt(ca_fir.Count - 1);//移除最后一个元素
                }
                else//存在终结符，并且没有first过
                {
                    ca_fir.Add(left);//加入新的需要判断的非终结符
                }
            }
            
        }

        //返回：找到的follow集合字符串
        //sear:待follow的字符串； str:扫描的规则；  index:规则中找到的sear索引
        //j：规则对应的索引位  i：sear对应的索引位 ； str_now：找到目前为止的元素集合
        public static string cal_Follow_assi(string sear, string str, int index,int j,int i, string str_now)
        {
            int k = 0;
            if (sear.Length == 2) k = 1;//如果是形如E'的形式，则多往后面判断一个字符
            char ju;
            if (index + 1 + k >= str.Length     //如果访问的位置超过了数组的范围，说明该字符在结尾的位置
                && Utility.non_follow[j].ToString() != Utility.regula_left[j].ToString())//如果对应位置的非终结符follow集已经找完
            {
                string[] ssub3 = Utility.non_follow[j].ToString().Split('|');//分离数据以依次判断
                string smp3 = "";
                foreach (string ss in ssub3)
                {
                    if (str_now.IndexOf(ss) == -1)//如果follow集合中没有该元素,则加入
                        smp3 += (ss + "|");
                }                
                return smp3;
            }
            if (index + 1 + k >= str.Length)//如果到了字符串尾部，但是左部没有准备好，就直接返回
                return "";
            ju = str[index+1+k];
            if( (ju >= 'a' && ju <= 'z' ) || ju == '+'|| ju == '*'
                || ju == '/'|| ju == '-' || ju == '('|| ju == ')')//是终结符
            {
                return ju + "|";
            }
            //右部的后面是非终结符,加入非终结符first集合，除了@的所有非重复元素
            bool istermial = false;
            string str_rightvn = ju +"";
            if( index +1+k+1 < str.Length && str[index +1+k+1] == '\'') str_rightvn += "'";//如果存在形如E'这样的非终结符
            int inin = Utility.regula_left.IndexOf(str_rightvn);
            string[] ssub = Utility.non_first[inin].ToString().Split('|');//first集合元素分离开
            string smp = "";
            foreach (string ss in ssub)
            {
                if (ss != "@" && str_now.IndexOf(ss) == -1)//如果follow集合中没有该元素,则加入
                    smp += (ss + "|");
                if (ss == "@")//存在@符号
                    istermial = true;
            }

            if (istermial 
                && Utility.non_follow[j].ToString() != Utility.regula_left[j].ToString() )//如果存在@符号，且对应非终结符follow搞定了
            {
                string[] ssub2 = Utility.non_follow[j].ToString().Split('|');//分离数据以依次判断
                foreach (string ss in ssub2)
                {
                    if (str_now.IndexOf(ss) == -1)//如果follow集合中没有该元素,则加入
                        smp += (ss + "|");
                }                
            }            
            return smp;
        }

        //计算每个非终结符的Follow
        public static void cal_Follow()
        {
            ArrayList ca_fo = (ArrayList)Utility.regula_left.Clone();

            //1、将终结符号，加入开始符号中
            int i, j;
            int indexs = Utility.regula_left.IndexOf(Utility.startCh);//开始符号的索引
            string strLeft = "#|";
            for (j = 0; j < ca_fo.Count; j++)
            {
                string[] ssub = Utility.regula_righ[j].ToString().Split('|');
                foreach (string st in ssub)
                {
                    int index = st.IndexOf(Utility.startCh);//搜索是否有开始符号
                    int index2 = st.IndexOf(Utility.startCh + "'");
                    if (index != -1 && index2 == -1)
                    {
                        strLeft += cal_Follow_assi(Utility.startCh, st, index, j, indexs,strLeft);
                    }
                }                
            }
            if (strLeft.Length != 0) strLeft = strLeft.Remove(strLeft.Length - 1, 1);
            Utility.non_follow[indexs] = strLeft;

            //2、依次检索其他非终结符
            for (i = 1; i < ca_fo.Count; i++ )//每个非终结符,  开始符号位于第一个索引，0号
            {
                string strVn = "";
                for (j = 0; j < ca_fo.Count; j++ )//遍历每条规则
                {
                    string[] ssub = Utility.regula_righ[j].ToString().Split('|');
                    foreach (string st in ssub)
                    {
                        int index = st.IndexOf( ca_fo[i].ToString() );
                        int index2 = st.IndexOf(ca_fo[i].ToString() + "'");
                        if (index != -1 && index2 == -1)//防止错把带“‘”号的也算作搜索结果
                        {
                            strVn += cal_Follow_assi(ca_fo[i].ToString(), st, index, j, i,strVn);
                        }
                    }
                }
                if (strVn.Length != 0) strVn = strVn.Remove(strVn.Length - 1, 1);
                Utility.non_follow[i] = strVn;
            }
        }
        
        //建立select集合
        //leftid: 规则左部索引值；  sub_rule:子规则
        //return: 对应子规则的select集合
        public static string build_select(int leftid,string sub_rule)
        {
            string tojud = sub_rule[0] + "";
            if (sub_rule.Length > 1 && sub_rule[1] == '\'') tojud += "'";//对于E'情况的判断
            if (tojud == "@")//如果为空串，follow
            {
                return Utility.non_follow[leftid].ToString();
            }
            else if (tojud[0] >= 'A' && tojud[0] <= 'Z')//为非终结符，返回first
            {
                int id = Utility.regula_left.IndexOf(tojud);//找到非终结符索引
                return Utility.non_first[id].ToString();
            }
            else//其他终结符，返回本身
            {
                return tojud[0]+"";
            }
        }

        //比较每条规则的各个select集合
        public static bool cmp_select(ArrayList regu)
        {
            int i;
            int[] flag = new int[Utility.terminal.Count];//对每个终结符进行标记
            //Console.WriteLine("len:" + flag.Length);
            for (i = 0; i < flag.Length; i++ )
            {
                flag[i] = 0;//全部初始化为0
            }
            foreach (string sub_re in regu)
            {
                string[] ssub = sub_re.Split('|');
                foreach (string ss in ssub)
                { 
                    int index = Utility.terminal.IndexOf(ss);
                    if (index == -1) continue;//规避搜索不到非终结符的情况
                    if (flag[index] == 1)
                        return false;
                    flag[index] = 1;
                }
            }
            return true;
        }


        public static string is_ll()
        {
            int i,j;
            string srul = "分析成功！该文法是LL（1）文法。\n-------------[分析过程]-------------\n";
            for (i = 0; i < Utility.regula_righ.Count; i++)//遍历每一条规则
            {
                string[] ssub = Utility.regula_righ[i].ToString().Split('|');
                //if (ssub.Length == 1) continue;//如果只有一个子规则，直接跳过
                ArrayList regu = new ArrayList();
                foreach (string sti in ssub)
                {
                    string sub_selec = build_select(i, sti);
                    //加入子规则，形式 E->E+T
                    Utility.ll1_chart_rule.Add(Utility.regula_left[i].ToString() + "->" + sti);
                    //加入子规则对应的select，形式 a|*|(
                    Utility.ll1_chart_select.Add(sub_selec);
                    regu.Add(sub_selec);
                }
                srul += ( "\n============================\n" 
                    + Utility.regula_left[i].ToString() + " --> " + Utility.regula_righ[i].ToString() 
                        + "\n" );
                for (j = 0; j < ssub.Length; j++)
                {
                    srul += "SELECT( " + ssub[j] + " ) = { " + regu[j].ToString() + " }\n";
                }
                if (!cmp_select(regu))
                {
                    string st = "该文法不是LL(1)文法。\n-------------[出错的规则]-------------\n" 
                        + Utility.regula_left[i].ToString() + "-->" + Utility.regula_righ[i].ToString() + "\n";
                    for (j = 0; j < ssub.Length; j++)
                    {
                        st += "SELECT( " + ssub[j] + " ) = { " + regu[j].ToString() + " }\n";
                    }
                    return st;
                }
            }
            Window1.isll = true;
            return srul;
        }
    }
}
