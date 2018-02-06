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
       
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns.Add("Nombre","Nombre");
            dataGridView1.Columns.Add("Posicion", "Posicion");
            dataGridView1.Columns.Add("ap_atributos", "ap_atributos");
            dataGridView1.Columns.Add("ap_datos", "ap_datos");
            dataGridView1.Columns.Add("ap_siguiente_entidad", "ap_siguiente_entidad");
            dataGridView1.MultiSelect = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {

            if (Directory.Exists(Directory.GetCurrentDirectory()+"\\"+ textBox2.Text))
            {
                CurreentFileName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text+ "\\Diccionario.bin";
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
                    CurreentFileName = Directory.GetCurrentDirectory() + "\\"+textBox2.Text + "\\Diccionario.bin";
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
                InsertEntidad(Entidad.CreateNew(textBox1.Text), CurreentFileName);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                Directory.SetCurrentDirectory(Dialog.SelectedPath);
            }
        }
        private long getEnd(string FileName)
        {
            FileStream Stream= File.OpenRead(FileName);
            return Stream.Length;
            
        }
        private void InsertEntidad(Entidad entidad , string FileName)
        {
            using (FileStream Stream = File.Open(FileName, FileMode.Open))
            {                
                using (BinaryReader Reader = new BinaryReader(Stream))
                {
                    Int64 Cab = Reader.ReadInt64();
                    if (Cab == -1)
                    {
                       
                        entidad.WriteAt(Stream, 8);

                        //Header update
                        Stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(Stream);                        
                        Writer.Write((Int64)8);
                           
                        
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

            }
        }
        private Int64 FindEntidad(FileStream Stream,string EntidadName)
        {
            BinaryReader Reader =  new BinaryReader(Stream);
            Int64 AuxPtr = Reader.ReadInt64();
            while (AuxPtr != -1)
            {
                Entidad E = Entidad.FetchAt( Stream,AuxPtr);
                if (E.Nombre == EntidadName)
                    return E.Pos;                               
            }
            return AuxPtr;
        }
        private void DeleteEntidad(FileStream Stream, string EntidadName)
        {
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
       
    }
}

