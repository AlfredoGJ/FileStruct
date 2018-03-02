using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStruct
{
    class DataField
    {
        public object value;
        public bool isPrimaryKey;
        public DataField(object value, bool isKey)
        {
            this.value = value;
            this.isPrimaryKey = isKey;
        }
    }
}
