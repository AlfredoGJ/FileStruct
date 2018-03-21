using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
namespace FileStruct
{
    class AttributeManager
    {
        DictionaryFile dictionary;

        Entity entity;
        DataTable table;
        ComboBox entitySelector;
        DataGridView attributes;
        CheckBox primKey;
        TextBox attributeName;
        ComboBox attributeType;
        Button addButton;

        public AttributeManager(ComboBox selector, DataGridView attributes, CheckBox primkey, TextBox attrname, ComboBox attrtype, Button addbutton, DictionaryFile dictionary)
        {
            entitySelector = selector;
            this.attributes = attributes;
            primKey = primkey;
            attributeName = attrname;
            attributeType = attrtype;
            addButton = addbutton;
            this.dictionary = dictionary;


            entitySelector.SelectedValueChanged += EntityChange;
        }

        private void EntityChange(object sender, EventArgs e)
        {
            entity = dictionary.FetchEntidad(entitySelector.SelectedText);

        }

        private void FillAttributes()
        {
            foreach (Attribute a in entity.Atributos)
            {
                object[] reg = { a.LlavePrim, a.Nombre,a.type, a.Longitud, a.Posicion, a.ApNextAtr };
                attributes.Rows.Add(reg);
            }
        }

      

        public void UpdateEntityAttributes(Entity entity)
        {

           // EnableEntidadEditing();

            //if (entity != null)
            //{
            //    Atributos_DGV.Rows.Clear();
            //    button5.Enabled = true;
               
            //    foreach (Attribute a in E.Atributos)
            //    {
            //        object[] reg = { a.LlavePrim, a.Nombre, dataGridViewComboBoxColumn1.Items[a.TipoNumber], a.Longitud, a.Posicion, a.ApNextAtr };
            //        Atributos_DGV.Rows.Add(reg);
            //    }
            //    if (E.ApData != -1)
            //        DisableEntidadEditing();



            //}
        }


        public void SetEntities(List< Entity> entities )
        {
            entitySelector.DataSource = entities;
        }

    }
}
