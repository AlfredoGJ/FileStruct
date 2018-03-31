using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FileStruct
{
    class Util
    {
        public static char[] StringToArrayWSpaces( string original, int desiredlength)
        {
            char[] array = new char[desiredlength];
            for (int i = 0; i < desiredlength; i++)
            {
                if (i < original.Count())
                    array[i] = original[i];
                else
                    array[i] = ' ';
            }

            return array;

        }

        public static int IsKeyHere(List<DataRegister> reglist, DataField key)
        {
            if (reglist.Count == 0)
                return -1;

            if (key.value.GetType() == typeof(Int32))
                return reglist.FindIndex(x => (int)(x.key.value) == (int)key.value);
            else if (key.value.GetType() == typeof(Single))
                return reglist.FindIndex(x => (Single)(x.key.value) == (Single)key.value);
            else if (key.value.GetType() == typeof(char[]))
            {
                char[] a = ((char[])key.value);
                string keystring = new string(((char[])key.value));
                int i = reglist.FindIndex(x => new string((char[])x.key.value) == new string((char[])key.value));
                return i;
            }
                
            else if (key.value.GetType() == typeof(char))
                return reglist.FindIndex(x => (char)(x.key.value) == (char)key.value);
            else if (key.value.GetType() == typeof(long))
                return reglist.FindIndex(x => (long)(x.key.value) == (long)key.value);

           
            return -1;
        }


        /// <summary>
        /// Converts a DataGridView Row into a DataRegister  
        /// </summary>
        /// <param name="row"> The row as a DataGridViewRow object</param>
        /// <param name="atributos"> A list of Atributes in which is based every register field </param>
        /// <returns>A data register object with the values of the row </returns>
        public static DataRegister RowToRegister(DataGridViewRow row,List<Attribute> atributos)
        {
            List<DataField> fields = new List<DataField>();
            for (int i = 0; i < atributos.Count; i++)
            {
                object datavalue = new object();
                DataGridViewCell cell = row.Cells[i];

                if (cell.Value == null)
                {
                    return null;
                }

                if (cell.ValueType == typeof(string))
                {
                    DataGridViewTextBoxColumn c =(DataGridViewTextBoxColumn)cell.OwningColumn;
                    datavalue = Util.StringToArrayWSpaces(cell.Value.ToString(), c.MaxInputLength);
                }
                else
                {
                    datavalue = cell.Value;
                }
              
                fields.Add(new DataField(datavalue, atributos[i].LlavePrim));
            }

            DataRegister register = new DataRegister(fields);

            if (row.Cells[row.Cells.Count - 2].Value != DBNull.Value )
            {
                register.pos = (Int64)row.Cells[row.Cells.Count - 2].Value;
                register.next_reg = (Int64)row.Cells[row.Cells.Count - 1].Value;
            }
               
            return register;
        }
    }
}
