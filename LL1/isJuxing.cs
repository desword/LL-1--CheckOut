using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LL1
{
    class isJuxing
    {
        public static ArrayList stack = new ArrayList();//符号栈

        public static string init(string input)
        {
            stack.Clear();
            stack.Add("#");
            stack.Add(Utility.startCh);//符号栈中初始化为“#E”， 开始符号
            if (input[input.Length - 1] != '#')//如果输入串没有以# 结尾，则加上
                input += "#";
            return input;
        }

        public static string geterror(int buzhou,string input, int pos,string yuansu,string top)
        { 
            string fuhaoz = "";
            int i;
            for(i=0 ; i< stack.Count ; i++)
            {
                fuhaoz += (stack[i].ToString());
            }
            input = input.Remove(0, pos );
            string error = buzhou + ":";
            int total = 13;
            int len_fu = fuhaoz.Length;
            int len_in = input.Length;
            int len_yu = yuansu.Length;
            for (i = 0; i < total -5; i++)
                error += (" ");
            error += fuhaoz+top;
            for (i = 0; i < total-6-len_fu; i++)
                error += (" ");
            for (i = 0; i < total - len_in; i++)
                error += (" ");
            error += input;
            for (i = 0; i < total - 5; i++)
                error += (" ");
            error += yuansu + "\n";
            
            return error;
        }

        public static string is_jx(string input)
        {
            string error = "步骤    符号栈     输入串        引用\n";
            int buzhou = 0;
            //1、初始化符号栈， 输入串
            if (input == "" || input == null) return "请输入待分析的字符串";
            input = init(input);

            //2、依据ll1分析表，依次分析输入串
            int pos = 0;//记录输入串当前分析的位置
            int i, j;
            while (stack.Count > 0 || pos < input.Length - 1)
            {
                if (pos == input.Length) pos = input.Length - 1;
                if( Utility.terminal.IndexOf( input[pos]+"" ) == -1  )//提供的输入串中含有非法字符
                {
                    return "error!提供的输入串中含有非法字符 \"" + input[pos] + "\"\n" + error;//error
                }

               
                int in_stack = stack.Count - 1;//符号栈当前分析的字符
                
                string st_tmp = stack[in_stack].ToString();//将符号栈，顶元素出栈
                stack.RemoveAt(in_stack);
                if (st_tmp[0] == input[pos])//栈顶与输入串相同，分析成功，继续读取下一个终结符
                {
                    error += geterror(buzhou++, input, pos, "", st_tmp);
                    pos++;
                    continue;
                }
                for (i = 1; i < Utility.dt.Columns.Count && input[pos] != Utility.dt.Rows[0][i].ToString()[0]; i++) ;
                for (j = 1; j < Utility.dt.Rows.Count && st_tmp != Utility.dt.Rows[j][0].ToString(); j++) ;
                int col = i;//ll1表格中元素的列
                int row = j;//ll1表格中元素的行
                string yuansu;
                if (i == Utility.dt.Columns.Count || j == Utility.dt.Rows.Count)
                    yuansu = "栈顶元素与输入串匹配错误！";
                else yuansu = Utility.dt.Rows[row][col].ToString();

                error += geterror(buzhou++, input, pos, yuansu, st_tmp);
                //error += "[" + i + "][" + j + "]:" + yuansu + "\n";


                if (yuansu == "-" || yuansu == "栈顶元素与输入串匹配错误！") return "抱歉！该输入串不是此文法的句型！\n" + error;
                string ruzhan;
                int po_ys = yuansu.Length - 1;
                while (po_ys>= 0 && yuansu[po_ys] != '>' )//依次将非终结符，逆序入栈
                {
                    ruzhan = yuansu[po_ys] + "";
                    po_ys--;
                    if (ruzhan == "'")//处理E'的情况
                    {
                        ruzhan = yuansu[po_ys] + "'";
                        po_ys--;
                    }
                    if (ruzhan == "@") continue;//处理空字符串@的情况
                    stack.Add(ruzhan);
                }
            }
            return "恭喜！该输入串是此文法的句型！\n" + error;
             
                
        }
    }
}
