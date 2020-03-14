using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Prueba_Trabajo_Final
{
    [Serializable]
    class ListaUsuarios
    {
        public List<Usuarios> ls_Usuarios = new List<Usuarios>();
    }
}
