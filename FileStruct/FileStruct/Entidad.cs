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
        private Int64  posicion=8;
        private Int64 ap_atributos=-1;
        private Int64 ap_datos=-1;
        private Int64 ap_siguiente=-1;
        public string Nombre { get => new string(nombre).Trim();}
        public Int64 Pos { get => posicion; set => posicion = value; }
        public Int64 ApAtr { get => ap_atributos; set => ap_atributos = value; }
        public Int64 ApData { get => ap_datos; set => ap_datos = value; }
        public Int64 ApNext { get => ap_siguiente; set => ap_siguiente = value; }
        
        public Int64 Size { get => (sizeof(Int64) * 4 + 30); }


        public Entidad()
        {
            nombre= new char[30];
         
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

        public void WriteAt(FileStream Stream,Int64 Pos)
        {
            BinaryWriter Writer = new BinaryWriter(Stream);
            this.posicion = Pos;
            Writer.Seek((int)Pos,SeekOrigin.Begin);
            Writer.Write(nombre.ToArray());
            Writer.Write(posicion);
            Writer.Write(ap_atributos);
            Writer.Write(ap_datos);
            Writer.Write(ap_siguiente);
           
                
            
        }

        public static Entidad FetchAt(FileStream Stream, Int64 Pos)
        {
            Stream.Seek(Pos, SeekOrigin.Begin);
            Entidad E = new Entidad();
            BinaryReader Reader = new BinaryReader(Stream);
                            
            E.nombre = Reader.ReadChars(30);
            E.posicion = Reader.ReadInt64();
            E.ap_atributos = Reader.ReadInt64();
            E.ap_datos = Reader.ReadInt64();
            E.ap_siguiente = Reader.ReadInt64();
        
            
            return E;
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

        
    }
}
