using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace metoda_transportowa
{
    public partial class Form1 : Form
    {

        public List<Row> polaczenia;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            polaczenia = new List<Row>();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            AddToListView(textBox1, textBox2, listView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddToListView(textBox4, textBox3, listView2);
        }

        private void AddToListView(TextBox t1, TextBox t2, ListView lv)
        {
            int popyt;
            if (!string.IsNullOrWhiteSpace(t1.Text) && !string.IsNullOrWhiteSpace(t2.Text) && int.TryParse(t2.Text, out popyt))
            {
                var list = new List<string>() { t1.Text, t2.Text }.ToArray();
                var newRow = new ListViewItem(list);
                lv.Items.Add(newRow);
                t1.Text = null;
                t2.Text = null;
            }
            else
                throw new Exception("Błędnie wprowadzone dane");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var startLocation = new Point(20, 200);

            var dost = listView1.Items;
            var odb = listView2.Items;

            int counter = 1;

            var dostawcy = new List<ListViewItem>(); 
            var odbiorcy = new List<ListViewItem>();

            foreach (var dostawca in dost)
            {
                dostawcy.Add((ListViewItem)dostawca);

            }
            foreach (var odbiorca in odb)
            {
                odbiorcy.Add((ListViewItem)odbiorca);
            }

            var popytCalk = dostawcy.Sum(x => int.Parse(x.SubItems[1].Text));
            var podazCalk = odbiorcy.Sum(x => int.Parse(x.SubItems[1].Text));

            if (popytCalk > podazCalk)
            {
                string[] row = { "FO", (popytCalk - podazCalk).ToString() };
                odbiorcy.Add(new ListViewItem(row));
            }
            if (podazCalk > popytCalk)
            {
                string[] row = { "FD", (podazCalk - popytCalk).ToString() };
                dostawcy.Add(new ListViewItem(row));
            }


            foreach (var dostawca in dostawcy) {
                foreach (var odbiorca in odbiorcy)
                {
                    var newLabel = new Label();
                    newLabel.Text = $"{dostawca.Text}";
                    newLabel.Location = startLocation;
                    newLabel.Visible = true;
                    this.Controls.Add(newLabel);

                    var newTextBox = new TextBox();
                    newTextBox.Text = null;
                    newTextBox.Width = 20;
                    newTextBox.Location = new Point(startLocation.X + newLabel.Width + 5, startLocation.Y);
                    newTextBox.Visible = true;
                    newTextBox.Name = "odleglosc" + counter.ToString();
                    this.Controls.Add(newTextBox);

                    var newLabel2 = new Label();
                    newLabel2.Text = $"{odbiorca.Text}";
                    newLabel2.Location = new Point(startLocation.X + newLabel.Width + newTextBox.Width + 10, startLocation.Y);
                    newLabel2.Visible = true;
                    this.Controls.Add(newLabel2);

                    startLocation.Y += 25;

                    var dostawcaPopyt = int.Parse(dostawca.SubItems[1].Text);
                    var odbiorcaPodaz = int.Parse(odbiorca.SubItems[1].Text);
                    polaczenia.Add(new Row(counter, dostawca.Text, dostawcaPopyt, odbiorca.Text, odbiorcaPodaz, true));
                    counter++;
                }
            }
        }

        private void Calculate(List<Row> listRow)
        {
            var tempListRow = listRow;

            while (tempListRow.Any(x => x.enabled))
            {
                var findRow = findLowest(tempListRow.Where(x => x.enabled == true).ToList());
                if (findRow.popyt > findRow.podaż)
                {
                    findRow.popyt = findRow.popyt - findRow.podaż;
                    findRow.result = findRow.podaż;
                    findRow.podaż = 0;
                    


                    var toClear = tempListRow.Where(x => x.id != findRow.id && x.odbiorca == findRow.odbiorca).ToList();
                    var toClear2 = tempListRow.Where(x => x.id != findRow.id && x.dostawca == findRow.dostawca).ToList();
                    toClear2.ForEach(x => x.popyt = findRow.popyt);
                    toClear.ForEach(x => x.enabled = false);
                }
                else
                {
                    findRow.podaż = findRow.podaż - findRow.popyt;
                    findRow.result = findRow.popyt;
                    findRow.popyt = 0;

                    var toClear = tempListRow.Where(x => x.id != findRow.id && x.dostawca == findRow.dostawca).ToList();
                    var toClear2 = tempListRow.Where(x => x.id != findRow.id && x.odbiorca == findRow.odbiorca).ToList();
                    toClear2.ForEach(x => x.podaż = findRow.podaż);
                    toClear.ForEach(x => x.enabled = false);


                }
                findRow.enabled = false;
                tempListRow[tempListRow.IndexOf(findRow)] = findRow;
            }

            kosztyCalk.Text = CalculateKoszty(tempListRow).ToString();

        }

        private int CalculateKoszty(List<Row> tempListRow)
        {
            var concerned = tempListRow.Where(x => x.result.HasValue).ToList();
            int result = 0;
            foreach (var a in concerned) {
                result += (a.odleglosc * a.result.Value);
            }
            return result;
        }

        private Row findLowest(List<Row> tempListRow)
        {
            return tempListRow.OrderBy(x => x.odleglosc).First();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= polaczenia.Count; i++)
            {
                if (this.Controls.ContainsKey("odleglosc" + i.ToString()))
                {
                    TextBox txtBox = this.Controls["odleglosc" + i.ToString()] as TextBox;

                    polaczenia[i-1].odleglosc = int.Parse(txtBox.Text);
                }
            }
            Calculate(polaczenia);

            var startLocation = new Point(300, 200);

     
            foreach (var row in polaczenia) {
                var newLabel = new Label();
                var res = row.result.HasValue ? row.result.Value.ToString() : "X";
                newLabel.Text = $"{row.dostawca}, {row.odbiorca}: {res}";
                newLabel.Location = startLocation;
                newLabel.Visible = true;
                this.Controls.Add(newLabel);

                startLocation.Y += 25;


            }
        }
    }
}
