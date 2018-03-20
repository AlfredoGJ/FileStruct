using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
namespace FileStruct
{
    class Entity
    {
        private char[] name= new char[30];
        private Int64 posicion = 8;
        private Int64 ap_atributos = -1;
        private Int64 ap_datos = -1;
        private Int64 ap_siguiente = -1;
        private List<Attribute> atributos;
        private bool hasPrimaryKey=false;
        private DictionaryFile dictionary;
        private List<DataRegister> registers;
        private DataFile dataFile;

        public char[] NombreAsArray { get =>name; }
        public string Nombre { get => new string(name).Trim();}
        public Int64 Pos { get => posicion; set => posicion = value; }
        public Int64 ApAtr { get => ap_atributos; set => ap_atributos = value; }
        public Int64 ApData { get => ap_datos; set => ap_datos = value; }
        public Int64 ApNext { get => ap_siguiente; set => ap_siguiente = value; }      
        public Int64 Size { get => (sizeof(Int64) * 4 + 30); }
        public List<Attribute> Atributos { get => atributos; set => atributos = value; }
        public bool HasPrimaryKey { get => hasPrimaryKey; set => hasPrimaryKey = value; }
        public DictionaryFile Dictionary { get => dictionary; set => dictionary = value; }
        public List<DataRegister> Registers { get => registers; }

        public Entity(string name)
        {
            this.SetName(name);
            atributos = new List<Attribute>();
            dataFile= new DataFile(Form1.projectName + "//" + this.Nombre); // This may be provitional
           

        }

        public void SetName(string Name)
        {
            for (int i = 0; i < 30; i++)
            {
                if (i < Name.Count())
                    this.name[i] = Name[i];
                else
                    this.name[i] = ' ';
            }

        }
        
        public static Entity CreateNew(string Name)
        {
            Entity E = new Entity(Name);
            E.SetName(Name);
            E.posicion= -1;
            E.ApAtr = -1;
            E.ApData = -1;
            E.ApNext = -1;
            E.dataFile = new DataFile(Form1.projectName + "//" + E.Nombre); // This may be provitional
            E.UpdateRegisters();
      
            return E;
        }

       

        public void InsertRegister(DataRegister register)
        {

                DataFile dataFile = new DataFile(Form1.projectName + "//" + this.Nombre);
                UpdateRegisters();

                int keyPrimIndex = Atributos.IndexOf(Atributos.Find(x => x.LlavePrim == true));

                if (registers.Count == 0)
                {
                    dataFile.WriteRegister(0, register);
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
                        ap_datos = dataFile.lenght();
                        dataFile.WriteRegister(dataFile.lenght(), register);
                    }
                    else if (index == registers.Count - 1)
                    {
                        registers[(int)index - 1].next_reg = dataFile.lenght();
                        dataFile.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                        dataFile.WriteRegister(dataFile.lenght(), register);
                    }
                    else
                    {
                        register.next_reg = registers[(int)index - 1].next_reg;
                        registers[(int)index - 1].next_reg = dataFile.lenght();
                        dataFile.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                        dataFile.WriteRegister(dataFile.lenght(), register);

                    }
                }
                dataFile.Close();  
            
        }

        public void InsertEditedRegister(DataRegister register, object previouskey)
        {

            DataFile dataFile = new DataFile(Form1.projectName + "//" + this.Nombre);
            if (previouskey == register.key.value)
            { 
                // The key has not changed
                dataFile.WriteRegister(register.pos, register);

            }
            else
            {
                // The key has been changed

                DeleteRegisterAt(register.pos);
                Dictionary.WriteEntidad(this.posicion, this);
                UpdateRegisters();
                registers.Add(register);
                registers = OrderRegistersList(registers, register.key);
                Int64 index = Util.IsKeyHere(registers, register.key);

                if (index == 0)
                {
                    register.next_reg = registers[1].pos;
                    ap_datos = register.pos;
                    dataFile.WriteRegister(register.pos, register);
                }
                else if (index == registers.Count - 1)
                {
                    registers[(int)index - 1].next_reg = register.pos;
                    dataFile.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                    register.next_reg = -1;
                    dataFile.WriteRegister(register.pos, register);
                }
                else
                {
                    register.next_reg = registers[(int)index - 1].next_reg;
                    registers[(int)index - 1].next_reg = register.pos;
                    dataFile.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
                    dataFile.WriteRegister(register.pos, register);
                }
            }


            dataFile.Close();
        }

        public void DeleteRegisterAt(Int64 pos)
        {
            DataFile dataFile = new DataFile(Form1.projectName + "//" + this.Nombre);
            UpdateRegisters();
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


        public void UpdateRegisters()
        {
            // DataFile dataFile = new DataFile(Form1.projectName + "//" + this.Nombre);

            if (ap_datos != -1)
            {
                List<DataRegister> registers= dataFile.GetAllRegisters(this.Atributos, ap_datos);
                this.registers= registers;
            }


            else
            {
                this.registers= new List<DataRegister>();
            }
               
           
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

            DataFile dataFile = new DataFile(Form1.projectName + "//" + this.Nombre);
            dataFile.WriteRegister(register.pos,register);
            dataFile.Close();
        }

        
    }
}
