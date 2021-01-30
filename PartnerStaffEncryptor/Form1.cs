using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PartnerStaffEncryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("^(\\S+?)\\s+(\\d+)$");

            string[] lines = textBox1.Lines;
            StringBuilder error = new StringBuilder();
            StringBuilder output = new StringBuilder();
            int success_count = 0, error_count = 0;
            foreach(string line in lines)
            {
                //删除可能存在的前后空格
                string person = line.Trim();

                // 跳过空行
                if (String.Empty.Equals(person)) continue;

                Match m = regex.Match(line.Trim());
                if(m.Success)
                {
                    string name = m.Groups[1].Value;
                    string mobile = m.Groups[2].Value;
                    string finger = "";

                    switch (name.Length)
                    {
                        case 1:
                            finger = "*";
                            break;
                        case 2:
                            finger = "*" + name.Substring(1);
                            break;
                        default:
                            finger = name.Substring(0, 1) + "*" + name.Substring(name.Length);
                            break;
                    }
                    finger += " " + CreateMD5(name + mobile);
                    output.AppendLine(finger);
                    success_count++;
                }
                else
                {
                    error_count++;
                    error.AppendLine(person + " 无法识别姓名和电话号码");
                }
            }

            File.WriteAllText("output.txt", output.ToString(), Encoding.UTF8);
            File.WriteAllText("error.txt", error.ToString(), Encoding.UTF8);
            MessageBox.Show(
                String.Format("本次转换共成功处理{0}条记录，{1}条记录无法识别。\n详情请查看目录下的output.txt和error.txt", success_count, error_count), 
                "转换结果", 
                MessageBoxButtons.OK
            );
        }

        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
