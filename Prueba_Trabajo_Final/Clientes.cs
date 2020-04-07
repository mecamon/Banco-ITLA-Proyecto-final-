using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Prueba_Trabajo_Final
{
    class Clientes
    {
        enum opcionesMenuClientes
        {
            RETIRAR_EFECTIVO = 1,
            DEPOSITAR_EFECTIVO,
            COMPRAR_TARJETA_LLAMADA,
            CONSULTAR_BALANCE,
            SALIR,
        }

        enum opcionesModoDisp
        {
            DE_200_1000 = 1,
            DE_100_500,
            DE_100_200_500_1000,
        }

        //--------------------------------Metodos para tarjeta habiente-------------------------------------------
        public static void menuClientes(List<Usuarios> Lista, Usuarios User)
        {
            Console.Clear();
            Console.WriteLine("Sesion de: {0} {1}\n", User.nombre, User.apellido);
            Console.WriteLine("Menu de clientes. Que operacion desea realizar?");
            Console.WriteLine(" 1- Retirar efectivo\n 2- Depositar efectivo\n 3- Comprar tarjeta de llamada\n 4- Consultar balance\n 5- Salir");

            int entry;

            try
            {
                entry = Convert.ToInt32(Console.ReadLine());

                switch (entry)
                {
                    case (int)opcionesMenuClientes.RETIRAR_EFECTIVO:
                        Console.Clear();
                        retiroEfectivo(Lista, User);
                        break;

                    case (int)opcionesMenuClientes.DEPOSITAR_EFECTIVO:
                        Console.Clear();
                        depositarEfectivo(Lista, User);
                        break;

                    case (int)opcionesMenuClientes.COMPRAR_TARJETA_LLAMADA:
                        Console.Clear();
                        comprarTarjeta(Lista, User);
                        break;

                    case (int)opcionesMenuClientes.CONSULTAR_BALANCE:
                        Console.Clear();
                        consultaBalance(Lista, User);
                        break;

                    case (int)opcionesMenuClientes.SALIR:
                        Console.WriteLine("Gracias por utilizar nuestros servicios.");
                        Console.ReadKey();
                        SerUsuarios.serializacion(Lista);
                        Console.Clear();
                        SerUsuarios.login();
                        break;

                    default:
                        Console.WriteLine("Seleccione solo dentro de las opciones dadas. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        menuClientes(Lista, User);
                        break;

                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Solo se aceptan entradas de tipo entero. Trate de nuevo.");
                Console.WriteLine("Presione cualquier tecla para volver al login.");
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                SerUsuarios.login();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("Presione cualquier tecla para volver al login.");
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                SerUsuarios.login();
            }
        }
        public static void consultaBalance(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Consulta de balance";

            try
            {
                double saldo = 0;

                foreach (Usuarios item in Lista)
                {
                    if (User.numeroTarjeta == item.numeroTarjeta)
                    {
                        saldo = item.saldo;
                        Console.WriteLine("     Banco {0}", item.N_banco.nombreBanco);
                        Console.WriteLine("");
                        Console.WriteLine("{0} {1} - {2}", item.nombre, item.apellido, item.numeroTarjeta.Substring(15));
                        Console.WriteLine("");
                        Console.Write("Balance Disponible: ");
                        Console.Write(item.saldo.ToString("n2"));
                        Console.Write(" RD$");
                        break;
                    }
                }

                llenarTrans(User.numeroTarjeta, tipoTrans, 0, saldo, saldo, DateTime.Now, Lista);

                repetirTransaccion(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("Presione cualquier tecla para volver al login.");
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }
        }
        public static void depositarEfectivo(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Deposito           ";

            try
            {
                Console.Write("Ingrese la cantidad que desea depositar: ");
                double saldoAgregar = Convert.ToDouble(Console.ReadLine());

                double saldoNuevo;

                double saldoAnterior = 0;

                //Para almacenar balance antes de la transaccion
                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals(User.numeroTarjeta))
                    {
                        saldoAnterior = item.saldo;
                    }
                }

                if (saldoAgregar % 100 == 0)
                {
                    saldoNuevo = saldoAgregar + saldoAnterior;

                    Lista.Where(t => t.numeroTarjeta == User.numeroTarjeta).ToList().ForEach(t => t.saldo = saldoNuevo);

                    foreach (Usuarios item in Lista)
                    {
                        if (User.numeroTarjeta == item.numeroTarjeta)
                        {
                            Console.Clear();
                            Console.WriteLine("     Banco {0}", User.N_banco.nombreBanco);
                            Console.WriteLine("");
                            Console.WriteLine("{0} {1} - {2}", User.nombre, User.apellido, User.numeroTarjeta.Substring(15));
                            Console.WriteLine("");
                            Console.Write("Balance Anterior: ");
                            Console.Write(saldoAnterior.ToString("n2"));
                            Console.Write(" RD$");

                            Console.Write("\nNuevo Balance: ");
                            Console.Write(item.saldo.ToString("n2"));
                            Console.Write(" RD$");
                            break;
                        }
                    }

                    llenarTrans(User.numeroTarjeta, tipoTrans, saldoAgregar, saldoAnterior, saldoNuevo, DateTime.Now, Lista);


                }
                else
                {
                    Console.WriteLine("\nLa entrada de dinero debe ser multiplos de 100.00 RD$. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuClientes(Lista, User);
                }

                repetirTransaccion(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("\nPresione cualquier tecla para volver al login.");
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }

        }
        public static void retiroEfectivo(List<Usuarios> Lista, Usuarios User)
        {
            switch (User.N_banco.modoDispensacion)
            {
                case (int)opcionesModoDisp.DE_200_1000:
                    modo200y1000(Lista, User);
                    break;

                case (int)opcionesModoDisp.DE_100_500:
                    modo100y500(Lista, User);
                    break;

                case (int)opcionesModoDisp.DE_100_200_500_1000:
                    modo100a1000(Lista, User);
                    break;
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menu.");
        }
        public static void comprarTarjeta(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Compra de tarjeta  ";
            int tarjeta = 0;
            string nombreCompania = "";
            double montos = 0;
            int montosEle = 0;
            double saldoNuevo = 0;

            try
            {
                Console.WriteLine("De que compania desea comprar su tarjeta?");
                Console.WriteLine("\n 1- Claro\n 2- Altice\n 3- Viva\n 4- Cancelar");
                tarjeta = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login", exp.Message);
                Console.Clear();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }

            switch (tarjeta)
            {
                case 1:
                    nombreCompania = "Claro";
                    break;

                case 2:
                    nombreCompania = "Altice";
                    break;

                case 3:
                    nombreCompania = "Viva";
                    break;

                case 4:
                    menuClientes(Lista, User);
                    break;

                default:
                    Console.WriteLine("Debe elegir de entre las opciones dadas. Presione cualquier tecla para volver al menu de clientes.");
                    Console.ReadKey();
                    menuClientes(Lista, User);
                    break;
            }

            try
            {
                Console.Clear();
                Console.WriteLine("De que monto desea comprar su tarjeta {0}?", nombreCompania);
                Console.WriteLine("\n 1- 60.00 RD$\n 2- 100.00 RD$\n 3- 150.00 RD$\n 4- 200.00 RD$\n 5- 250.00 RD$\n 6- Cancelar");
                montosEle = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login.", exp.Message);
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }

            switch (montosEle)
            {
                case 1:
                    montos = 60.00;
                    break;

                case 2:
                    montos = 100.00;
                    break;

                case 3:
                    montos = 150.00;
                    break;

                case 4:
                    montos = 200.00;
                    break;

                case 5:
                    montos = 250.00;
                    break;

                case 6:
                    menuClientes(Lista, User);
                    break;

                default:
                    Console.WriteLine("Debe elegir de entre las opciones dadas. Presione cualquier tecla para volver al menu de clientes.");
                    Console.ReadKey();
                    menuClientes(Lista, User);
                    break;
            }


            double saldoAnterior = 0;

            //Valiando si cuenta con suficiente saldo
            foreach (Usuarios item in Lista)
            {
                if (item.numeroTarjeta.Equals(User.numeroTarjeta))
                {
                    saldoAnterior = item.saldo;

                    if (item.saldo < montos)
                    {
                        Console.WriteLine("Usted no cuenta con sufieciente saldo para realizar esta transaccion. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        menuClientes(Lista, User);
                    }
                }
            }


            saldoNuevo = saldoAnterior - montos;

            Lista.Where(t => t.numeroTarjeta == User.numeroTarjeta).ToList().ForEach(t => t.saldo = saldoNuevo);

            foreach (Usuarios item in Lista)
            {
                if (User.numeroTarjeta == item.numeroTarjeta)
                {
                    Console.Clear();
                    Console.WriteLine("     Banco {0}", User.N_banco.nombreBanco);
                    Console.WriteLine("{0} {1} - {2}", User.nombre, User.apellido, User.numeroTarjeta.Substring(15));
                    Console.WriteLine("");
                    Console.WriteLine("Compania de telefono: {0}", nombreCompania);

                    Console.Write("Monto de la tarjeta: ");
                    Console.Write(montos.ToString("n2"));
                    Console.Write(" RD$\n");

                    Console.Write("Balance Anterior: ");
                    Console.Write(saldoAnterior.ToString("n2"));
                    Console.Write(" RD$");

                    Console.Write("\nNuevo Balance: ");
                    Console.Write(item.saldo.ToString("n2"));
                    Console.Write(" RD$");
                    break;
                }
            }

            llenarTrans(User.numeroTarjeta, tipoTrans, montos, saldoAnterior, saldoNuevo, DateTime.Now, Lista);


            repetirTransaccion(Lista, User);

        }
        public static void repetirTransaccion(List<Usuarios> Lista, Usuarios User)
        {
            Console.Write("\n\nDesea realizar alguna otra transaccion [Y] o [N]? ");

            string resp = Console.ReadLine();

            if (resp == "Y" || resp == "y")
            {
                Console.Clear();
                menuClientes(Lista, User);
            }
            else if (resp == "N" || resp == "n")
            {
                Console.Clear();
                SerUsuarios.serializacion(Lista);
                SerUsuarios.login();
            }
            else
            {
                Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                Console.ReadKey();
                Console.Clear();
                menuClientes(Lista, User);
            }
        }
        public static void llenarTrans(string n_tarjeta, string t_trans, double m_trans, double b_anterior, double b_nuevo, DateTime date, List<Usuarios> Lista)
        {

            Transacciones trans = new Transacciones(n_tarjeta, t_trans, m_trans, b_anterior, b_nuevo, date);

            Lista.Where(t => t.numeroTarjeta.Equals(n_tarjeta)).ToList().ForEach(t => t.N_transacciones.Add(trans));
        }

        public static void modo200y1000(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Retiro             ";
            double retiro;
            double _retiro;
            double nuevoSaldo = 0;
            double saldoAnterior = 0;
            int cantidadDe1000 = 0;
            int cantidadDe200 = 0;
            string resp = "";

            foreach (Usuarios item in Lista)
            {
                if (User.numeroTarjeta.Equals(item.numeroTarjeta))
                {
                    saldoAnterior = item.saldo;
                }
            }

            try
            {
                do
                {
                    Console.Write("Ingrese la cantidad de dinero que desea retirar: ");
                    retiro = Convert.ToDouble(Console.ReadLine());

                    _retiro = retiro;

                    if (_retiro % 200 != 0)
                    {
                        Console.WriteLine("\nEste cajero solo dispensa papeletas de 1,000.00 RD$ y 200.00 RD$. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo200y1000(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            SerUsuarios.login();
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro > User.saldo)
                    {
                        Console.WriteLine("No cuenta con suficiente fondos en su cuenta. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo200y1000(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro % 200 == 0)
                    {
                        while (_retiro >= 1000)
                        {
                            _retiro -= 1000;
                            cantidadDe1000++;
                        }

                        while (_retiro >= 200)
                        {

                            _retiro -= 200;
                            cantidadDe200++;

                        }

                    }

                } while (retiro % 200 != 0);

                nuevoSaldo = User.saldo - retiro;

                Lista.Where(t => t.numeroTarjeta == User.numeroTarjeta).ToList().ForEach(t => t.saldo = nuevoSaldo);

                if (cantidadDe1000 > 0)
                {
                    Console.WriteLine("\n{0} Papeletas de 1,000.00", cantidadDe1000);
                }
                if (cantidadDe200 > 0)
                {
                    Console.WriteLine("{0} Papeletas de 200.00", cantidadDe200);
                }

                llenarTrans(User.numeroTarjeta, tipoTrans, retiro, saldoAnterior, nuevoSaldo, DateTime.Now, Lista);

                repetirTransaccion(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login.", exp.Message);
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }
        }
        public static void modo100y500(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Retiro             ";
            double retiro;
            double _retiro;
            double nuevoSaldo = 0;
            double saldoAnterior = 0;
            int cantidadDe500 = 0;
            int cantidadDe100 = 0;
            string resp = "";

            foreach (Usuarios item in Lista)
            {
                if (User.numeroTarjeta.Equals(item.numeroTarjeta))
                {
                    saldoAnterior = item.saldo;
                }
            }

            try
            {
                do
                {
                    Console.Write("Ingrese la cantidad de dinero que desea retirar: ");
                    retiro = Convert.ToDouble(Console.ReadLine());

                    _retiro = retiro;

                    if (_retiro % 100 != 0)
                    {
                        Console.WriteLine("\nEste cajero solo dispensa papeletas de 500.00 RD$ y 100.00 RD$. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo100y500(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            SerUsuarios.login();
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro > User.saldo)
                    {
                        Console.WriteLine("No cuenta con suficiente fondos en su cuenta. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo100y500(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            SerUsuarios.login();
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro % 100 == 0)
                    {
                        while (_retiro >= 500)
                        {
                            _retiro -= 500;
                            cantidadDe500++;
                        }

                        while (_retiro >= 100)
                        {

                            _retiro -= 100;
                            cantidadDe100++;

                        }

                    }

                } while (retiro % 100 != 0);

                nuevoSaldo = User.saldo - retiro;

                Lista.Where(t => t.numeroTarjeta == User.numeroTarjeta).ToList().ForEach(t => t.saldo = nuevoSaldo);

                if (cantidadDe500 > 0)
                {
                    Console.WriteLine("\n{0} Papeletas de 500.00", cantidadDe500);
                }
                if (cantidadDe100 > 0)
                {
                    Console.WriteLine("{0} Papeletas de 100.00", cantidadDe100);
                }

                llenarTrans(User.numeroTarjeta, tipoTrans, retiro, saldoAnterior, nuevoSaldo, DateTime.Now, Lista);

                repetirTransaccion(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login.", exp.Message);
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }
        }
        public static void modo100a1000(List<Usuarios> Lista, Usuarios User)
        {
            string tipoTrans = "Retiro             ";
            double retiro;
            double _retiro;
            double nuevoSaldo = 0;
            double saldoAnterior = 0;
            int cantidadDe1000 = 0;
            int cantidadDe500 = 0;
            int cantidadDe200 = 0;
            int cantidadDe100 = 0;
            string resp = "";

            foreach (Usuarios item in Lista)
            {
                if (User.numeroTarjeta.Equals(item.numeroTarjeta))
                {
                    saldoAnterior = item.saldo;
                }
            }

            try
            {
                do
                {
                    Console.Write("Ingrese la cantidad de dinero que desea retirar: ");
                    retiro = Convert.ToDouble(Console.ReadLine());

                    _retiro = retiro;

                    if (_retiro % 100 != 0)
                    {
                        Console.WriteLine("\nEste cajero solo dispensa papeletas de 1,000.00 RD$, 500.00 RD$, 200.00 RD$ y 100.00 RD$. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo100a1000(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            SerUsuarios.login();
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro > User.saldo)
                    {
                        Console.WriteLine("No cuenta con suficiente fondos en su cuenta. Quiere intentar con otra cantidad? [Y] o [N].");
                        resp = Console.ReadLine();

                        if (resp == "Y" || resp == "y")
                        {
                            Console.Clear();
                            modo100a1000(Lista, User);
                        }
                        else if (resp == "N" || resp == "n")
                        {
                            Console.Clear();
                            SerUsuarios.login();
                        }
                        else
                        {
                            Console.WriteLine("\n\nDebe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                            Console.ReadKey();
                            Console.Clear();
                            menuClientes(Lista, User);
                        }
                    }

                    else if (_retiro % 100 == 0)
                    {
                        while (_retiro >= 1000)
                        {
                            _retiro -= 1000;
                            cantidadDe1000++;
                        }

                        while (_retiro >= 500)
                        {
                            _retiro -= 500;
                            cantidadDe500++;
                        }

                        while (_retiro >= 200)
                        {
                            _retiro -= 200;
                            cantidadDe200++;
                        }

                        while (_retiro >= 100)
                        {
                            _retiro -= 100;
                            cantidadDe100++;
                        }

                    }

                } while (retiro % 100 != 0);

                nuevoSaldo = User.saldo - retiro;

                Lista.Where(t => t.numeroTarjeta == User.numeroTarjeta).ToList().ForEach(t => t.saldo = nuevoSaldo);

                if (cantidadDe1000 > 0)
                {
                    Console.WriteLine("\n{0} Papeletas de 1,000.00", cantidadDe1000);
                }
                if (cantidadDe500 > 0)
                {
                    Console.WriteLine("{0} Papeletas de 500.00", cantidadDe500);
                }
                if (cantidadDe200 > 0)
                {
                    Console.WriteLine("{0} Papeletas de 200.00", cantidadDe200);
                }
                if (cantidadDe100 > 0)
                {
                    Console.WriteLine("{0} Papeletas de 100.00", cantidadDe100);
                }


                llenarTrans(User.numeroTarjeta, tipoTrans, retiro, saldoAnterior, nuevoSaldo, DateTime.Now, Lista);

                repetirTransaccion(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login.", exp.Message);
                Console.ReadKey();
                SerUsuarios.serializacion(Lista);
                Console.Clear();
                SerUsuarios.login();
            }
        }
    }
}
