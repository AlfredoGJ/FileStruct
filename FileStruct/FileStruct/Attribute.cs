using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileStruct
{
    public enum AtributeType {Int,Float,String,Char,Long};
    
    class Attribute
    {
        private string charTypes = "IFSCL";

        public Type type;
        private int tipoIndex;        
        private char[] nombre;
        private char tipo= ' ';
        private Int64 longitud=-1;
        private Int64 posicion=-1;
        private Int64 apSigAtr=-1;
        private bool isKey=false;
        private Entity entidad; 
       
        public long Longitud { get => longitud; set => longitud = value; }
        public long Posicion { get => posicion; set => posicion = value; }
        public long ApNextAtr { get => apSigAtr; set => apSigAtr = value; }
        public bool LlavePrim { get => isKey; set => isKey = value; }
        public char[] NombreAsArray { get => nombre;}
        public string Nombre { get => new string(nombre).Trim();}
        public int TipoNumber { get => tipoIndex;}
        public char Tipo { get => tipo;}

        public Attribute()
        {
            this.nombre = new char[30];
        }

        public Attribute(string name)
        {
            this.nombre = new char[30];
            SetName(name);
        }
        public void SetType(int index)
        {
            tipo = charTypes[index];
            tipoIndex = index;
        }
        public void SetType(char tipo)
        {
            this.tipo = tipo;
            tipoIndex = charTypes.IndexOf(tipo);
        }
        public void SetTypeType(Type type)
        {
            this.type = type;
        }
        public void SetName(string name)
        {
            for (int i = 0; i < 30; i++)
            {
                if (i < name.Count())
                    this.nombre[i] = name[i];
                else
                    this.nombre[i] = ' ';
            }
        }
       

    }

    
}
