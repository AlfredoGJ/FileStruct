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
        public static Dictionary<char, Type> typeschar = new Dictionary<char, Type>();

        private DictionaryFile currentFile;
        private string EditingCellName;
        private object EditingCellValue;
        public static string projectName;
        public Form1()
        {
            typeschar.Add('I', typeof(int));
            typeschar.Add('S', typeof(string));
            typeschar.Add('F', typeof(Single));
            typeschar.Add('C', typeof(char));
            typeschar.Add('L', typeof(long));
            typeschar.Add('B', typeof(bool));


            InitializeComponent();
            button5.Enabled = false;

            dataGridView1.Columns.Add("Nombre", "Nombre");
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

            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 70;
            dataGridView1.Columns[2].Width = 70;
            dataGridView1.Columns[3].Width = 70;
            dataGridView1.Columns[4].Width = 70;

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
            textBox1.Enabled = false;
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

            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + textBox2.Text))
            {
                projectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                //CurreentFileName = ProjectName+ "\\Diccionario.bin";
                currentFile = new DictionaryFile(projectName);
                DumpCurrentFileToScreen();

            }

            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + textBox2.Text);
                projectName = Directory.GetCurrentDirectory() + "\\" + textBox2.Text;
                //CurreentFileName = ProjectName + "\\Diccionario.bin";
                currentFile = new DictionaryFile(projectName);



            }


        }


        private void DumpCurrentFileToScreen()
        {
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            datos_EntidadesComboBox.Items.Clear();
            List<object[]> Entidades = currentFile.ReadEntidades();
            foreach (object[] o in Entidades)
            {
                dataGridView1.Rows.Add(o);
                comboBox1.Items.Add(o[0]);
                datos_EntidadesComboBox.Items.Add(o[0]);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(textBox1.Text)) && (currentFile != null))
            {
                if (currentFile.FindEntidad(textBox1.Text) != -1)
                    MessageBox.Show("Ya existe una una entidad con el nombre " + textBox1.Text + " En el archivo ");
                else
                {
                    Entity E = Entity.CreateNew(textBox1.Text);
                    currentFile.InsertEntidad(E);
                   

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
            FileStream Stream = File.OpenRead(FileName);
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
            if (currentFile != null)
                currentFile.Close();
            currentFile = null;
            //CurreentFileName = string.Empty;
            DisableFileManipulation();
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            dataGridView2.Rows.Clear();
            datos_EntidadesComboBox.Items.Clear();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {

            EnableEntidadEditing();

            if (comboBox1.SelectedItem != null)
            {
                dataGridView2.Rows.Clear();
                button5.Enabled = true;
                Int64 entidadPos = currentFile.FindEntidad(comboBox1.SelectedItem.ToString());
                Entity E = currentFile.FetchEntidad(entidadPos);
                foreach (Attribute a in E.Atributos)
                {
                    object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                    dataGridView2.Rows.Add(reg);
                }
                if (E.ApData != -1)
                    DisableEntidadEditing();

               

            }
        }

        private void DisableEntidadEditing()
        {
            checkBox1.Enabled = false;
            textBox3.Enabled = false;
            comboBox2.Enabled = false;
            textBox4.Enabled = false;
            button5.Enabled = false;
            dataGridView2.Enabled = false;

        }

        private void EnableEntidadEditing()
        {
            checkBox1.Enabled = true;
            textBox3.Enabled = true;
            comboBox2.Enabled = true;
            textBox4.Enabled = true;
            button5.Enabled = true;
            dataGridView2.Enabled = true;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Int64 entidadPos = currentFile.FindEntidad(comboBox1.SelectedItem.ToString());
            Entity E = currentFile.FetchEntidad(entidadPos);
            Attribute AtrAux = new Attribute();


            if ((!string.IsNullOrEmpty(textBox3.Text)) && (comboBox2.SelectedItem != null) && (!string.IsNullOrEmpty(textBox4.Text)))
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
                        currentFile.InsertAtributo(E, AtrAux);
                        dataGridView2.Rows.Clear();
                        foreach (Attribute a in E.Atributos)
                        {
                            object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                            dataGridView2.Rows.Add(reg);
                        }

                    }
                    else
                        MessageBox.Show("La entidad ya contiene un atributo co el nombre " + textBox3.Text);
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
                    textBox4.Text = "1";
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
            if (char.IsControl(e.KeyChar))
                e.Handled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Int64 entidadPos = currentFile.FindEntidad(comboBox1.Text);
            Entity E = currentFile.FetchEntidad(entidadPos);
            currentFile.DeleteAtributo(E, dataGridView2.SelectedRows[0].Cells[1].Value.ToString());
            dataGridView2.Rows.Clear();
            foreach (Attribute a in E.Atributos)
            {
                object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                dataGridView2.Rows.Add(reg);
            }
            
        }
        private void updateAtributos(Entity entidad)
        {
            dataGridView2.Rows.Clear();
            foreach (Attribute a in entidad.Atributos)
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
            Entity E = currentFile.FetchEntidad(entidadPos);
            int colIndex = dataGridView2.CurrentCell.ColumnIndex;
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            string atrName = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            string newValue = dataGridView2.CurrentRow.Cells[rowIndex].Value.ToString();

            if ((colIndex != 0) && (colIndex != 1))
            {
                // User is editing type or length
                if (colIndex == 2)
                {
                    switch (dataGridView2.CurrentRow.Cells[2].Value.ToString())
                    {
                        case "Int":
                            dataGridView2.CurrentRow.Cells[3].Value = 4;
                            break;
                        case "Float":
                            dataGridView2.CurrentRow.Cells[3].Value = 4;
                            break;
                        case "Char":
                            dataGridView2.CurrentRow.Cells[3].Value = 1;
                            break;
                        case "Long":
                            dataGridView2.CurrentRow.Cells[3].Value = 8;

                            break;


                    }
                    currentFile.EditAtributo(E, atrName, 3, dataGridView2.CurrentRow.Cells[3].Value.ToString());

                }
                else
                {
                    if (int.TryParse(dataGridView2.CurrentRow.Cells[3].Value.ToString(), out int value))
                    {
                        if (dataGridView2.CurrentRow.Cells[2].Value.ToString() == "String")
                            dataGridView2.CurrentRow.Cells[3].Value = value;
                        else
                            dataGridView2.CurrentRow.Cells[3].Value = EditingCellName;

                    }
                    else
                        MessageBox.Show("Este campo solo admite enteros");

                }



                currentFile.EditAtributo(E, atrName, colIndex, dataGridView2.CurrentRow.Cells[colIndex].Value.ToString());
            }

            else if (colIndex == 0)
            {


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
                    MessageBox.Show("La entidad ya contiene un atributo con el nombre" + atrName);
                }

            }


          


        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {



        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dataGridView2.CurrentRow != null && e.ColumnIndex == 0)
            {
                // MessageBox.Show(e.RowIndex+"  "+e.ColumnIndex+ dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                Int64 entidadPos = currentFile.FindEntidad(comboBox1.Text);
                Entity E = currentFile.FetchEntidad(entidadPos);
                string atrName = dataGridView2.CurrentRow.Cells[1].Value.ToString();

                if (dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() == "False")
                {
                    // Trying to set keyprim to true
                    if (E.HasPrimaryKey)
                    {
                        MessageBox.Show("Ya existe una llave primaria en la entidad " + E.Nombre);

                    }

                    else
                    {
                        MessageBox.Show("Se establecio " + atrName + " como llave primaria");
                        currentFile.EditAtributo(E, atrName, 0, "True");

                    }

                }
                else
                {

                    // Trying to set keyprim to true
                    currentFile.EditAtributo(E, atrName, 0, "False");

                    MessageBox.Show("Se quito " + atrName + " como llave primaria");

                }
                updateAtributos(E);
              
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void datos_EntidadesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            
            DataFill(datos_EntidadesComboBox.Text);

           
        }

        private void datos_DatosEntidadDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
          
            
        }

        private void UpdateData(Entity entidad)
        {
            
            List<DataRegister> registros = entidad.Registers;
            foreach (DataRegister registro in registros)
            {

                DataDGV.Rows.Add(registro.Fields());
                DataDGV.Rows[DataDGV.Rows.Count - 2].Tag = "saved";
            }
        }

        private void datos_DatosEntidadDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row= DataDGV.CurrentRow;
            Entity E = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            EditingCellValue = row.Cells[E.Atributos.FindIndex(x => x.LlavePrim = true)].Value;
           
           
        }

        private void datos_DatosEntidadDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            //Entidad entidad = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            //UpdateData(entidad);
        }

        private void datos_DatosEntidadDataGridView_Validating(object sender, CancelEventArgs e)
        {
            MessageBox.Show("validatong");
        }

        private void datos_DatosEntidadDataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            Entity entidad = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            UpdateData(entidad);
        }

        private void datos_DatosEntidadDataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Entity entidad = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            DataGridViewRow row = DataDGV.Rows[e.RowIndex];
            entidad.DeleteRegisterAt((Int64)row.Cells[row.Cells.Count-2].Value);
            currentFile.WriteEntidad(entidad.Pos,entidad);
            DumpCurrentFileToScreen();
            MessageBox.Show("El registro se ha eliminado");

        }

        private void datos_DatosEntidadDataGridView_Leave(object sender, EventArgs e)
        {

           // DumpCurrentFileToScreen();
            //UpdateData();

        }

        private void UpdateData()
        {
            Entity E = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            List<DataRegister>registers=E.Registers;
            DataDGV.Rows.Clear();
            foreach (DataRegister reg in registers)
            {
                

                    DataDGV.Rows.Add(reg.Fields());
                    DataDGV.Rows[DataDGV.Rows.Count - 2].Tag = "saved";
            }
           
        }

        private void datos_DatosEntidadDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = DataDGV.CurrentRow;
            if (row.Tag == "saved")
                DataDGV.CurrentRow.Tag = "edited"; 
        }


        private void InsertDataRegister()
        {
            Entity entidad = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);

           // DataDGV.CurrentRow.
            DataRegister register = Util.RowToRegister(DataDGV.CurrentRow, entidad.Atributos);

            if (register != null)
            {
                if ((DataDGV.CurrentRow.Tag == null) || ((string)(DataDGV.CurrentRow.Tag) == "not saved"))
                {
                    if (Util.IsKeyHere(entidad.Registers, register.key) == -1)
                    {
                        DataGridViewRow row = DataDGV.CurrentRow;
                        entidad.InsertRegister(register);
                        currentFile.WriteEntidad(entidad.Pos, entidad);
                        // row.Cells[row.Cells.Count - 2].Value = register.pos;
                        //row.Cells[row.Cells.Count - 1].Value = register.next_reg;
                        row.Tag = "saved";

                    }
                    else
                    {
                        MessageBox.Show("Ya existe un registro con esta clave");
                       // e.Cancel = true;
                    }

                }

                else if ((string)DataDGV.CurrentRow.Tag == "edited")
                {
                    int keyPrimIndex = entidad.Atributos.FindIndex(z => z.LlavePrim == true);
                    List<DataRegister> registers = entidad.Registers;
                    int indexOldKey = Util.IsKeyHere(registers, new DataField(EditingCellValue, true));
                    int indexNewKey = Util.IsKeyHere(registers, register.key);

                    if (indexNewKey != -1)
                    {
                        MessageBox.Show("Ya existe un registro con esa llave en la entidad");
                       // e.Cancel = true;
                    }

                    else if (indexOldKey != -1)
                    {
                        //MessageBox.Show("estas tratando de editar el registro con clave " + EditingCellValue.ToString());

                        entidad.InsertEditedRegister(register, EditingCellValue);
                        currentFile.WriteEntidad(entidad.Pos, entidad);  // <<----- DONT FORGET TO ENABLE THIS
                    }
                    else
                    {
                        MessageBox.Show("Ya existe un registro con esa clave");
                       // e.Cancel = true;
                    }


                }
            }




        }

        private void DataFill(string entityName)
        {
           // DataDGV.Columns.Clear();
            //DataDGV.Rows.Clear();
            Entity entidad = currentFile.FetchEntidad(entityName);
            DataTable dataTable = new DataTable();

            foreach (Attribute atr in entidad.Atributos)
            {
    
                DataColumn column = new DataColumn() ;

                if (atr.Tipo == 'I')
                {
                    column = new DataColumn(atr.Nombre, typeof(int));
                    
                }
                else if (atr.Tipo=='C')
                {
                    column = new DataColumn(atr.Nombre, typeof(char));
                    column.MaxLength = 1;
                }
                else if (atr.Tipo == 'L')
                {
                    column = new DataColumn(atr.Nombre, typeof(long));
                }
                else if (atr.Tipo == 'S')
                {
                    column = new DataColumn(atr.Nombre, typeof(string));
                    column.MaxLength = (int)atr.Longitud;
                    
                }
                else if (atr.Tipo == 'F')
                {
                    column = new DataColumn(atr.Nombre, typeof(float));
                }
                else if (atr.Tipo == 'B')
                {
                    column = new DataColumn(atr.Nombre, typeof(bool));
                }

                dataTable.Columns.Add(column);
                
            }

            DataColumn colPosicion= new DataColumn("Posicion");
            colPosicion.ReadOnly = true;
            colPosicion.DataType = typeof(long);
            colPosicion.DefaultValue = null;
            dataTable.Columns.Add(colPosicion);
          
       

            DataColumn colApNext = new DataColumn("Ap_Siguiente");
            colApNext.ReadOnly = true;
            colApNext.DataType = typeof(long);
            colApNext.DefaultValue = null;
            dataTable.Columns.Add(colApNext);
           
           
            

            

            List<DataRegister> registros = entidad.Registers;

            foreach (DataRegister registro in registros)
            {
                
                DataRow row = dataTable.Rows.Add(registro.Fields());
                
                
               // dataTable.Rows[DataDGV.Rows.Count - 2].Tag = "saved";
                
            }
           
            DataDGV.DataSource = dataTable;

            for (int i = 0; i < dataTable.Columns.Count;i++)
            {
                if (dataTable.Columns[i].MaxLength!=-1)
                {
                    DataGridViewTextBoxColumn c = (DataGridViewTextBoxColumn)DataDGV.Columns[i];
                    c.MaxInputLength = dataTable.Columns[i].MaxLength;
                }
                
            }
            

            if (entidad.HasPrimaryKey == false)
            {
                Label l = new Label();
                l.Text = "No puede insertar datos en esta entidad haste que agregue una llave primaria";
                l.AutoSize = true;
                l.BackColor = Color.Yellow;
                flowLayoutPanel1.Controls.Add(l);
                flowLayoutPanel1.Invalidate();
                DataDGV.Enabled = false;
            }
            else
            {
                flowLayoutPanel1.Controls.Clear();
                DataDGV.Enabled = true;

            }

            dataTable.RowChanged += rowChanged;
            dataTable.TableNewRow += helo;
            dataTable.AcceptChanges();
           
        }

        private void helo(object sender, DataTableNewRowEventArgs e)
        {
            
        }

        private void rowChanged(object sender, DataRowChangeEventArgs e)
        {
            Entity entidad = currentFile.FetchEntidad(datos_EntidadesComboBox.Text);
            DataRegister register = Util.RowToRegister(DataDGV.CurrentRow, entidad.Atributos);
            if (register != null)
            {
                switch (e.Row.RowState)
                {
                    case DataRowState.Modified:
                        InsertModifiedRegister(entidad,register,e);
                        break;

                    case DataRowState.Added:
                        InsertNewRegister(entidad,register,e);
                        break;
                }
            }
            
           
            //InsertDataRegister();
           
           
        }

        private void InsertModifiedRegister(Entity entidad,DataRegister register, DataRowChangeEventArgs e)
        {

            int keyPrimIndex = entidad.Atributos.FindIndex(z => z.LlavePrim == true);
            List<DataRegister> registers = entidad.Registers;
            int indexOldKey = Util.IsKeyHere(registers, new DataField(EditingCellValue, true));
            int indexNewKey = Util.IsKeyHere(registers, register.key);

            if (indexNewKey != -1)
            {
                MessageBox.Show("Ya existe un registro con esa llave en la entidad");
                // e.Cancel = true;
            }

            else if (indexOldKey != -1)
            {
                //MessageBox.Show("estas tratando de editar el registro con clave " + EditingCellValue.ToString());

                entidad.InsertEditedRegister(register, EditingCellValue);
                currentFile.WriteEntidad(entidad.Pos, entidad);
                e.Row.AcceptChanges();
            }
            else
            {
                MessageBox.Show("Ya existe un registro con esa clave");
                // e.Cancel = true;
                
            }
        }

        private void InsertNewRegister(Entity entidad, DataRegister register, DataRowChangeEventArgs e)
        {

            if (Util.IsKeyHere(entidad.Registers, register.key) == -1)
            {
                DataGridViewRow row = DataDGV.CurrentRow;
                entidad.InsertRegister(register);
                currentFile.WriteEntidad(entidad.Pos, entidad);

                e.Row.AcceptChanges();
                
            }
            else
            {
                MessageBox.Show("Ya existe un registro con esta clave");
                
            }
        }
    }
}

