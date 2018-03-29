using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStruct
{
    public class DataRegister
    {
        public List<Tuple<char, object>> DataFields;
        public List<DataField> fields;
        public Int64 pos;
        public Int64 next_reg;
        string Entidad;
        public int keyprim;
        public DataField key;
        

        public DataRegister(List<Tuple<char,object>> fields)
        {
            DataFields = fields;
            pos = -1;
            next_reg = -1;
            keyprim = -1;
        }
        public DataRegister(List<DataField> fields)
        {
            this.fields = fields;
            pos = -1;
            next_reg = -1;
            key = null;
            foreach (DataField field in fields)
            {
                if (field.isPrimaryKey)
                    this.key = field;
            }

        }

        public DataRegister(List<Tuple<char, object>> fields, int key)
        {
            DataFields = fields;
            pos = -1;
            next_reg = -1;
            keyprim = key;
        }


        public object[] Fields()
        {
            object[] fields= new object[this.fields.Count()+2];
            for (int i=0;i<this.fields.Count;i++)
            {
                if (this.fields[i].value.GetType()== typeof(char[]))
                    fields[i] = new string((char[])this.fields[i].value);
                else
                    fields[i] = this.fields[i].value;
            }

            fields[fields.Length - 2] = pos;
            fields[fields.Length - 1] = next_reg;
            return fields;
        }


    }
}
