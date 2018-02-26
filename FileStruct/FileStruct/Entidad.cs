using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
           
            return E;
        }

        public void InsertRegister(DataRegister register )
        {
            DataFile file = new DataFile(Form1.projectName + "//" + this.Nombre);
            List<DataRegister> registers = GetRegisters(file);

            if (registers.Count == 0)
                file.WriteRegister(0, register);
            else
            {
                file.WriteRegister(file.lenght,register);
                registers.Add(register);
                switch (register.DataFields[register.keyprim].Item1)
                {
                    case 'I':
                        registers.OrderBy(x=> (int)x.DataFields[x.keyprim].Item2);
                            break;
                    case 'F':
                        registers.OrderBy(x => (float)x.DataFields[x.keyprim].Item2);
                        break;
                    case 'S':
                        registers.OrderBy(x => (string)x.DataFields[x.keyprim].Item2);
                        break;
                    case 'C':
                        registers.OrderBy(x => (char)x.DataFields[x.keyprim].Item2);
                        break;
                    case 'L':
                        registers.OrderBy(x => (long)x.DataFields[x.keyprim].Item2);
                        break;
                }

                Int64 index = registers.IndexOf(register);
                if (index == 0)
                {
                    register.next_reg = registers[1].pos;
                    ap_datos = register.pos;
                    file.WriteRegister(register.pos,register);

                }
                else if (index == registers.Count - 1)
                {
                    registers[(int)index-1].next_reg = register.pos;
                    file.WriteRegister(registers[(int)index - 1].pos, registers[(int)index - 1]);
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

        public List<DataRegister> GetRegisters(DataFile file)
        {
           
            if (ap_datos != -1)
                return file.GetAllRegisters(this.Atributos);

            else
                return new List<DataRegister>();
           
        }



        
    }
}
