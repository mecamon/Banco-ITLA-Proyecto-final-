using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba_Trabajo_Final
{
    [Serializable]
    class Banco
    {
        public string nombreBanco { get; set; }
        public int modoDispensacion { get; set; }

        public Banco(string x = "ITLA", int y = 1) 
        {
            nombreBanco = x;
            modoDispensacion = y;
        }
    }
}
