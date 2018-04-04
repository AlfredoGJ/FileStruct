using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileStruct
{
    public partial class Inicio : Form
    {

        private string currentProjectName;

        public Inicio()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                OpenFile();
               

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                Directory.SetCurrentDirectory(Dialog.SelectedPath);
               // Entidades_DGV.Rows.Clear();
                //currentFile = null;
               currentProjectName = string.Empty;
                textBox2.Text = "";

            }
        }
        private void OpenFile()
        {

            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + textBox2.Text))
            {
                currentProjectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                DictionaryFile currentFile = new DictionaryFile(currentProjectName); 

                Form1 form = new Form1(currentFile,currentProjectName,this);
                form.Show();
                this.Hide();

            }

            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + textBox2.Text);
                currentProjectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                DictionaryFile  currentFile = new DictionaryFile(currentProjectName); 
                Form1 form = new Form1(currentFile,currentProjectName,this);
                form.Show();
                this.Hide();
            }
        }


    }
}
