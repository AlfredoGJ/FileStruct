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

        private AttributeManager attributeManager;

        private string entityOnAttrEdit;

        public Form1(DictionaryFile file,string projectname)
        {
        
            typeschar.Add('I', typeof(int));
            typeschar.Add('S', typeof(string));
            typeschar.Add('F', typeof(Single));
            typeschar.Add('C', typeof(char));
            typeschar.Add('L', typeof(long));
            typeschar.Add('B', typeof(bool));

            projectName = projectname;
            currentFile = file;
           

            InitializeComponent();
           
            
            
            UpdateEntities();
            UpdateAvailableEntities();
        }

        private void EnableFileManipulation()
        {
            button1.Enabled = true;
            
            textBox1.Enabled = true;
        }

        private void DisableFileManipulation()
        {
            button1.Enabled = false;
           
            textBox1.Enabled = false;
        }


        private void UpdateAvailableEntities()
        {
            Atributos_CBox1.Items.Clear();
            Datos_CBox.Items.Clear();

            List<object[]> Entidades = currentFile.ReadEntidades();
            foreach (object[] o in Entidades)
            {
                
                Atributos_CBox1.Items.Add(o[0]);
                Datos_CBox.Items.Add(o[0]);
            }

        }

        


        private void UpdateEntities()
        {
            Entidades_DGV.Rows.Clear();

           
            List<object[]> Entidades = currentFile.ReadEntidades();
            foreach (object[] o in Entidades)
            {
                Entidades_DGV.Rows.Add(o);
           
            }

        }

        private void AddEntity(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(textBox1.Text)) && (currentFile != null))
            {
                if (currentFile.FindEntidad(textBox1.Text) != -1)
                    MessageBox.Show("Ya existe una una entidad con el nombre " + textBox1.Text + " En el archivo ");
                else
                {
                    Entity E = Entity.CreateNew(textBox1.Text);
                    currentFile.InsertEntidad(E);
                    textBox1.Clear();

                }


                UpdateEntities();
                UpdateAvailableEntities();

            }
        }

       
        private long getEnd(string FileName)
        {
            FileStream Stream = File.OpenRead(FileName);
            return Stream.Length;

        }




        private void DeleteEntity(string entityname)
        {
            currentFile.DeleteEntidad(entityname);

            string Cbox1Text = Atributos_CBox1.Text;
            string DatosCboxText = Datos_CBox.Text;
            UpdateEntities();
            UpdateAvailableEntities();

           

            if ( Cbox1Text== entityname)
            {
                Atributos_DGV.Rows.Clear();
            }
            else
            {
                Atributos_CBox1.Text = Cbox1Text;
            }

            if (DatosCboxText == entityname)
                DataDGV.Columns.Clear();
            else
            {
                Datos_CBox.Text=DatosCboxText;
            }

            

        }


        private void Entidades_DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (currentFile.FindEntidad(Entidades_DGV.Rows[e.RowIndex].Cells[0].Value.ToString()) == -1)
                {
                    currentFile.EditEntidad(EditingCellName, Entidades_DGV.Rows[e.RowIndex].Cells[0].Value.ToString());
                    UpdateEntities();
                }

                else
                {
                    MessageBox.Show("El nombre de la entidad Ya existe");
                    Entidades_DGV.Rows[e.RowIndex].Cells[0].Value = EditingCellName;
                }
            }
          
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // MessageBox.Show("Cell DC " + e.RowIndex.ToString()+","+e.ColumnIndex.ToString());

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EditingCellName = Entidades_DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            // MessageBox.Show("Cell Edit  " + e.RowIndex.ToString() + "," + e.ColumnIndex.ToString()+"Name:"+dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }
        private void CloseFile()
        {
            if (currentFile != null)
                currentFile.Close();
            currentFile = null;
            //CurreentFileName = string.Empty;
            DisableFileManipulation();
            Entidades_DGV.Rows.Clear();
            Atributos_CBox1.Items.Clear();
            Atributos_DGV.Rows.Clear();
            Datos_CBox.Items.Clear();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            entityOnAttrEdit = Atributos_CBox1.SelectedItem.ToString();
            EnableEntidadEditing();

            if (Atributos_CBox1.SelectedItem != null)
            {
               
                Atributos_Btn.Enabled = true;
                Int64 entidadPos = currentFile.FindEntidad(Atributos_CBox1.SelectedItem.ToString());
                Entity E = currentFile.FetchEntidad(entidadPos);


                UpdateAtributos(E);
                //foreach (Attribute a in E.Atributos)
                //{
                //    object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                //    Atributos_DGV.Rows.Add(reg);
                //}




                if (E.ApData != -1)
                    DisableEntidadEditing();

               

            }
        }

        private void DisableEntidadEditing()
        {
            checkBox1.Enabled = false;
            Atributos_TBox.Enabled = false;
            Atributos_CBox2.Enabled = false;
            Atributos_TBox2.Enabled = false;
            Atributos_Btn.Enabled = false;
            Atributos_DGV.Enabled = false;

        }

        private void EnableEntidadEditing()
        {
            checkBox1.Enabled = true;
            Atributos_TBox.Enabled = true;
            Atributos_CBox2.Enabled = true;
            Atributos_TBox2.Enabled = true;
            Atributos_Btn.Enabled = true;
            Atributos_DGV.Enabled = true;

        }

        private void button5_Click(object sender, EventArgs e)
        {


            if ((!string.IsNullOrEmpty(Atributos_TBox.Text)) && (Atributos_CBox2.SelectedItem != null) && (!string.IsNullOrEmpty(Atributos_TBox2.Text)))
            {
                Int64 entidadPos = currentFile.FindEntidad(entityOnAttrEdit);
                Entity E = currentFile.FetchEntidad(entidadPos);
                Attribute AtrAux = new Attribute();

                AtrAux.SetName(Atributos_TBox.Text);
                AtrAux.SetType(Atributos_CBox2.SelectedIndex);
                int.TryParse(Atributos_TBox2.Text, out int longitud);

                if (longitud == 0)
                {
                    MessageBox.Show("La longitud debe de ser un entero distinto de cero");
                    return;
                }
                   
                AtrAux.Longitud = longitud;
                AtrAux.LlavePrim = checkBox1.Checked;
    
                if ((checkBox1.Checked && !E.HasPrimaryKey) || (!checkBox1.Checked))
                {
                    if (E.Atributos.All(a => a.Nombre != AtrAux.Nombre))
                    {
                        currentFile.InsertAtributo(E, AtrAux);

                        UpdateAtributos(E);
                        UpdateEntities();
                        Atributos_TBox.Clear();
                        checkBox1.Checked = false;
                       
                    }
                    else
                        MessageBox.Show("La entidad ya contiene un atributo co el nombre " + Atributos_TBox.Text);
                }
                else
                    MessageBox.Show("Ya existe una llave primaria en la entidad " + E.Nombre);
            }
            else
                MessageBox.Show("Complete todos los campos para el nuevo atributo");
                        
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Atributos_CBox2.SelectedIndex)
            {
                case 0: Atributos_TBox2.Text = "4";
                    Atributos_TBox2.Enabled = false;
                    break;

                case 1:
                    Atributos_TBox2.Text = "4";
                    Atributos_TBox2.Enabled = false;
                    break;

                case 3:
                    Atributos_TBox2.Text = "1";
                    Atributos_TBox2.Enabled = false;
                    break;

                case 4:
                    Atributos_TBox2.Text = "8";
                    Atributos_TBox2.Enabled = false;
                    break;
                case 2:
                    Atributos_TBox2.Clear();
                    Atributos_TBox2.Enabled = true;
                    break;
            }
        }

       

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
                e.Handled = true;
            if (char.IsControl(e.KeyChar))
                e.Handled = false;
        }

        private void DeleteAtrubute(object sender, EventArgs e)
        {
            
            Int64 entidadPos = currentFile.FindEntidad(Atributos_CBox1.Text);
            Entity E = currentFile.FetchEntidad(entidadPos);
            currentFile.DeleteAtributo(E, Atributos_DGV.SelectedRows[0].Cells[1].Value.ToString());

            UpdateAtributos(E);
            UpdateEntities();
            //Atributos_DGV.Rows.Clear();
            //foreach (Attribute a in E.Atributos)
            //{
            //    object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
            //    Atributos_DGV.Rows.Add(reg);
            //}
            
        }

        private void UpdateAtributos(Entity entidad)
        {
            Atributos_DGV.Rows.Clear();
            foreach (Attribute a in entidad.Atributos)
            {
                
                object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
                Atributos_DGV.Rows.Add(reg);
            }
        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EditingCellName = Atributos_DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            // MessageBox.Show(e.RowIndex.ToString()+":"+e.ColumnIndex.ToString()+" "+);
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Int64 entidadPos = currentFile.FindEntidad(Atributos_CBox1.Text);
            Entity E = currentFile.FetchEntidad(entidadPos);
            int colIndex = Atributos_DGV.CurrentCell.ColumnIndex;
            int rowIndex = Atributos_DGV.CurrentCell.RowIndex;
            string atrName = Atributos_DGV.CurrentRow.Cells[1].Value.ToString();
            string newValue = Atributos_DGV.CurrentRow.Cells[rowIndex].Value.ToString();

            if ((colIndex != 0) && (colIndex != 1))
            {
                // User is editing type or length
                if (colIndex == 2)
                {
                    switch (Atributos_DGV.CurrentRow.Cells[2].Value.ToString())
                    {
                        case "Int":
                            Atributos_DGV.CurrentRow.Cells[3].Value = 4;
                            break;
                        case "Float":
                            Atributos_DGV.CurrentRow.Cells[3].Value = 4;
                            break;
                        case "Char":
                            Atributos_DGV.CurrentRow.Cells[3].Value = 1;
                            break;
                        case "Long":
                            Atributos_DGV.CurrentRow.Cells[3].Value = 8;

                            break;


                    }
                    currentFile.EditAtributo(E, atrName, 3, Atributos_DGV.CurrentRow.Cells[3].Value.ToString());

                }
                else
                {
                    if (int.TryParse(Atributos_DGV.CurrentRow.Cells[3].Value.ToString(), out int value))
                    {
                        if (Atributos_DGV.CurrentRow.Cells[2].Value.ToString() == "String")
                            Atributos_DGV.CurrentRow.Cells[3].Value = value;
                        else
                            Atributos_DGV.CurrentRow.Cells[3].Value = EditingCellName;

                    }
                    else
                        MessageBox.Show("Este campo solo admite enteros");

                }



                currentFile.EditAtributo(E, atrName, colIndex, Atributos_DGV.CurrentRow.Cells[colIndex].Value.ToString());
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
                    Atributos_DGV.CurrentRow.Cells[colIndex].Value = EditingCellName;
                    MessageBox.Show("La entidad ya contiene un atributo con el nombre" + atrName);
                }

            }


          


        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {



        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == Atributos_DGV.Columns.Count - 1)
                DeleteAtrubute(new object(),new EventArgs());

            if (Atributos_DGV.CurrentRow != null && e.ColumnIndex == 0)
            {
                // MessageBox.Show(e.RowIndex+"  "+e.ColumnIndex+ dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                Int64 entidadPos = currentFile.FindEntidad(Atributos_CBox1.Text);
                Entity E = currentFile.FetchEntidad(entidadPos);
                string atrName = Atributos_DGV.CurrentRow.Cells[1].Value.ToString();

                if (Atributos_DGV.CurrentRow.Cells[e.ColumnIndex].Value.ToString() == "False")
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
                UpdateAtributos(E);
              
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
            if (e.ColumnIndex == Entidades_DGV.Columns.Count - 1)
            {
                string entityname = Entidades_DGV.SelectedRows[0].Cells[0].Value.ToString();
                Entity E = currentFile.FetchEntidad(entityname);

                if (E.Registers.Count > 0)
                    MessageBox.Show("No puede eliminar entidades que tengan registros");
                else
                    DeleteEntity(entityname);
            }

        }

        private void datos_EntidadesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            
            DataFill(Datos_CBox.Text);

           
        }

        private void UpdateData(Entity entidad)
        {
            DataTable table=(DataTable)DataDGV.DataSource;
            table.RowChanged -= rowChanged;
            table.Clear();

            
            List<DataRegister> registros = entidad.Registers;

            foreach (DataRegister registro in registros)
            {

                DataRow row = table.Rows.Add(registro.Fields());
                row.AcceptChanges();

            }
            DataDGV.DataSource = table;
            table.RowChanged += rowChanged;
        }

        private void datos_DatosEntidadDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row= DataDGV.CurrentRow;
            Entity E = currentFile.FetchEntidad(Datos_CBox.Text);
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
            Entity entidad = currentFile.FetchEntidad(Datos_CBox.Text);
            UpdateData(entidad);
        }

        private void DeleteRegister(int rowindex)
        {
            Entity entidad = currentFile.FetchEntidad(Datos_CBox.Text);
            if (entidad.Registers.Count > rowindex)
            {
                DataGridViewRow row = DataDGV.Rows[rowindex];
                entidad.DeleteRegisterAt((Int64)row.Cells[row.Cells.Count - 3].Value);
                currentFile.WriteEntidad(entidad.Pos, entidad);
                DataTable table = (DataTable)DataDGV.DataSource;

                UpdateData(entidad);
                UpdateEntities();
                MessageBox.Show("El registro se ha eliminado");
            }
            
            
        }

        private void datos_DatosEntidadDataGridView_Leave(object sender, EventArgs e)
        {

           // DumpCurrentFileToScreen();
            //UpdateData();

        }

       

        private void datos_DatosEntidadDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = DataDGV.CurrentRow;
            if (row.Tag == "saved")
                DataDGV.CurrentRow.Tag = "edited"; 
        }


        private void InsertDataRegister()
        {
            Entity entidad = currentFile.FetchEntidad(Datos_CBox.Text);

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

            DataDGV.Columns.Clear();
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
          
       

            DataColumn colApNext = new DataColumn("Apuntador sig registro");
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
            DataGridViewButtonColumn btncolumn = new DataGridViewButtonColumn();
            btncolumn.Text = "Eliminar";
            btncolumn.HeaderText = "Eliminar registro";
            btncolumn.UseColumnTextForButtonValue = true;

            DataDGV.Columns.Add(btncolumn);

            for (int i = 0; i < dataTable.Columns.Count;i++)
            {
                if (dataTable.Columns[i].MaxLength!=-1)
                {
                    DataGridViewTextBoxColumn c = (DataGridViewTextBoxColumn)DataDGV.Columns[i];
                    c.MaxInputLength = dataTable.Columns[i].MaxLength;
                }

                DataDGV.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                
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

            dataTable.AcceptChanges();
            dataTable.RowChanged += rowChanged;
            dataTable.TableNewRow += helo;
            
        }

        private void helo(object sender, DataTableNewRowEventArgs e)
        {
            
        }

        private void rowChanged(object sender, DataRowChangeEventArgs e)
        {
            Entity entidad = currentFile.FetchEntidad(Datos_CBox.Text);
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
                UpdateData(entidad);
            }
 
        }

        private void InsertModifiedRegister(Entity entidad,DataRegister register, DataRowChangeEventArgs e)
        {

            int keyPrimIndex = entidad.Atributos.FindIndex(z => z.LlavePrim == true);
            List<DataRegister> registers = entidad.Registers;
            int indexOldKey = Util.IsKeyHere(registers, new DataField(EditingCellValue, true));
            int indexNewKey = Util.IsKeyHere(registers, register.key);


            // The key has not been modified
            if (indexNewKey == indexOldKey)
            {
                entidad.InsertEditedRegister(register, register.key);
                currentFile.WriteEntidad(entidad.Pos, entidad);
                e.Row.AcceptChanges();
                UpdateEntities();
            }

            else
            {
                if (indexNewKey != -1)
                {
                    MessageBox.Show("Ya existe un registro con esa llave en la entidad");

                }
                else
                {
                    entidad.InsertEditedRegister(register, EditingCellValue);
                    currentFile.WriteEntidad(entidad.Pos, entidad);
                    e.Row.AcceptChanges();
                    UpdateEntities();
                }

               
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
                UpdateEntities();
            }
            else
            {
                MessageBox.Show("Ya existe un registro con esta clave");
                
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void DataDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DataDGV.Columns.Count - 1 && DataDGV.Rows.Count > e.RowIndex )
                DeleteRegister(e.RowIndex);
        }

        private void DataDGV_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

      

       

        private void DataDGV_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            
            DataGridViewCell cell = DataDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.IsInEditMode)
            {
                if (cell.ValueType == typeof(string) || cell.ValueType == typeof(char))
                {
                    cell.Value = cell.Value.ToString().ToLower();
                }
            }
            
        }
    }
}

