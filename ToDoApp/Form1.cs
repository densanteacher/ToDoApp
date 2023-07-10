using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoApp
{

    public partial class Form1 : Form
    {

        public int counter = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            //Form2を表示する
            //ここではモーダルダイアログボックスとして表示する
            //オーナーウィンドウにthisを指定する
            f.ShowDialog(this);
            //フォームが必要なくなったところで、Disposeを呼び出す
            f.Dispose();

        }
        public void addControl()
        {
            Panel panel = new Panel();
            panel.Location = new Point(0, counter * 50);
            panel.Size = new Size(20, 20);
            panel.TabIndex = counter;
            panel.Text = counter.ToString();
            this.panelRoot.Controls.Add(panel);
            
            Label title = new Label();
            title.Location = new Point(12, 20 + counter * 50);
            title.Size = new Size(50, 19);
            title.TabIndex = counter;
            title.Text = counter.ToString();

            Label memo = new Label();
            memo.Location = new Point(12, 20 + counter * 50);
            memo.Size = new Size(50, 19);
            memo.TabIndex = counter;
            memo.Text = counter.ToString();

            //パネルにラベルリンクを追加
            this.panelRoot.Controls.Add(title);
            this.panelRoot.Controls.Add(memo);

         /*   Button btn = new Button();
            btn.Location = new Point(80, 20 + counter * 50);
            btn.Text = counter.ToString();
            btn.Name = counter.ToString();
            btn.Size = new System.Drawing.Size(75, 23);

            //パネルにボタンを追加
            this.panel1.Controls.Add(btn);*/

            counter++;

        }
    }

}
