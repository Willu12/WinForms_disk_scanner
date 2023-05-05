using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinForms16_lab_304_kurdekb
{
    public partial class Form2 : Form
    {
        private int checked_driver;
        public Form2()
        {
            InitializeComponent();
            list_disk();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

       
        void list_disk()
        {
            
            //listViewDrivers.Items.Clear();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            //int i = 0;
            dataGridView1.AllowUserToAddRows = false;

            foreach (DriveInfo drive in allDrives)
            {
                int i = dataGridView1.Rows.Add();
                //if(i++>0)  dataGridView1.Rows.Add();
                double level = (double)drive.AvailableFreeSpace / (double)drive.TotalSize * 100;
                string name = drive.Name.ToString();
                string total = (drive.TotalSize /(int)(Math.Pow(1024,3))).ToString() + "GB";
                string free = (drive.AvailableFreeSpace / (int)(Math.Pow(1024, 3))).ToString() + "GB";
                string percent = String.Format("{0:0.00}%", level);

                Size size = dataGridView1.Rows[i].Cells[3].Size;
                

                dataGridView1.Rows[i].Cells[0].Value = name;
                dataGridView1.Rows[i].Cells[1].Value = total;
                dataGridView1.Rows[i].Cells[2].Value = free;
                dataGridView1.Rows[i].Cells[3].Value= CreateBitmap(size.Width, size.Height, level/100);
                dataGridView1.Rows[i].Cells[4].Value = percent;

                //i++;
               // ListViewItem newItem = new ListViewItem(new[] { name, total,free,percent });

                //listViewDrivers.Items.Add(newItem);
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //dataGridView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //listViewDrivers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                   


                    //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    textBox1.Text =  fbd.SelectedPath.ToString();
                    
                }
            }
        }

        private void InvButton_Click(object sender, EventArgs e)
        {
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void InvButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        public Image CreateBitmap(int x, int y, double percent)
        {
            Bitmap flag = new Bitmap(x, y);
            Graphics flagGraphics = Graphics.FromImage(flag);

            flagGraphics.FillRectangle(Brushes.LightGray, 0, 0, x, y);
            flagGraphics.FillRectangle(Brushes.SlateBlue, 0, 0, (int)(x*percent), y);

            return flag;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 parent = (Form1)this.Owner;

            if(folderButton.Checked == true)
            {
                parent.LoadTree(textBox1.Text);
            }
            else if(InvButton.Checked == true)
            {

                parent.LoadTree(checked_driver);
            }
            else
            {
                parent.LoadTree();
            }

            //odpalanie workera
            


            this.Close();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            string parent = System.IO.Path.GetDirectoryName(textBox1.Text);
            if (parent == null) return;
            // string path = Path.Combine(textBox1.Text, "..");

            if (textBox1.Text == "") return;
            if (parent == "") return;

            else
            {

                textBox1.ForeColor = Color.Black;
                //find all files
                try
                {
                    FileAttributes attr = File.GetAttributes(parent);

                }
                catch (System.IO.FileNotFoundException)
                {
                    textBox1.ForeColor = Color.Red;
                    return;
                }
                // if (attr != FileAttributes.Directory) return;


                //string[] files = Directory.GetFiles(parent);
                string[] subdirectories = Directory.GetDirectories(parent);

                // string[] all_files = new string[files.Length + subdirectories.Length];
                // files.CopyTo(all_files, 0);
                //subdirectories.CopyTo(all_files, files.Length);

                //AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                //foreach (string f in subdirectories)
                //{

                //    col.Add(f);


                //}
                //textBox1.AutoCompleteCustomSource = col;
                foreach (string f in subdirectories)
                {
                    if (f.Contains(textBox1.Text) == true) return;
                }
                textBox1.ForeColor = Color.Red;



            }
        }

        private void listViewDrivers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }

        private void listViewDrivers_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void listViewDrivers_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_MouseCaptureChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            checked_driver = dataGridView1.SelectedRows[0].Index;

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
