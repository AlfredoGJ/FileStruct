using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
namespace FileStruct
{
    class Entidad
    {


        private char[] nombre;
        private Int64 posicion = 8;
        private Int64 ap_atributos = -1;
        private Int64 ap_datos = -1;
        private Int64 ap_siguiente = -1;
        private List<Atributo> atributos;
        private bool hasPrimaryKey=false;
        private DataFile file;


        public char[] NombreAsArray { get =>nombre; }
        public string Nombre { get => new string(nombre).Trim();}
        public Int64 Pos { get => posicion; set => posicion = value; }
        public Int64 ApAtr { get => ap_atributos; set => ap_atributos = value; }
        public Int64 ApData { get => ap_datos; set => ap_datos = value; }
        public Int64 ApNext { get => ap_siguiente; set => ap_siguiente = value; }
        
        public Int64 Size { get => (sizeof(Int64) * 4 + 30); }
        internal List<Atributo> Atributos { get => atributos; set => atributos = value; }
        public bool HasPrimaryKey { get => hasPrimaryKey; set => hasPrimaryKey = value; }


        public Entidad()
        {
            nombre= new char[30];
            atributos = new List<Atributo>();
            

        }
        public void SetName(string Name)
        {
            for (int i = 0; i < 30; i++)
            {
                if (i < Name.Count())
                    this.nombre[i] = Name[i];
                else
                    this.nombre[i] = ' ';
            }

        }
        
        public static Entidad CreateNew(string Name)
        {

            Entidad E = new Entidad();
            E.SetName(Name);
            E.posicion= -1;
            E.ApAtr = -1;
            E.ApData = -1;
            E.ApNext = -1;
            E.file = new DataFile(Form1.projectName + "//" + E.Nombre);
           
            return E;
        }

        public void InsertRegister(DataRegister register)
        {
                List<DataRegister> registers = GetRegisters();

                int keyPrimIndex = Atributos.IndexOf(Atributos.Find(x => x.LlavePrim == true));

                if (registers.Count == 0)
                {
                    file.WriteRegister(0, register);
                    ApData = 0;
                }
                else
                {
                    registers.Add(register);
                    registers = OrderRegistersList(registers,register.key);
                  
                    Int64 index = registers.IndexOf(register);
                    if (index == 0)
                    {
                        register.next_reg = registers[1].pos;
                        ap_datos = file.lenght;
                        file.WriteRegister(file.lenght, register);
                    }
                    else if (index == registers.Count - 1)
                    {
                        registers[(int)index - 1].next_reg = file.lenght;
                        file.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                        file.WriteRegister(file.lenght, register);
                    }
                    else
                    {
                        register.next_reg = registers[(int)index - 1].next_reg;
                        registers[(int)index - 1].next_reg = file.lenght;
                        file.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                        file.WriteRegister(file.lenght, register);

                    }
                }
                file.Close();  
            
        }

        public void InsertEditedRegister(DataRegister register, object previouskey)
        {
           
            file.WriteRegister(register.pos,register);
            List<DataRegister> registers = GetRegisters();
            //int keyPrimIndex = Atributos.IndexOf(Atributos.Find(x => x.LlavePrim == true));
            registers = OrderRegistersList(registers, register.key);


            if (previouskey == register.key.value)
            { 
                // The key has not changed
                file.WriteRegister(register.pos, register);

            }
            else
            { 
                // Th key has been changed

                Int64 index = Util.IsKeyHere(registers, new DataField(previouskey, true));

                if (index == 0)
                {
                    register.next_reg = registers[1].pos;
                    ap_datos = register.pos;
                    file.WriteRegister(register.pos, register);
                }
                else if (index == registers.Count - 1)
                {
                    registers[(int)index - 1].next_reg = register.pos;
                    file.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                    file.WriteRegister(register.pos, register);
                }
                else
                {
                    register.next_reg = registers[(int)index - 1].next_reg;
                    registers[(int)index - 1].next_reg = register.pos;
                    file.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                    file.WriteRegister(register.pos, register);
                }
            }


            file.Close();
        }

        public void DeleteRegisterAt(Int64 pos)
        {
            List<DataRegister> registers = GetRegisters();
            int index = registers.FindIndex(x => x.pos == pos);

            if (index == 0)
            {

                if (registers.Count == 1)
                    ap_datos = -1;
                else
                    ap_datos = registers[1].pos;
            }
            else if (index == registers.Count - 1)
            {
                registers[registers.Count - 2].next_reg = -1;
                WriteRegister(registers[registers.Count - 2]);
            }
            else
            {
                registers[index - 1].next_reg = registers[index + 1].pos;
                WriteRegister(registers[index - 1]);
            }
                   

        }


        public List<DataRegister> GetRegisters()
        {
          
            if (ap_datos != -1)
                return file.GetAllRegisters(this.Atributos,ap_datos);

            else
                return new List<DataRegister>();
           
        }
        /// <summary>
        /// Takes a list of registers and returns a list Ordered by the value of a key field
        /// </summary>
        /// <param name="registers"> The listo of registers to order</param>
        /// <param name="key">The key field used to order the list</param>
        /// <returns> A ordered version of the list given, if cant be ordered the original list is returned</returns>
        private List<DataRegister> OrderRegistersList(List<DataRegister> registers, DataField key)
        {
            if (key.value.GetType() == typeof(Int32))
                return registers.OrderBy(x => (int)x.key.value).ToList();
            else if (key.value.GetType() == typeof(Single))
                return registers.OrderBy(x => (Single)x.key.value).ToList();
            else if (key.value.GetType() == typeof(char[]))
                return registers.OrderBy(x => new string((char[])x.key.value)).ToList();
            else if (key.value.GetType() == typeof(char))
                return registers.OrderBy(x => (char)x.key.value).ToList();
            else if (key.value.GetType() == typeof(long))
                return registers.OrderBy(x => (long)x.key.value).ToList();

            return registers;
        }

        public void WriteRegister(DataRegister register)
        {
            file.WriteRegister(register.pos,register);
        }

        internal void Katanazo()
        {
            this.file.Close();
        }
    }
}
