using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FileStruct
{
    public class DictionaryFile
    {
        private bool isOpen;
        private Int64 lenght;
        private string filePath;
        private string projectPath;
        private FileStream stream;

        public bool IsOpen {get => isOpen;}
        public string FilePath { get => filePath;}
        public Int64 Lenght { get => stream.Length; }

        public DictionaryFile(string projectDirectory)
        {
            this.projectPath = projectDirectory;
            this.filePath = projectDirectory + "//Diccionario";
            Int64 size;
            stream = File.Open(filePath, FileMode.OpenOrCreate);                                      
            if (Lenght==0)                   
                SetHader(-1);
            
            

        }

        public void SetHader(Int64 value)
        {
           
            stream.Seek(0,SeekOrigin.Begin);
            BinaryWriter writer = new BinaryWriter(stream);            
            writer.Write(value);
           
       
            
        }
        public void InsertEntidad(Entity entidad)
        {

            //using (FileStream Stream = File.Open(filePath, FileMode.Open))
            //{
                stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
                //using (BinaryReader Reader = new BinaryReader(stream))
                //{
                    Int64 Cab = reader.ReadInt64();
                    if (Cab == -1)
                    {
                        //Header update
                        stream.Seek(0, SeekOrigin.Begin);
                        BinaryWriter Writer = new BinaryWriter(stream);
                        Writer.Write((Int64)stream.Length);

                        //entidad.WriteAt(Stream, Stream.Length);///Position waas 8, now it is stream.lenght
                        WriteEntidad(stream.Length,entidad,stream);
                    }
                    else
                    {
                        Int64 APEntidad = Cab;

                       // Entidad LastEnt = Entidad.FetchAt(Stream, APEntidad);
                        Entity LastEnt = FetchEntidad(stream,APEntidad);
                        while (LastEnt.ApNext != -1)
                        {
                            LastEnt = FetchEntidad(stream, APEntidad);
                            APEntidad = LastEnt.ApNext;
                        }

                        LastEnt.ApNext = stream.Length;
                        WriteEntidad(LastEnt.Pos, LastEnt,stream);
                        WriteEntidad(stream.Length, entidad,stream);
                        


                    }

                  
               // }
                if (!File.Exists(projectPath + "\\" + entidad.Nombre))
                   File.Create(projectPath + "\\" + entidad.Nombre).Close();

           // }


        }


        public void DeleteEntidad(string EntidadName)
        {
            //using (FileStream Stream = File.Open(filePath, FileMode.Open))
            //{
                if (File.Exists(projectPath + "\\" + EntidadName))
                {
                    File.Delete(projectPath + "\\" + EntidadName);
                    MessageBox.Show(projectPath + "\\" + EntidadName);
                }

                else
                    MessageBox.Show("El archivo no existe");


                BinaryReader Reader = new BinaryReader(stream);
                stream.Seek(0, SeekOrigin.Begin);
                Int64 AuxPtr = Reader.ReadInt64();
                Int64 Cab = AuxPtr;



                while (AuxPtr != -1)
                {
                   // Entidad E = Entidad.FetchAt(Stream, AuxPtr);
                    Entity E = FetchEntidad(stream,AuxPtr);

                    if (E.ApNext != -1)
                    {
                       // Entidad ENext = Entidad.FetchAt(Stream, E.ApNext);
                        Entity ENext = FetchEntidad(stream, E.ApNext);
                        if (ENext.Nombre == EntidadName)
                        {
                            if (ENext.ApNext != -1)
                            {
                                E.ApNext = ENext.ApNext;
                               // E.WriteAt(Stream, E.Pos);
                                WriteEntidad(E.Pos,E,stream);
                                return;
                            }
                            else
                            {
                                E.ApNext = -1;
                                WriteEntidad(E.Pos,E,stream);
                               // E.WriteAt(Stream, E.Pos);
                                return;

                            }


                        }
                        if (E.Nombre == EntidadName)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            BinaryWriter Writer = new BinaryWriter(stream);
                            Writer.Write((Int64)E.ApNext);
                            return;
                        }

                        AuxPtr = E.ApNext;
                    }
                    else
                    {
                        if (Cab == E.Pos && E.Nombre == EntidadName)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            BinaryWriter Writer = new BinaryWriter(stream);
                            Writer.Write((Int64)(-1));
                            return;
                        }

                    }


                }
          
               
        }

        public void EditEntidad(string EntidadName, string NewNAme)
        {
            Int64 Pos = FindEntidad(EntidadName);
                          
                if (Pos != -1)
                {
                   
                    Entity EAux = FetchEntidad(stream,Pos);
                    EAux.SetName(NewNAme);
                  
                    WriteEntidad(Pos,EAux,stream);
                }
           
            if (File.Exists(projectPath + "\\" + EntidadName))
                File.Move(projectPath + "\\" + EntidadName, projectPath + "\\" + NewNAme);

        }

        public List<object[]> ReadEntidades()
        {
            //using (FileStream Stream = File.OpenRead(filePath))
            //{
                List<object[]> entidades = new List<object[]>();
                stream.Seek(0, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(stream);

                Int64 ApAux = Reader.ReadInt64();
                while (ApAux != -1)
                {
                   //Entidad EAux = Entidad.FetchAt(Stream, ApAux);
                    Entity EAux = FetchEntidad(stream,ApAux);
                    object[] reg = { EAux.Nombre, EAux.Pos, EAux.ApAtr, EAux.ApData, EAux.ApNext };
                    entidades.Add(reg);
                    ApAux = EAux.ApNext;

                }
                return entidades;
            //}
               
           
        }
        
        private  Entity FetchEntidad(FileStream Stream,Int64 Pos)
        {
                       
            Stream.Seek(Pos, SeekOrigin.Begin);                
            BinaryReader Reader = new BinaryReader(Stream);

            Entity E = new Entity(new string(Reader.ReadChars(30)));
            E.Pos = Reader.ReadInt64();
            E.ApAtr = Reader.ReadInt64();
            E.ApData = Reader.ReadInt64();
            E.ApNext = Reader.ReadInt64();
            FindAtributos(Stream, E);
            E.UpdateRegisters();
            
           
           

            return E;
        }
        /// <summary>
        /// Reads an Entidad object from the dictionary file in a certain position
        /// </summary>
        /// <param name="pos">
        /// The position in the file of the Entidad
        /// </param>
        /// <returns>
        /// An Entidad object
        /// </returns>
        public Entity FetchEntidad(Int64 pos)
        {
          
                stream.Seek(pos, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(stream);
                Entity E = Entity.CreateNew(new string(Reader.ReadChars(30)));     
                E.Pos = Reader.ReadInt64();
                E.ApAtr = Reader.ReadInt64();
                E.ApData = Reader.ReadInt64();
                E.ApNext = Reader.ReadInt64();
                FindAtributos(stream,E);
         
                           
            return E;
        }


        /// <summary>
        /// Searches an Entidad in the Dictionary file by its name and returns it as an Entidad object 
        /// </summary>
        /// <param name="entidadName"> The name of the Entidad</param>
        /// <returns> An Entidad object if found, otherwise null
        /// </returns>
        public Entity FetchEntidad(string entidadName)
        {
            Int64 pos = FindEntidad(entidadName);
            if (pos != -1)
            {
                stream.Seek(pos, SeekOrigin.Begin);
                BinaryReader Reader = new BinaryReader(stream);
                Entity E = Entity.CreateNew(new string(Reader.ReadChars(30)));
                E.Pos = Reader.ReadInt64();
                E.ApAtr = Reader.ReadInt64();
                E.ApData = Reader.ReadInt64();
                E.ApNext = Reader.ReadInt64();
                FindAtributos(stream, E);
                E.Dictionary = this;
                E.UpdateRegisters();
                return E;
            }
            else
                return null;
            


           
        }
        /// <summary>
        /// Finds an Entidad object in the dictionary file and returns the position of the object in the file
        /// </summary>
        /// <param name="EntidadName">The name of the Entidad </param>
        /// <returns> An integer with the position of the Entidad </returns>
        public Int64 FindEntidad(string EntidadName)
        {
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader Reader = new BinaryReader(stream);
            Int64 AuxPtr = Reader.ReadInt64();
            while (AuxPtr != -1)
            {
                Entity E = FetchEntidad(stream, AuxPtr);
                if (E.Nombre == EntidadName)
                    return E.Pos;
                AuxPtr = E.ApNext;
            }
            return AuxPtr;
            
        }

        private void WriteEntidad(Int64 Pos, Entity entidad, FileStream stream)
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

        public void WriteEntidad(Int64 Pos, Entity entidad)
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

        private void WriteAtributo(FileStream Stream, Int64 pos, Attribute atributo)
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

        private Attribute FetchAttibuto(FileStream Stream, Int64 Pos)
        {
            Stream.Seek(Pos, SeekOrigin.Begin);
            Attribute auxAtr = new Attribute();
            BinaryReader Reader = new BinaryReader(Stream);

            auxAtr.SetName(new string(Reader.ReadChars(30)));
            auxAtr.SetType(Reader.ReadChar());
            auxAtr.Longitud = Reader.ReadInt64();
            auxAtr.Posicion = Reader.ReadInt64();
            auxAtr.ApNextAtr = Reader.ReadInt64();
            auxAtr.LlavePrim = Reader.ReadBoolean();

            return auxAtr;
        }
        public void InsertAtributo(Entity entidad, Attribute atributo)
        {
            
                if (entidad.Atributos.Count == 0)
                {
                    entidad.ApAtr = stream.Length;
                    WriteAtributo(stream, stream.Length, atributo);
                    WriteEntidad(entidad.Pos, entidad, stream);
                    

                }
                else
                {
                    Attribute atrAux = entidad.Atributos.Last();
                    atrAux.ApNextAtr = stream.Length;
                    WriteAtributo(stream, atrAux.Posicion, atrAux);
                    WriteAtributo(stream, stream.Length, atributo);
                    WriteEntidad(entidad.Pos, entidad, stream);

                }
                FindAtributos(stream, entidad);
                   

        }

        public void EditAtributo(Entity e, string atrName, int colIndex, string value)
        {
            Attribute atrAux = e.Atributos.Find(atr => atr.Nombre == atrName);

          
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
                WriteAtributo(stream, atrAux.Posicion, atrAux);
            
               
          
        }

        private void FindAtributos(FileStream stream, Entity entidad)
        {
            List<Attribute> atributos = new List<Attribute>();
            Int64 apAux = entidad.ApAtr;

            while (apAux != -1)
            {
                Attribute atrAux = FetchAttibuto(stream, apAux);
                atributos.Add(atrAux);
                apAux = atrAux.ApNextAtr;

            }
            if (atributos.Any(a => a.LlavePrim))
                entidad.HasPrimaryKey = true;
            else
                entidad.HasPrimaryKey = false;
            entidad.Atributos = atributos;

        }

        public void DeleteAtributo(Entity entidad, string atributoName)
        {
            
                if ((entidad.Atributos.Count == 1) && entidad.Atributos[0].Nombre == atributoName)
                {
                    entidad.ApAtr = -1;
                    WriteEntidad(entidad.Pos, entidad, stream);
                }

                else if (entidad.Atributos.Count > 1)
                {
                    Int64 index = entidad.Atributos.FindIndex(atr => atr.Nombre == atributoName);

                    if (index == 0)
                    {
                        entidad.ApAtr = entidad.Atributos[1].Posicion;
                        WriteEntidad(entidad.Pos, entidad, stream);
                    }
                    else if (index == entidad.Atributos.Count - 1)
                    {
                        entidad.Atributos[(int)index - 1].ApNextAtr = -1;
                        WriteAtributo(stream, entidad.Atributos[(int)index - 1].Posicion, entidad.Atributos[(int)index - 1]);
                    }
                    else
                    {
                        entidad.Atributos[(int)index - 1].ApNextAtr = entidad.Atributos[(int)index + 1].Posicion;
                        WriteAtributo(stream, entidad.Atributos[(int)index - 1].Posicion, entidad.Atributos[(int)index - 1]);
                        WriteAtributo(stream, entidad.Atributos[(int)index + 1].Posicion, entidad.Atributos[(int)index + 1]);

                    }

                }
                FindAtributos(stream,entidad);

            

        }
        public void Close()
        {
            this.stream.Close();
        }
    }
}
