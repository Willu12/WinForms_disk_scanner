using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinForms16_lab_304_kurdekb
{
    public partial class Form1 : Form
    {
        //private FileDialog fileDialog = new FileDialog();
        private Form2 dialogBox;
        private string basePath;
        private string selectedPath;
        private List<(string, int, int)> Data_List;
        private bool data_collected = false;
        private Blueprint blueprint;

        public Form1()
        {
            InitializeComponent();
            initFileDialog();
            //backgroundWorker1 = new BackgroundWorker();
            //backgroundWorker1.WorkerSupportsCancellation = true;
            LoadTree();
            //backgroundWorker1.RunWorkerAsync();
        }
        private void InitBlueprint()
        {
            blueprint = new Blueprint(Canvas.Width, Canvas.Height, Data_List);
            Canvas.Image = blueprint.Bitmap;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Select_click(object sender, EventArgs e)
        {
            dialogBox.ShowDialog(this);
        }

       

        void initFileDialog()
        {
            dialogBox = new Form2();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       //https://codehill.com/2013/06/list-drives-and-folders-in-a-treeview-using-c/
        public void LoadTree()
        {
            basePath = "";
            selectedPath = "";
            treeView1.Nodes.Clear();
            string[] drives = Environment.GetLogicalDrives();

            //get a list of the drives

            foreach (string drive in drives)
            {
                DriveInfo di = new DriveInfo(drive);
                int driveImage = 2;

                TreeNode node = new TreeNode(drive.Substring(0, 1), driveImage, driveImage);
                node.Tag = drive;

                if (di.IsReady == true)
                    node.Nodes.Add("...");

                treeView1.Nodes.Add(node);
            }
            toolStripProgressBar1.Value = 0;

            backgroundWorker1.CancelAsync();
            data_collected = false;

            while (backgroundWorker1.IsBusy) Application.DoEvents();
            backgroundWorker1.RunWorkerAsync();
        }

        public void LoadTree(int d)
        {
            basePath = "";
            treeView1.Nodes.Clear();

            string[] drives = Environment.GetLogicalDrives();

            //get a list of the drives


            string drive = drives[d];
            selectedPath = drive;
            
            
            DriveInfo di = new DriveInfo(drive);
            int driveImage = 2;

            TreeNode node = new TreeNode(drive.Substring(0, 1), driveImage, driveImage);
            node.Tag = drive;

            if (di.IsReady == true)
                node.Nodes.Add("...");

            toolStripProgressBar1.Value = 0;
            treeView1.Nodes.Add(node);
            backgroundWorker1.CancelAsync();
            data_collected = false;

            while (backgroundWorker1.IsBusy) Application.DoEvents();
            backgroundWorker1.RunWorkerAsync();


        }

        public void LoadTree(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false) return;
            basePath = System.IO.Path.GetDirectoryName(directoryPath);

            selectedPath = directoryPath;
            treeView1.Nodes.Clear();

            string subdirectory = directoryPath;
            
            
                DirectoryInfo di = new DirectoryInfo(subdirectory);
                int directoryImage = 3;

                TreeNode node = new TreeNode(di.Name, directoryImage, directoryImage);
                node.Tag = subdirectory;

                if (di.GetDirectories().Length > 0) node.Nodes.Add("...");
            toolStripProgressBar1.Value = 0;
                treeView1.Nodes.Add(node);
            backgroundWorker1.CancelAsync();
            data_collected = false;

            while (backgroundWorker1.IsBusy) Application.DoEvents();
            backgroundWorker1.RunWorkerAsync();
        }


        //https://codehill.com/2013/06/list-drives-and-folders-in-a-treeview-using-c/
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

                    //add files of rootdirectory
                    DirectoryInfo rootDir = new DirectoryInfo(e.Node.Tag.ToString());
                   

                    if (rootDir.GetFiles().Count() > 3)
                    {
                        TreeNode fileNode = new TreeNode("<FILE>");
                        e.Node.Nodes.Add(fileNode);

                        foreach (var file in rootDir.GetFiles())
                        {
                            // Add each file as a child of the fileNode
                            TreeNode n = new TreeNode(file.Name, 13, 13);
                            fileNode.Nodes.Add(n);
                        }
                    }
                    else
                    {
                        foreach (var file in rootDir.GetFiles())
                        {
                            TreeNode n = new TreeNode(file.Name, 13, 13);
                            e.Node.Nodes.Add(n);
                        }
                    }

                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        
                        TreeNode node = new TreeNode(di.Name, 0, 1);

                        try
                        {
                            node.Tag = dir;

                            if (di.GetDirectories().Count() > 0)    node.Nodes.Add(null, "...", 0, 0);

                            if(di.GetFiles().Count() > 3)
                            {
                                TreeNode fileNode = new TreeNode("<FILE>");
                                node.Nodes.Add(fileNode);

                                foreach (var file in di.GetFiles())
                                {
                                    TreeNode n = new TreeNode(file.Name, 13, 13);
                                    fileNode.Nodes.Add(n);
                                }
                            }
                            else
                            {
                                foreach (var file in di.GetFiles())
                                {
                                    TreeNode n = new TreeNode(file.Name, 13, 13);
                                    node.Nodes.Add(n);
                                }
                            }
                            

                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            long size;
            int files_count, subdirs_count;
            string downloand_path = e.Node.FullPath;
            string temp_path = Path.Combine(basePath, downloand_path);
            string path = $"{temp_path[0]}:\\";
            if(temp_path.Length > 1) path+= temp_path.Substring(2);

            if(path.Contains("<FILE>"))
            {
                string temp = path.Replace("\\<FILE>", String.Empty);
                path = temp;

            }
            //selectedPath = path;
            //  try
            // {
            FileAttributes attr = System.IO.File.GetAttributes(path);
            //}
            //catch
            //{

            //}
            if (attr == FileAttributes.Directory)
            {
                ( size,  files_count,  subdirs_count) = GetDirectorySize(path);

            }
            else if(downloand_path.Length == 1)
            {
                detailsTextBox.Text = "";
                return;
            }
            else
            {
                (size, files_count, subdirs_count) = GetFileSize(path);
            }


            DirectoryInfo d = new DirectoryInfo(path);


            detailsTextBox.Text = "";
            detailsTextBox.AppendText($"Full Path:            {path}");
            detailsTextBox.AppendText(Environment.NewLine);
            detailsTextBox.AppendText($"Size:                    {size} MB");
            detailsTextBox.AppendText(Environment.NewLine);

            detailsTextBox.AppendText($"Items:                 {files_count + subdirs_count}");
            detailsTextBox.AppendText(Environment.NewLine);

            detailsTextBox.AppendText($"Files:                   {files_count}");
            detailsTextBox.AppendText(Environment.NewLine);

            detailsTextBox.AppendText($"Subdirs:              {subdirs_count}");
            detailsTextBox.AppendText(Environment.NewLine);

            detailsTextBox.AppendText($"Last change:     {d.LastWriteTime}");
        }

        public static (long size, int files_count, int subdirs_count) GetFileSize(string filePath)
        {
            long size = 0;
            int files_count = 0;
            int subdirs_count = 0;

            
             FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == false) return (0, 0, 0);
             size += fileInfo.Length;
            
            
            return (size, files_count, subdirs_count);
        }

        public static (long size,int files_count,int subdirs_count) GetDirectorySize(string directoryPath)
        {
            long size = 0;
            int files_count = 0;
            int subdirs_count = 0;

            string[] files = Directory.GetFiles(directoryPath);
            string[] subdirectories = Directory.GetDirectories(directoryPath);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
                files_count++;
            }
            foreach (string subdirectory in subdirectories)
            {
                subdirs_count++;
                (long csize, int cfiles_count,int csubdirs_count) = GetDirectorySize(subdirectory);
                size += csize;
                files_count += cfiles_count;
                subdirs_count += csubdirs_count;
            }

            return (size,files_count,subdirs_count);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (data_collected) Data_List.Clear();
            backgroundWorker1.ReportProgress(0);
            Dictionary<string, (int,int)> extentionsMap = new Dictionary<string, (int,int)>();

            List<string> DirsList = new List<string>();


            if(selectedPath == "")
            {
                string[] drives = Environment.GetLogicalDrives();
                // DirsList.Add(drivers[])
                
                foreach(string d in drives)
                {
                     DirsList.Add(d);
                }
                
            }
            else
            {
                DirsList.Add(selectedPath);
            }
           // backgroundWorker1.CancelAsync();

            foreach(string path in DirsList)
            {
                CountExtensions(extentionsMap, path,e,100/DirsList.Count);
            }

            //find 9 most values in dict
            List<(string, int, int)> ext_list = new List<(string, int, int)>();
            int extentions_count = extentionsMap.Count > 9 ? 9 : extentionsMap.Count;


            for(int i =0; i<extentions_count; i++)
            {
                string max = extentionsMap.Aggregate((l, r) => l.Value.Item1 > r.Value.Item1 ? l : r).Key;
                ext_list.Add((max, extentionsMap[max].Item1, extentionsMap[max].Item2));
                extentionsMap.Remove(max);

            }

            if(extentionsMap.Count > 0)
            {
                extentions_count++;
                int sum_count = 0;
                int sum_size = 0;

                foreach(var el in extentionsMap)
                {
                    sum_count += el.Value.Item1;
                    sum_size += el.Value.Item2;
                }
                ext_list.Add(("others", sum_count, sum_size));
            }
            Data_List = ext_list;
            data_collected = true;
            InitBlueprint();
           // var max = extentionsMap.Aggregate((l, r) => l.Value.Item2 > r.Value.Item2 ? l : r).Key;
            //detailsTextBox.Text = max;
            

        }
        private  void CountExtensions (Dictionary<string,(int,int)> map,string directoryPath, DoWorkEventArgs e,int curr_dir_size)
        {
            //int size = 0;
            //if (Directory.) return;
            // if (e..l == true) return;
            int value_start = toolStripProgressBar1.Value;
            double sum = 0;

            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                Directory.GetFiles(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            string[] files = Directory.GetFiles(directoryPath);
            string[] subdirectories = Directory.GetDirectories(directoryPath);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string ex = fileInfo.Extension;
                if(ex == "") continue;
                if (map.ContainsKey(ex) == false) map[ex] = (0,0);

                int size = map[ex].Item2 + (int)fileInfo.Length;
                int count = map[ex].Item1 + 1;
                map[ex] = (count, size);
            }

            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            int new_dir_size = curr_dir_size;
            if (subdirectories.Length != 0)
            {
                new_dir_size = curr_dir_size / subdirectories.Length;
            }
            
            foreach (string subdirectory in subdirectories)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                CountExtensions(map, subdirectory,e, new_dir_size);
                // toolStripProgressBar1.Value += (int) new_dir_size;
               
                    value_start += new_dir_size;
                    backgroundWorker1.ReportProgress(value_start);
               
            }
            //work
           // toolStripProgressBar1.Value = value_start + (int)dir_size;
            return;
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                toolStripProgressBar1.Value = 0;
                return;

            }
            toolStripProgressBar1.Value = 100;

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {

            if (data_collected)
            {

                InitBlueprint();

                blueprint.data_list = Data_List;


                if (comboBox1.SelectedIndex == 0)
                {
                    blueprint.Draw_BarCharts();
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    blueprint.Draw_BarLogCharts();

                }


                Canvas.Refresh();


            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Canvas_Click(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

            if(data_collected)
            {

                InitBlueprint();

                blueprint.data_list = Data_List;


                if (comboBox1.SelectedIndex == 0)
                {
                    blueprint.Draw_BarCharts();
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    blueprint.Draw_BarLogCharts();

                }

                
                Canvas.Refresh();

                
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (data_collected)
            {

                InitBlueprint();

                blueprint.data_list = Data_List;


                if (comboBox1.SelectedIndex == 0)
                {
                    blueprint.Draw_BarCharts();
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    blueprint.Draw_BarLogCharts();

                }


                Canvas.Refresh();


            }
        }
    }

   

    
}
