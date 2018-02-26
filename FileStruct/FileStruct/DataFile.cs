using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            stream = File.Open(filepath,FileMode.Open);
            
        }

        public void WriteRegister(Int64 pos, DataRegister register)

        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Seek((int)pos, SeekOrigin.Begin);

            foreach (Tuple<char, object> field in register.DataFields)
            {

                switch (field.Item1)
                {
                    case 'I':
                        writer.Write((Int32) field.Item2);            
                        break;
                    case 'F':
                        writer.Write((float)field.Item2);
                        break;
                    case 'S':
                        writer.Write((string)field.Item2);
                        break;
                    case 'C':
                        writer.Write((char)field.Item2);
                        break;

                    case 'L':
                        writer.Write((long)field.Item2);
                        break;
                }
            }
            writer.Write(register.next_reg);


        }

        public DataRegister ReadRegister(Int64 pos,List<Atributo> template )
        {
            BinaryReader reader = new BinaryReader(stream);
            DataRegister register;
            reader.BaseStream.Seek(pos,SeekOrigin.Begin);
            List<Tuple<char, object>> fields= new List<Tuple<char, object>>();
            foreach (Atributo atr in template)
            {
                switch (atr.Tipo)
                {
                    case 'I':
                        fields.Add(new Tuple<char, object>('I', reader.ReadInt32()));
                        break;

                    case 'F':
                        fields.Add(new Tuple<char, object>('F', reader.ReadSingle()));
                        break;

                    case 'S':
                        fields.Add(new Tuple<char, object>('S', new string(reader.ReadChars((int)atr.Longitud))));
                        break;

                    case 'C':
                        fields.Add(new Tuple<char, object>('C', reader.ReadChar()));
                        break;
                    case 'L':
                        fields.Add(new Tuple<char, object>('L', reader.ReadInt64()));
                        break;
                }
            }

            register = new DataRegister(fields);
            register.pos = pos;
            return register;
        }

        public  List<DataRegister> GetAllRegisters(List<Atributo> template)
        {
            BinaryReader reader = new BinaryReader(stream);
            List<DataRegister> registers= new List<DataRegister>();
            DataRegister register = this.ReadRegister(0,template);


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
