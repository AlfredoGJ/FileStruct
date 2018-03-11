using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FileStruct
{
    class DataFile
    {
        string filePath;
        FileStream stream;
        
        public Int64 lenght { get => stream.Length; }

        public DataFile(string filepath)
        {
            filePath = filepath;
            stream = File.Open(filepath,FileMode.OpenOrCreate);
            
        }

        public void WriteRegister(Int64 pos, DataRegister register)

        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Seek((int)pos, SeekOrigin.Begin);

            foreach (DataField field in register.fields)
            {
               // MessageBox.Show(field.value.GetType().ToString());

                if (field.value.GetType() == typeof(Int32))
                    writer.Write((Int32)field.value); 
                else if (field.value.GetType() == typeof(Single))
                    writer.Write((Single)field.value);
                else if (field.value.GetType() == typeof(char[]))
                    writer.Write((char[])field.value);
                else if (field.value.GetType() == typeof(char))
                    writer.Write((char)field.value);
                else if (field.value.GetType() == typeof(long))
                    writer.Write((long)field.value);
            }
            writer.Write(register.next_reg);
            register.pos = pos;


        }

        public DataRegister ReadRegister(Int64 pos,List<Attribute> template )
        {
            BinaryReader reader = new BinaryReader(stream);
            DataRegister register;
            reader.BaseStream.Seek(pos,SeekOrigin.Begin);
            List<DataField> fields = new List<DataField>();

            foreach (Attribute atr in template)
            {
                switch (atr.Tipo)
                {
                    case 'I':
                     
                        fields.Add(new DataField(reader.ReadInt32(), atr.LlavePrim));
                        break;

                    case 'F':
                       
                        fields.Add(new DataField(reader.ReadSingle(), atr.LlavePrim));
                        break;

                    case 'S':
                       
                        fields.Add(new DataField(reader.ReadChars((int)atr.Longitud), atr.LlavePrim));
                        break;

                    case 'C':
                     
                        fields.Add(new DataField(reader.ReadChar(), atr.LlavePrim));

                        break;
                    case 'L':
                       
                        fields.Add(new DataField(reader.ReadInt64(), atr.LlavePrim));
                        break;
                }
            }

            register = new DataRegister(fields);
            register.pos = pos;
            register.next_reg = reader.ReadInt64();
            register.keyprim = template.FindIndex(x => x.LlavePrim==true);
            
            return register;
        }

        public  List<DataRegister> GetAllRegisters(List<Attribute> template, Int64 begin)
        {
            BinaryReader reader = new BinaryReader(stream);
            List<DataRegister> registers= new List<DataRegister>();
            DataRegister register = this.ReadRegister(begin,template);


            registers.Add(register);
            while(register.next_reg!=-1)
            {
                register = this.ReadRegister(register.next_reg, template);
                registers.Add(register);
            }
            return registers;
        }

        public void Close()
        {
            stream.Close();
        }

    }
}
