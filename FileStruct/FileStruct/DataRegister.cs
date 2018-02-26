using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStruct
{
    class DataRegister
    {
        public List<Tuple<char, object>> DataFields;
        public Int64 pos;
        public Int64 next_reg;
        string Entidad;
        public int keyprim;


        public DataRegister(List<Tuple<char,object>> fields)
        {
            DataFields = fields;
            pos = -1;
            next_reg = -1;
            keyprim = -1;
        }



    }
}
