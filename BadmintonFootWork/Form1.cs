using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BadmintonFootWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] options = { "1.0s","1.5s","2.0s", "2.5s", "3.0s", "3.5s", "4.0s" ,"4.5s"};
            comboBox1.Items.AddRange(options);
            comboBox2.Items.AddRange(options);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 获取ComboBox的值
            string selectedValue1 = comboBox1.SelectedItem?.ToString() ?? "3.0s";
            string selectedValue2 = comboBox2.SelectedItem?.ToString() ?? "3.0s";

            // 解析为毫秒
            int interval1 = (int)(double.Parse(selectedValue1.TrimEnd('s')) * 1000);
            int interval2 = (int)(double.Parse(selectedValue2.TrimEnd('s')) * 1000);

            // 打开Form2并传递参数
            Form2 form2 = new Form2(interval1, interval2);
            form2.Show();
        }
    }
}
