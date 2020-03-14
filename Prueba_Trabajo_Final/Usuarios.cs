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
    class Usuarios
    {
        public string numeroTarjeta { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string contrasenya { get; set; }
        public bool isAdmin { get; set; }
        public double saldo { get; set; }
        public bool isActive { get; set; }
        public Banco N_banco { get; set; }
        public List<Transacciones> N_transacciones { get; set; }
    }

    
}
