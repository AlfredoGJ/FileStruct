using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FileStruct
{

    class DBFile
    {
        private bool isOpen;
        private Int64 lenght;
        private string filePath;
        private string projectPath;

        public bool IsOpen {get => isOpen;}
        public Int64 Lenght { get => lenght; }
        public string FilePath { get => filePath;}

        public DBFile(string projectDirectory)
        {
            this.projectPath = projectDirectory;
            this.filePath = projectDirectory + "//Diccionario";
            Int64 size;
            using (FileStream Stream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                size = Stream.Length;
                Stream.Close();
            }
                            
            if (size==0)                   
                SetHader(-1);
            
            

        }

        public void SetHader(Int64 value)
        {
            using (FileStream Stream = File.OpenWrite(filePath))
            {
                Stream.Seek(0,SeekOrigin.Begin);
                using (BinaryWriter Writer = new BinaryWriter(Stream))
                {
                    Writer.Write(value);
                    Writer.Close();
                }
                Stream.Close();
            }
        }
        public void InsertEntidad(Entidad entidad)
        {
            using (FileStream Stream = File.Open(filePath, FileMode.Open))
            {
                Stream.Seek(0, SeekOrigin.Begin);
                using (BinaryReader Reader = new BinaryReader(Stream))
                {
                    Int64 Cab = Reader.ReadInt64();
                    if (Cab == -1)
                    {
                        //Header update
                        Stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(Stream);
                        Writer.Write((Int64)Stream.Length);

                        //entidad.WriteAt(Stream, Stream.Length);///Position waas 8, now it is stream.lenght
                        WriteEntidad(Stream.Length,entidad,Stream);
                    }
                    else
                    {
                        Int64 APEntidad = Cab;

                       // Entidad LastEnt = Entidad.FetchAt(Stream, APEntidad);
                        Entidad LastEnt = FetchEntidad(Stream,APEntidad);
                        while (LastEnt.ApNext != -1)
                        {
                            LastEnt = FetchEntidad(Stream, APEntidad);
                            APEntidad = LastEnt.ApNext;
                        }

                        LastEnt.ApNext = Stream.Length;
                        WriteEntidad(LastEnt.Pos, LastEnt,Stream);
                        WriteEntidad(Stream.Length, entidad,Stream);
                        


                       



                    }

                    Reader.Close();
                }
                if (!File.Exists(projectPath + "\\" + entidad.Nombre))
                   File.Create(projectPath + "\\" + entidad.Nombre).Close();

            }


        }

        public Int64 FindEntidad(string EntidadName)
        {
            using (FileStream Stream = File.OpenRead(filePath))
            {
                Stream.Seek(0, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(Stream);
                Int64 AuxPtr = Reader.ReadInt64();
                while (AuxPtr != -1)
                {
                   // Entidad E = Entidad.FetchAt(Stream, AuxPtr);
                    Entidad E = FetchEntidad(Stream,AuxPtr);
                    if (E.Nombre == EntidadName)
                        return E.Pos;
                    AuxPtr = E.ApNext;
                }
                return AuxPtr;
            }
              
        }

        public void DeleteEntidad(string EntidadName)
        {
            using (FileStream Stream = File.Open(filePath, FileMode.Open))
            {
                if (File.Exists(projectPath + "\\" + EntidadName))
                {
                    File.Delete(projectPath + "\\" + EntidadName);
                    MessageBox.Show(projectPath + "\\" + EntidadName);
                }

                else
                    MessageBox.Show("El archivo no existe");


                BinaryReader Reader = new BinaryReader(Stream);
                Stream.Seek(0, SeekOrigin.Begin);
                Int64 AuxPtr = Reader.ReadInt64();
                Int64 Cab = AuxPtr;



                while (AuxPtr != -1)
                {
                   // Entidad E = Entidad.FetchAt(Stream, AuxPtr);
                    Entidad E = FetchEntidad(Stream,AuxPtr);

                    if (E.ApNext != -1)
                    {
                       // Entidad ENext = Entidad.FetchAt(Stream, E.ApNext);
                        Entidad ENext = FetchEntidad(Stream, E.ApNext);
                        if (ENext.Nombre == EntidadName)
                        {
                            if (ENext.ApNext != -1)
                            {
                                E.ApNext = ENext.ApNext;
                               // E.WriteAt(Stream, E.Pos);
                                WriteEntidad(E.Pos,E,Stream);
                                return;
                            }
                            else
                            {
                                E.ApNext = -1;
                                WriteEntidad(E.Pos,E,Stream);
                               // E.WriteAt(Stream, E.Pos);
                                return;

                            }


                        }
                        if (E.Nombre == EntidadName)
                        {
                            Stream.Seek(0, SeekOrigin.Begin);
                            BinaryWriter Writer = new BinaryWriter(Stream);
                            Writer.Write((Int64)E.ApNext);
                            return;
                        }

                        AuxPtr = E.ApNext;
                    }
                    else
                    {
                        if (Cab == E.Pos && E.Nombre == EntidadName)
                        {
                            Stream.Seek(0, SeekOrigin.Begin);
                            BinaryWriter Writer = new BinaryWriter(Stream);
                            Writer.Write((Int64)(-1));
                            return;
                        }

                    }


                }
            }
               
        }

        public void EditEntidad(string EntidadName, string NewNAme)
        {
            Int64 Pos = FindEntidad(EntidadName);
            using (FileStream Stream = File.Open(filePath, FileMode.Open))
            {                
                if (Pos != -1)
                {
                    //Entidad EAux = Entidad.FetchAt(Stream, Pos);
                    Entidad EAux = FetchEntidad(Stream,Pos);
                    EAux.SetName(NewNAme);
                   // EAux.WriteAt(Stream, Pos);
                    WriteEntidad(Pos,EAux,Stream);
                }
            }
            if (File.Exists(projectPath + "\\" + EntidadName))
                File.Move(projectPath + "\\" + EntidadName, projectPath + "\\" + NewNAme);

        }

        public List<object[]> ReadEntidades()
        {
            using (FileStream Stream = File.OpenRead(filePath))
            {
                List<object[]> entidades = new List<object[]>();
                Stream.Seek(0, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(Stream);

                Int64 ApAux = Reader.ReadInt64();
                while (ApAux != -1)
                {
                   //Entidad EAux = Entidad.FetchAt(Stream, ApAux);
                    Entidad EAux = FetchEntidad(Stream,ApAux);
                    object[] reg = { EAux.Nombre, EAux.Pos, EAux.ApAtr, EAux.ApData, EAux.ApNext };
                    entidades.Add(reg);
                    ApAux = EAux.ApNext;

                }
                return entidades;
            }
               
           
        }
        private  Entidad FetchEntidad(FileStream Stream,Int64 Pos)
        {
            Entidad E = new Entidad();           
            Stream.Seek(Pos, SeekOrigin.Begin);                
            BinaryReader Reader = new BinaryReader(Stream);
            E.SetName( new string(Reader.ReadChars(30)));
            E.Pos = Reader.ReadInt64();
            E.ApAtr = Reader.ReadInt64();
            E.ApData = Reader.ReadInt64();
            E.ApNext = Reader.ReadInt64();
            FindAtributos(Stream, E);

            return E;
        }

        public Entidad FetchEntidad(Int64 Pos)
        {
            Entidad E = new Entidad();
            using (FileStream Stream = File.OpenRead(filePath))
            {
                Stream.Seek(Pos, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(Stream);
                E.SetName(new string( Reader.ReadChars(30)));
                E.Pos = Reader.ReadInt64();
                E.ApAtr = Reader.ReadInt64();
                E.ApData = Reader.ReadInt64();
                E.ApNext = Reader.ReadInt64();
                FindAtributos(Stream,E);
            }
                           
            return E;
        }

        private void WriteEntidad(Int64 Pos, Entidad entidad, FileStream stream)
        {           
            BinaryWriter Writer = new BinaryWriter(stream);
            entidad.Pos = Pos;
            Writer.Seek((int)Pos, SeekOrigin.Begin);

            Writer.Write(entidad.NombreAsArray);
            Writer.Write(entidad.Pos);
            Writer.Write(entidad.ApAtr);
            Writer.Write(entidad.ApData);
            Writer.Write(entidad.ApNext);           
        }

        private void WriteAtributo(FileStream Stream, Int64 pos, Atributo atributo)
        {

            BinaryWriter Writer = new BinaryWriter(Stream);
            atributo.Posicion = pos;
            Writer.Seek((int)pos, SeekOrigin.Begin);

            Writer.Write(atributo.NombreAsArray);
            Writer.Write(atributo.Tipo);
            Writer.Write(atributo.Longitud);
            Writer.Write(atributo.Posicion);
            Writer.Write(atributo.ApNextAtr);
            Writer.Write(atributo.LlavePrim);
        }

        private Atributo FetchAttibuto(FileStream Stream, Int64 Pos)
        {
            Stream.Seek(Pos, SeekOrigin.Begin);
            Atributo auxAtr = new Atributo();
            BinaryReader Reader = new BinaryReader(Stream);

            auxAtr.SetName(new string(Reader.ReadChars(30)));
            auxAtr.SetType(Reader.ReadChar());
            auxAtr.Longitud = Reader.ReadInt64();
            auxAtr.Posicion = Reader.ReadInt64();
            auxAtr.ApNextAtr = Reader.ReadInt64();
            auxAtr.LlavePrim = Reader.ReadBoolean();

            return auxAtr;
        }
        public void InsertAtributo(Entidad entidad, Atributo atributo)
        {
            using (FileStream Stream = File.Open(filePath, FileMode.Open))
            {
                if (entidad.Atributos.Count == 0)
                {
                    entidad.ApAtr = Stream.Length;
                    WriteAtributo(Stream, Stream.Length, atributo);
                    WriteEntidad(entidad.Pos, entidad, Stream);
                    //llenar lista de atributos

                }
                else
                {
                    Atributo atrAux = entidad.Atributos.Last();
                    atrAux.ApNextAtr = Stream.Length;
                    WriteAtributo(Stream, atrAux.Posicion, atrAux);
                    WriteAtributo(Stream, Stream.Length, atributo);
                    WriteEntidad(entidad.Pos, entidad, Stream);

                }
                FindAtributos(Stream, entidad);
            }         

        }

        public void EditAtributo(Entidad e, string atrName, int colIndex, string value)
        {
            Atributo atrAux = e.Atributos.Find(atr => atr.Nombre == atrName);

            using (FileStream Stream = File.Open(FilePath, FileMode.Open))
            {
                switch (colIndex)
                {
                    case 0:

                        bool.TryParse(value, out bool b);
                        atrAux.LlavePrim = b;
                        break;
                    case 1:
                        atrAux.SetName(value);
                        break;
                    case 2:
                        atrAux.SetType(value[0]);
                        break;
                    case 3:
                        int.TryParse(value, out int l);
                        atrAux.Longitud = l;
                        break;
                }
                WriteAtributo(Stream, atrAux.Posicion, atrAux);
            }
               
          
        }

        private void FindAtributos(FileStream stream, Entidad entidad)
        {
            List<Atributo> atributos = new List<Atributo>();
            Int64 apAux = entidad.ApAtr;

            while (apAux != -1)
            {
                Atributo atrAux = FetchAttibuto(stream, apAux);
                atributos.Add(atrAux);
                apAux = atrAux.ApNextAtr;

            }
            if (atributos.Any(a => a.LlavePrim))
                entidad.HasPrimaryKey = true;
            entidad.Atributos = atributos;

        }

        public void DeleteAtributo(Entidad entidad, string atributoName)
        {
            using (FileStream Stream = File.Open(FilePath, FileMode.Open))
            {
                if ((entidad.Atributos.Count == 1) && entidad.Atributos[0].Nombre == atributoName)
                {
                    entidad.ApAtr = -1;
                    WriteEntidad(entidad.Pos, entidad, Stream);
                }

                else if (entidad.Atributos.Count > 1)
                {
                    Int64 index = entidad.Atributos.FindIndex(atr => atr.Nombre == atributoName);

                    if (index == 0)
                    {
                        entidad.ApAtr = entidad.Atributos[1].Posicion;
                        WriteEntidad(entidad.Pos, entidad, Stream);
                    }
                    else if (index == entidad.Atributos.Count - 1)
                    {
                        entidad.Atributos[(int)index - 1].ApNextAtr = -1;
                        WriteAtributo(Stream, entidad.Atributos[(int)index - 1].Posicion, entidad.Atributos[(int)index - 1]);

                    }
                    else
                    {
                        entidad.Atributos[(int)index - 1].ApNextAtr = entidad.Atributos[(int)index + 1].Posicion;
                        WriteAtributo(Stream, entidad.Atributos[(int)index - 1].Posicion, entidad.Atributos[(int)index - 1]);
                        WriteAtributo(Stream, entidad.Atributos[(int)index + 1].Posicion, entidad.Atributos[(int)index + 1]);

                    }

                }
                FindAtributos(Stream,entidad);

            }

        }
    }
}
