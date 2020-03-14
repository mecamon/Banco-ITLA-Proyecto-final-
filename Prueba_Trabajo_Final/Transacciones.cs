using System;
using System.Collections.Generic;
using System.Text;


namespace Prueba_Trabajo_Final
{
    [Serializable]
    class Transacciones
    {
        public string numeroTarjeta { get; set; }
        public string tipoTransaccion { get; set; }
        public double montoTransaccion { get; set; }
        public double balanceAnterior { get; set; }
        public double balanceNuevo { get; set; }  
        public DateTime DateTime { get; set; }

        public Transacciones(string n_tarjeta, string t_trans, double m_trans, double b_anterior, double b_nuevo, DateTime date) 
        {
            numeroTarjeta = n_tarjeta;
            tipoTransaccion = t_trans;
            montoTransaccion = m_trans;
            balanceAnterior = b_anterior;
            balanceNuevo = b_nuevo;
            DateTime = date;
        }
    }
}
