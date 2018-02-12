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
    public partial class Form1 : Form
    {
       
        private DBFile currentFile; 
        private string EditingCellName;
        private string projectName;
        public Form1()
        {
            InitializeComponent();
            button5.Enabled = false;
           
            dataGridView1.Columns.Add("Nombre","Nombre");
            dataGridView1.Columns.Add("Posicion", "Posicion");
            dataGridView1.Columns.Add("ap_atributos", "ap_atributos");
            dataGridView1.Columns.Add("ap_datos", "ap_datos");
            dataGridView1.Columns.Add("ap_siguiente_entidad", "ap_siguiente_entidad");
            dataGridView1.MultiSelect = false;
            dataGridView1.Columns[0].ReadOnly = false;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;

            DisableFileManipulation();
        }
        private void EnableFileManipulation()
        {
            button1.Enabled = true;
            button4.Enabled = true;
            textBox1.Enabled = true;
        }

        private void DisableFileManipulation()
        {
            button1.Enabled = false;
            button4.Enabled = false;
            textBox1.Enabled =false;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                OpenFile();
                EnableFileManipulation();
                DumpCurrentFileToScreen();

            }
           
        }

        private void OpenFile()
        {

            if (Directory.Exists(Directory.GetCurrentDirectory()+"\\"+ textBox2.Text))
            {
                projectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                //CurreentFileName = ProjectName+ "\\Diccionario.bin";
                currentFile = new DBFile(projectName);
                DumpCurrentFileToScreen();
                             
            }

            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory()+"\\"+textBox2.Text);
                projectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                //CurreentFileName = ProjectName + "\\Diccionario.bin";
                currentFile = new DBFile(projectName);

             
                
            }


        }

     
        private void DumpCurrentFileToScreen()
        {
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            List<object[]> Entidades = currentFile.ReadEntidades();
            foreach (object[] o in Entidades)
            {
                dataGridView1.Rows.Add(o);
                comboBox1.Items.Add(o[0]);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ( (!string.IsNullOrEmpty(textBox1.Text))&& (currentFile != null) )
            {
                if (currentFile.FindEntidad(textBox1.Text) != -1)
                    MessageBox.Show("Ya existe una una entidad con el nombre " + textBox1.Text + " En el archivo ");
                else
                {
                    currentFile.InsertEntidad(Entidad.CreateNew(textBox1.Text));
                   
                }
                    

                DumpCurrentFileToScreen();
                                                                            
            }                
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                Directory.SetCurrentDirectory(Dialog.SelectedPath);
                DisableFileManipulation();
                dataGridView1.Rows.Clear();
                currentFile = null;
                projectName = string.Empty;
                textBox2.Text = "";
                
            }
        }
        private long getEnd(string FileName)
        {
            FileStream Stream= File.OpenRead(FileName);
            return Stream.Length;
            
        }
        
        
       

        private void button4_Click(object sender, EventArgs e)
        {           
            currentFile.DeleteEntidad(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());           
            dataGridView1.Rows.Clear();
            DumpCurrentFileToScreen();

        }
        

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (currentFile.FindEntidad(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()) == -1)
            {
                currentFile.EditEntidad(EditingCellName, dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                DumpCurrentFileToScreen();
            }
                
            else
            {
                MessageBox.Show("El nombre de la entidad Ya existe");
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = EditingCellName;
            }          
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           // MessageBox.Show("Cell DC " + e.RowIndex.ToString()+","+e.ColumnIndex.ToString());
            
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EditingCellName = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
           // MessageBox.Show("Cell Edit  " + e.RowIndex.ToString() + "," + e.ColumnIndex.ToString()+"Name:"+dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }
        private void CloseFile()
        {
            currentFile = null;
            //CurreentFileName = string.Empty;
            DisableFileManipulation();
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            dataGridView2.Rows.Clear();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                dataGridView2.Rows.Clear();
                button5.Enabled = true;
                Int64 entidadPos = currentFile.FindEntidad(comboBox1.SelectedItem.ToString());
                Entidad E = currentFile.FetchEntidad(entidadPos);
                foreach (Atributo a in E.Atributos)
                {
                    object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                    dataGridView2.Rows.Add(reg);
                }

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
           Int64 entidadPos = currentFile.FindEntidad(comboBox1.SelectedItem.ToString());
            Entidad E=currentFile.FetchEntidad(entidadPos);
            Atributo AtrAux = new Atributo();


            if ((!string.IsNullOrEmpty(textBox3.Text)) && (comboBox2.SelectedItem!=null) && (!string.IsNullOrEmpty(textBox4.Text)))
            {

                AtrAux.SetName(textBox3.Text);
                AtrAux.SetType(comboBox2.SelectedIndex);
                int.TryParse(textBox4.Text, out int longitud);
                AtrAux.Longitud = longitud;
                AtrAux.LlavePrim = checkBox1.Checked;           
               

                if ((checkBox1.Checked && !E.HasPrimaryKey) || (!checkBox1.Checked))
                {
                    if (E.Atributos.All(a => a.Nombre != AtrAux.Nombre))
                    {
                        currentFile.InsertAtributo(E,AtrAux);
                        dataGridView2.Rows.Clear();
                        foreach (Atributo a in E.Atributos)
                        {
                            object[] reg = {a.LlavePrim,a.Nombre,dataGridViewComboBoxColumn1.Items[a.TipoNumber] ,a.Longitud,a.Posicion,a.ApNextAtr};
                            dataGridView2.Rows.Add(reg);
                        }                       
                        
                    }
                    else
                        MessageBox.Show("La entidad ya contiene un atributo co el nombre "+ textBox3.Text);
                }
                else
                    MessageBox.Show("Ya existe una llave primaria en la entidad " + E.Nombre);                
            }
            else
                MessageBox.Show("Complete todos los campos para el nuevo atributo");
       
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0: textBox4.Text = "4";
                    textBox4.Enabled = false;
                    break;

                case 1:
                    textBox4.Text = "4";
                    textBox4.Enabled = false;
                    break;

                case 3:
                    textBox4.Text ="1" ;
                    textBox4.Enabled = false;
                    break;

                case 4:
                    textBox4.Text = "8";
                    textBox4.Enabled = false;
                    break;
                case 2:
                    textBox4.Clear();
                    textBox4.Enabled = true;
                    break;
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
                e.Handled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Int64 entidadPos = currentFile.FindEntidad(comboBox1.Text);
            Entidad E=currentFile.FetchEntidad(entidadPos);
            currentFile.DeleteAtributo(E,dataGridView2.SelectedRows[0].Cells[1].Value.ToString());
            dataGridView2.Rows.Clear();
            foreach (Atributo a in E.Atributos)
            {
                object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                dataGridView2.Rows.Add(reg);
            }
        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EditingCellName = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

           // MessageBox.Show(e.RowIndex.ToString()+":"+e.ColumnIndex.ToString()+" "+);
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Int64 entidadPos = currentFile.FindEntidad(comboBox1.Text);
            Entidad E = currentFile.FetchEntidad(entidadPos);
            int colIndex= dataGridView2.CurrentCell.ColumnIndex;
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            string atrName = dataGridView2.CurrentRow.Cells[1].Value.ToString();

            if ((colIndex != 0) && (colIndex != 1))
            {
                if (colIndex == 2)
                {

                }
                currentFile.EditAtributo(E,atrName,colIndex,dataGridView2.CurrentRow.Cells[colIndex].Value.ToString());
            }
            else if (colIndex == 0)
            {
                if (!E.HasPrimaryKey)
                {
                    currentFile.EditAtributo(E, atrName, colIndex, dataGridView1.CurrentRow.Cells[colIndex].ToString());
                }
                else
                    MessageBox.Show("La entidad ya tiene una llave primaria");
            }
            else if (colIndex == 1)
            {
                if (E.Atributos.FindIndex(atr => atr.Nombre == atrName) == -1)
                {
                    currentFile.EditAtributo(E, EditingCellName, colIndex, atrName);
                }
                else
                {
                    dataGridView2.CurrentRow.Cells[colIndex].Value = EditingCellName;
                    MessageBox.Show("La entidad ya contiene un atributo con el nombre" + EditingCellName);
                }

            }





        }
            
        
    }
}

