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
        private string CurreentFileName;
        private string EditingCellName;
        private string ProjectName;
        public Form1()
        {
            InitializeComponent();
            
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
            }
           
        }

        private void OpenFile()
        {

            if (Directory.Exists(Directory.GetCurrentDirectory()+"\\"+ textBox2.Text))
            {
                ProjectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                CurreentFileName = ProjectName+ "\\Diccionario.bin";
               
                FileStream Stream = File.OpenRead(CurreentFileName);
                using (BinaryReader Reader = new BinaryReader(Stream))
                {
                    DumpToScreen(Stream);
                    Reader.Close();
                }
            }

            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory()+"\\"+textBox2.Text);
                ProjectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                CurreentFileName = ProjectName + "\\Diccionario.bin";
                FileStream F = File.Create(CurreentFileName);
                    F.Close();
                    F.Dispose();

                    using (BinaryWriter Writer = new BinaryWriter(File.OpenWrite(CurreentFileName)))
                    {
                        Writer.Write((Int64)(-1));
                        Writer.Close();
                    }
             
                
            }


        }
        private void DumpToScreen(FileStream Stream)
        {
            Stream.Seek(0, SeekOrigin.Begin);
            BinaryReader Reader = new BinaryReader(Stream);

            Int64 ApAux = Reader.ReadInt64();
            while (ApAux!=-1)
            {
                Entidad EAux = Entidad.FetchAt(Stream, ApAux);
                object[] reg = {EAux.Nombre,EAux.Pos,EAux.ApAtr,EAux.ApData,EAux.ApNext };
                dataGridView1.Rows.Add(reg);
                ApAux = EAux.ApNext;
              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(CurreentFileName))
            {
                using (FileStream Stream = File.Open(CurreentFileName,FileMode.Open))
                {
                    if (FindEntidad(Stream, textBox1.Text) != -1)
                        MessageBox.Show("Ya existe una una entidad con el nombre " + textBox1.Text + " En el archivo ");
                    else
                        InsertEntidad(Stream,Entidad.CreateNew(textBox1.Text));
                    
                }
                   
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
                CurreentFileName = string.Empty;
                ProjectName = string.Empty;
                textBox2.Text = "";
                
            }
        }
        private long getEnd(string FileName)
        {
            FileStream Stream= File.OpenRead(FileName);
            return Stream.Length;
            
        }
        private void InsertEntidad( FileStream Stream, Entidad entidad )
        {
                Stream.Seek(0, SeekOrigin.Begin);
                using (BinaryReader Reader = new BinaryReader(Stream))
                {
                    Int64 Cab = Reader.ReadInt64();
                    if (Cab == -1)
                    {
                       
                       // entidad.WriteAt(Stream,8);///Position waas 8, now it is stream.lenght

                        //Header update
                        Stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(Stream);                        
                        Writer.Write((Int64)Stream.Length);

                        entidad.WriteAt(Stream, Stream.Length);///Position waas 8, now it is stream.lenght
                }
                else
                    {
                        Int64 APEntidad = Cab;
                        Entidad LastEnt = Entidad.FetchAt(Stream, APEntidad);
                        while (LastEnt.ApNext !=-1)
                        {
                            LastEnt = Entidad.FetchAt(Stream, APEntidad);
                            APEntidad = LastEnt.ApNext;
                        }

                        LastEnt.ApNext = Stream.Length;
                        LastEnt.WriteAt(Stream,LastEnt.Pos);
                        entidad.WriteAt(Stream, Stream.Length);



                    }
                    dataGridView1.Rows.Clear();
                    DumpToScreen(Stream);
                       
                    Reader.Close();
                }
            if (!File.Exists(ProjectName + "\\" + entidad.Nombre))
                File.Create(ProjectName+"\\"+entidad.Nombre).Close();
            
        }
        private Int64 FindEntidad(FileStream Stream,string EntidadName)
        {
            Stream.Seek(0,SeekOrigin.Begin);
            BinaryReader Reader =  new BinaryReader(Stream);
            Int64 AuxPtr = Reader.ReadInt64();
            while (AuxPtr != -1)
            {
                Entidad E = Entidad.FetchAt(Stream,AuxPtr);
                if (E.Nombre == EntidadName)
                    return E.Pos;
                AuxPtr = E.ApNext;
            }
            return AuxPtr;
        }
        private void DeleteEntidad(FileStream Stream, string EntidadName)
        {
            if (File.Exists(ProjectName + "\\" + EntidadName))
            {
                File.Delete(ProjectName + "\\" + EntidadName);
                MessageBox.Show(ProjectName + "\\" + EntidadName);
            }

            else
                MessageBox.Show("El archivo no existe");


            BinaryReader Reader = new BinaryReader(Stream);
            Stream.Seek(0, SeekOrigin.Begin);
            Int64 AuxPtr = Reader.ReadInt64();
            Int64 Cab = AuxPtr;



            while (AuxPtr != -1)
            {
                Entidad E = Entidad.FetchAt(Stream, AuxPtr);

                if (E.ApNext != -1)
                {
                    Entidad ENext = Entidad.FetchAt(Stream, E.ApNext);
                    if (ENext.Nombre == EntidadName)
                    {
                        if (ENext.ApNext != -1)
                        {
                            E.ApNext = ENext.ApNext;
                            E.WriteAt(Stream, E.Pos);
                            return;
                        }
                        else
                        {
                            E.ApNext = -1;
                            E.WriteAt(Stream, E.Pos);
                            return;

                        }


                    }
                    if (E.Nombre == EntidadName)
                    {
                        Stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(Stream);
                        Writer.Write((Int64)E.ApNext);
                        return;
                    }

                    AuxPtr = E.ApNext;
                }
                else
                {
                    if (Cab == E.Pos&&E.Nombre==EntidadName) 
                    {
                        Stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(Stream);
                        Writer.Write((Int64)(-1));
                        return;
                    }
                    
                }
                
                                               
            }
            
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileStream Stream = File.Open(CurreentFileName, FileMode.Open);
            DeleteEntidad(Stream, dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            dataGridView1.Rows.Clear();
            DumpToScreen(Stream);
            Stream.Close();

        }
        private void EditEntidad(string EntidadName,string NewNAme)
        {
            using (FileStream Stream = File.Open(CurreentFileName, FileMode.Open))
            {
                Int64 Pos = FindEntidad(Stream, EntidadName);
                if (Pos != -1)
                {
                    Entidad EAux = Entidad.FetchAt(Stream, Pos);
                    EAux.SetName(NewNAme);
                    EAux.WriteAt(Stream, Pos);
                }
            }
            if (File.Exists(ProjectName+"\\"+EntidadName))
                File.Move(ProjectName + "\\" + EntidadName, ProjectName+ "\\" + NewNAme);

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            FileStream Stream = File.OpenRead(CurreentFileName);
            if (FindEntidad(Stream, dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()) == -1)
            {
                Stream.Close();
                EditEntidad(EditingCellName, dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            }
               
            else
            {
                Stream.Close();
                MessageBox.Show("El nombre de la entidad Ya existe");
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = EditingCellName;

            }
               
            
           // MessageBox.Show("Entidad Changed "+e.RowIndex.ToString());
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
            CurreentFileName = string.Empty;
            DisableFileManipulation();
            dataGridView1.Rows.Clear();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CloseFile();
        }
    }
}

