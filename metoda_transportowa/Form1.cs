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
            
            var dostawcy = listView1.Items;
            var odbiorcy = listView2.Items;
            ListViewItem tempDostawca;
            ListViewItem tempOdbiorca;

            foreach (var dostawca in dostawcy) {
                foreach (var odbiorca in odbiorcy) {
                    tempDostawca = (ListViewItem)dostawca;
                    tempOdbiorca = (ListViewItem)odbiorca;

                    var newLabel = new Label();
                    newLabel.Text = $"{tempDostawca.Text}";
                    newLabel.Location = startLocation;
                    newLabel.Visible = true;
                    this.Controls.Add(newLabel);
                    
                    var newTextBox = new TextBox();
                    newTextBox.Text = null;
                    newTextBox.Width = 20;
                    newTextBox.Location = new Point(startLocation.X + newLabel.Width + 5, startLocation.Y);
                    newTextBox.Visible = true;
                    this.Controls.Add(newTextBox);

                    var newLabel2 = new Label();
                    newLabel2.Text = $"{tempOdbiorca.Text}";
                    newLabel2.Location = new Point(startLocation.X + newLabel.Width + newTextBox.Width + 10, startLocation.Y);
                    newLabel2.Visible = true;
                    this.Controls.Add(newLabel2);

                    startLocation.Y += 25;

                    var dostawcaPopyt = int.Parse(tempDostawca.SubItems[1].Text);
                    var odbiorcaPodaz = int.Parse(tempOdbiorca.SubItems[1].Text);
                    Random rnd = new Random();
                    polaczenia.Add(new Row(tempDostawca.Text, dostawcaPopyt, tempOdbiorca.Text, odbiorcaPodaz,rnd.Next(1, 6), true));
                }
            }
        }
        private void Calculate(List<Row> listRow) {
            var tempListRow = listRow;

            while (tempListRow.Any(x => x.enabled))
            {
                var findRow = findLowest(tempListRow);
                if (findRow.popyt > findRow.podaż)
                {
                    findRow.popyt = findRow.popyt - findRow.podaż;
                    findRow.result = findRow.podaż;
                    findRow.podaż = 0;
                }
                else
                {
                    findRow.podaż = findRow.podaż - findRow.popyt;
                    findRow.result = findRow.popyt;
                    findRow.popyt = 0;
                }
                //w zależnosci od tego co się wyzerowało to zerować całą "kolumne" lub "wiersz"
                //policz koszty całkowite
            }
        }

        private Row findLowest(List<Row> tempListRow)
        {
            return tempListRow.OrderBy(x => x.odleglosc).First();
        }
    }
}
