using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Prueba_Trabajo_Final
{
    class SerUsuarios
    {
        //--------------------------------Metodos para admin y el login-------------------------------------------
        enum opcionesMenuP
        {
            AGREGAR_CLIENTE = 1,
            EDITAR_CLIENTE,
            ELIMINAR_CLIENTE,
            REINICIAR_CONTRASENYA,
            AGREGAR_SALDO = 5,
            VER_LOG,
            CONFIGURACION_ATM,
            ADMINISTRAR_USER,
            REACTIVACION_USER,
            CERRAR_SESION,
        }
        
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
        public static void serializacion(Object objeto) //Metodo para serializar
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("banco.dat", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, objeto);
            stream.Close();
        }
        public static List<Usuarios> deserializacion() //Metodo para deserializar
        {
            List<Usuarios> Lista;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("banco.dat", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Lista = (List<Usuarios>)formatter.Deserialize(stream);
            stream.Close();

            return Lista;
        }
        public static void login()
        {
            Console.WriteLine("Login al sistema.\n");

            int contador = 0;
            string tarjeta = "";
            string contrasenya = "";
            bool verif = false;

            try
            {
                List<Usuarios> Lista;

                //Cargando listado para analizar datos dentro de el
                Lista = deserializacion();

                Console.Write("Ingrese su numero de tarjeta: ");
                tarjeta = Console.ReadLine();

                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals(tarjeta))
                    {
                        verif = true;
                    }
                }

                if (verif == true)
                {
                    do
                    {
                        Console.Clear();
                        Console.Write("Ingrese su contrasena: ");
                        contrasenya = ReadPassword();


                        foreach (Usuarios item in Lista)
                        {
                            if (item.numeroTarjeta.Equals(tarjeta) && item.contrasenya.Equals(contrasenya))
                            {
                                if (item.isActive == true)
                                {
                                    if (item.isAdmin == true)
                                    {
                                        Console.WriteLine("Bienvenido {0}\n", item.nombre);
                                        menuAdmin(Lista, item);
                                        break;
                                    }
                                    else if (item.isAdmin == false)
                                    {
                                        Clientes.menuClientes(Lista, item);
                                        break;
                                    }
                                }
                                else if (item.isActive == false)
                                {
                                    Console.WriteLine("Este usuario ha sido desabilidado. Comuniquese con un administrador.");
                                    Console.ReadKey();
                                    Console.Clear();
                                    login();
                                }
                            }
                        }

                        contador++;

                        if (contador < 3)
                        {
                            Console.Write("\nContrasena equivocada. Presione cualquier tecla para intentar de nuevo.");
                            Console.ReadKey();
                        }

                    } while (contador < 3);

                    Lista.Where(t => t.numeroTarjeta.Equals(tarjeta)).ToList().ForEach(t => t.isActive = false);

                    serializacion( Lista);

                    Console.WriteLine("\nSu tarjeta numero {0} fue bloqueada por razones de seguridad. Para desbloquear comuniquese con un administrador.", tarjeta);
                    Console.ReadKey();
                    Console.Clear();
                    login();

                }

                else if (verif == false)
                {

                    Console.Clear();
                    Console.Write("Ingrese su contrasena: ");
                    contrasenya = ReadPassword();


                    Console.Write("\nContrasena o numero de tarjeta equivocado. Presione cualquier tecla para intentar de nuevo.");
                    Console.ReadKey();
                    Console.Clear();
                    login();

                }

            }
            catch (FormatException)
            {
                Console.WriteLine("Tipo de entrada equivocado. Presione cualquier tecla para volver al login.");
                Console.ReadKey();
                login();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al login.", exp.Message);
                Console.ReadKey();
                login();
            }
        }
        public static void crearArchivo() //En caso de que no este creado ya el archivo
        {
            if (!File.Exists("banco.dat"))
            {

                Usuarios usuarioAdmin = new Usuarios(); //Objeto para agregar a listado
                Banco userBanco = new Banco(); //Objeto para agregar a listado
                List<Usuarios> ListaU = new List<Usuarios>();

                usuarioAdmin.nombre = "admin";
                usuarioAdmin.contrasenya = "admin";
                usuarioAdmin.numeroTarjeta = "0000-0000-0000-0000";
                usuarioAdmin.isAdmin = true;
                usuarioAdmin.isActive = true;
                usuarioAdmin.N_banco = userBanco;
                usuarioAdmin.N_transacciones = new List<Transacciones>();
                ListaU.Add(usuarioAdmin);

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("banco.dat", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, ListaU);
                stream.Close();
            }
        }
        public static void menuAdmin(List<Usuarios> Lista, Usuarios User)
        {
            Console.WriteLine("Menu de administrador. Seleccione la opcion deseada.\n");
            Console.WriteLine(" 1- Agregar cliente\n 2- Editar cliente\n 3- Eliminar cliente\n 4- Reiniciar contrasena\n 5- Agregar saldo" +
                "\n 6- Ver log de transacciones\n 7- Configuracion del ATM\n 8- Administrar usuarios\n 9- Reactivacion de usuarios\n 10- Cerrar la sesion");

            int entry = 0;

            try
            {
                entry = Convert.ToInt32(Console.ReadLine());

                switch (entry)
                {
                    case (int)opcionesMenuP.AGREGAR_CLIENTE:
                        Console.Clear();
                        agregarCliente(Lista, User);
                        break;

                    case (int)opcionesMenuP.EDITAR_CLIENTE:
                        Console.Clear();
                        editarCliente(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.ELIMINAR_CLIENTE:
                        Console.Clear();
                        eliminarCliente(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.REINICIAR_CONTRASENYA:
                        Console.Clear();
                        reiniciarContrasenya(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.AGREGAR_SALDO:
                        Console.Clear();
                        agregarSaldo(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.VER_LOG:
                        Console.Clear();
                        verLogTrans(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.CONFIGURACION_ATM:
                        Console.Clear();
                        configurarATM(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.ADMINISTRAR_USER:
                        Console.Clear();
                        administrarUsuario(Lista, User);
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.REACTIVACION_USER:
                        Console.Clear();
                        reactivarUser(Lista, User);
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case (int)opcionesMenuP.CERRAR_SESION:
                        cerrarSesion(Lista, User);
                        Console.WriteLine("Gracias por utilizar nuestros servicios.");
                        Console.ReadKey();
                        Environment.Exit(00);
                        break;

                    default:
                        Console.WriteLine("Seleccione entre las opciones dadas.");
                        Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                        Console.ReadKey();
                        menuAdmin(Lista, User);
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Solo se aceptan entradas de tipo entero. Trate de nuevo.");
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void validacionTarjeta(Usuarios User, string mensajeLlegada, ref List<Usuarios> Lista, ref string tarjeta, ref int contador, ref int indice, ref string respuesta)
        {
            do
            {
                Console.Clear();
                Console.WriteLine(mensajeLlegada);

                Lista = deserializacion();//Cargando datos en objeto

                listarUsuarios(Lista, User);

                Console.Write("Inserte el numero de tarjeta del cliente: ");
                tarjeta = Console.ReadLine();

                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals(tarjeta))
                    {
                        contador++;
                        indice = Lista.IndexOf(item);
                    }
                }

                if (contador > 0)
                {
                    break;
                }

                else if (contador == 0)
                {
                    Console.Write("\nEl numero de tarjeta ingresado no existe en el registro. Desea realizar otra busqueda? [Y] o [N]: ");
                    respuesta = Console.ReadLine();

                    if (respuesta == "N" || respuesta == "n")
                    {
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                }
            } while (respuesta == "Y" || respuesta == "y");
        }
        public static void agregarCliente(List<Usuarios> Lista, Usuarios User)
        {
            Usuarios usuarioNuevo = new Usuarios();
            Banco userBanco = new Banco();

            string tarjeta_n;

            Console.WriteLine("Menu para agregar cliente\n");

            try
            {
                Console.Write("Inserte el nombre del cliente nuevo: ");
                usuarioNuevo.nombre = Console.ReadLine();

                Console.Write("\nInserte el apellido del cliente nuevo: ");
                usuarioNuevo.apellido = Console.ReadLine();

                Console.Write("\nInserte el numero de la tarjeta del cliente nuevo: ");
                tarjeta_n = Console.ReadLine();

                if (tarjeta_n.Length < 19)
                {
                    Console.WriteLine("\nSolo se permite formato de 0000-0000-0000-0000. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                }

                foreach (Usuarios item in Lista)
                {
                    if (tarjeta_n.Equals(item.numeroTarjeta))
                    {
                        Console.WriteLine("\nEl numero de tarjeta ingresado ya esta en uso. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                }

                usuarioNuevo.numeroTarjeta = tarjeta_n;

                Console.Write("\nInserte la contrasena del cliente nuevo: ");
                usuarioNuevo.contrasenya = ReadPassword();

                //Console.Write("\nInserte la contrasena del cliente nuevo: ");
                //usuarioNuevo.contrasenya = Console.ReadLine();

                Console.Write("\nInserte el saldo inicial del cliente nuevo: ");
                usuarioNuevo.saldo = Convert.ToInt32(Console.ReadLine());

                userBanco.nombreBanco = "ITLA";

                //Para asignar modo de dispensacion actual
                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals("0000-0000-0000-0000"))
                    {
                        userBanco.modoDispensacion = item.N_banco.modoDispensacion;
                    }
                }

                usuarioNuevo.isAdmin = false;
                usuarioNuevo.isActive = true;
                usuarioNuevo.N_banco = userBanco;
                usuarioNuevo.N_transacciones = new List<Transacciones>();

                Lista.Add(usuarioNuevo);//Actualizando lista

                serializacion(Lista);

                Console.WriteLine("Cliente agregado de manera exitosa! Presione cualquier tecla para volver al menu.");
                Console.ReadKey();

                List<Usuarios> ListaAct;

                //Cargando listado para analizar datos dentro de el
                ListaAct = deserializacion();

                Console.Clear();
                menuAdmin(ListaAct, User);
            }
            catch (FormatException)
            {
                Console.WriteLine("Tipo de dato ingresado equivocado. Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu principal.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }

        }
        public static void listarUsuarios(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                foreach (Usuarios item in Lista)
                {
                    if (item.isAdmin == false)
                    {
                        Console.WriteLine("Nombre: {0}", item.nombre);
                        Console.WriteLine("Apellido: {0}", item.apellido);
                        Console.WriteLine("N. de tarjeta: {0}", item.numeroTarjeta);
                        Console.WriteLine("");
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void editarCliente(List<Usuarios> Lista, Usuarios User)
        {
            try
            {

                int contador = 0;
                int indice = 9;
                string respuesta = "";
                string tarjeta = "";
                string mensajeLlegada = "Menu de edicion de clientes\n";

                validacionTarjeta(User, mensajeLlegada, ref Lista, ref tarjeta, ref contador, ref indice, ref respuesta);

                Console.WriteLine("Editando cliente: {0} {1}\n", Lista[indice].nombre, Lista[indice].apellido);

                Console.Write("\nInserte el nuevo nombre: ");
                string nuevoNombre = Console.ReadLine();

                Console.WriteLine("");

                Console.Write("\nInserte el nuevo apellido: ");
                string nuevoApellido = Console.ReadLine();

                Lista.Where(t => t.numeroTarjeta == tarjeta).ToList().ForEach(t => { t.nombre = nuevoNombre; t.apellido = nuevoApellido; });

                Console.WriteLine("Edicion realizada de manera exitosa! Presione cualquier tecla para volver al menu.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);

            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }
        public static void eliminarCliente(List<Usuarios> Lista, Usuarios User)
        {
            try
            {

                int contador = 0;
                int indice = 9;
                string respuesta = "";
                string tarjeta = "";
                string resp = "";
                string mensajeLlegada = "Menu para eliminar clientes\n";


                do
                {
                    validacionTarjeta(User, mensajeLlegada, ref Lista, ref tarjeta, ref contador, ref indice, ref respuesta);

                    Console.WriteLine("\nEsta seguro que desea elimnar al cliente: ");
                    Console.WriteLine("\nN. de tarjeta: {0}", Lista[indice].numeroTarjeta);
                    Console.WriteLine("Nombre: {0}", Lista[indice].nombre);
                    Console.WriteLine("Apellido: {0}", Lista[indice].apellido);
                    Console.Write("\nRespuesta: [Y] o [N]: ");

                    resp = Console.ReadLine();

                    if (resp == "Y" || resp == "y")
                    {
                        Lista.RemoveAt(indice);
                        Console.WriteLine("Cliente eliminado exitosamente! Presiona cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                    else if (resp == "N" || resp == "n")
                    {
                        Console.WriteLine("\nDesea realizar otra busqueda [Y] o [N]? ");
                        string resp2 = Console.ReadLine();

                        if (resp2 == "N" || resp2 == "n")
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Debe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                } while (resp == "N" || resp == "n");

            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void reiniciarContrasenya(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                int contador = 0;
                int indice = 9;
                string respuesta = "";
                string tarjeta = "";
                string mensajeLlegada = "Menu para reiniciar contrasena\n";

                validacionTarjeta(User, mensajeLlegada, ref Lista, ref tarjeta, ref contador, ref indice, ref respuesta);

                Console.WriteLine("Editando cliente: {0} {1}\n", Lista[indice].nombre, Lista[indice].apellido);

                Console.Write("Ingrese la nueva contrasena: ");
                string nuevaCont = ReadPassword();


                Lista.Where(t => t.numeroTarjeta == tarjeta).ToList().ForEach(t => t.contrasenya = nuevaCont);

                Console.WriteLine("\nEdicion realizada de manera exitosa! Presione cualquier tecla para volver al menu.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }
        public static void agregarSaldo(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                double saldoNuevo = 0;
                int contador = 0;
                int indice = 9;
                string respuesta = "";
                string tarjeta = "";
                string mensajeLlegada = "Menu para agregar saldo\n";

                validacionTarjeta(User, mensajeLlegada, ref Lista, ref tarjeta, ref contador, ref indice, ref respuesta);

                Console.WriteLine("Editando cliente: {0} {1}\n", Lista[indice].nombre, Lista[indice].apellido);

                Console.Write("Inserte el saldo a agregar: ");
                double saldoAgregar = Convert.ToDouble(Console.ReadLine());

                foreach (Usuarios item in Lista)
                {
                    saldoNuevo = saldoAgregar + item.saldo;
                }

                Lista.Where(t => t.numeroTarjeta == tarjeta).ToList().ForEach(t => t.saldo = saldoNuevo);

                Console.WriteLine("\nEdicion realizada de manera exitosa! Presione cualquier tecla para volver al menu.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void configurarATM(List<Usuarios> Lista, Usuarios User)
        {
            Console.WriteLine("Elija la opcion deseada: ");
            Console.WriteLine("\n 1- Cambiar nombre de banco\n 2- Modo de dispensacion \n 3- Volver atras");
            int opcion = 0;
            try
            {
                opcion = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }

            switch (opcion)
            {
                case 1:
                    cambiarNombreBanco(Lista, User);
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                    break;

                case 2:
                    cambiarModoDispensacion(Lista, User);
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                    break;

                case 3:
                    Console.Clear();
                    menuAdmin(Lista, User);
                    break;

                default:
                    Console.WriteLine("Debe elegir de entre las opciones dadas. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    menuAdmin(Lista, User);
                    break;
            }

        }
        public static void cambiarNombreBanco(List<Usuarios> Lista, Usuarios User)
        {

            Console.Write("Cual es el nuevo nombre del banco: ");
            string nuevoNombreBanco = "";
            string sel = "";
            try
            {
                nuevoNombreBanco = Console.ReadLine();

                Console.WriteLine("\nEsta seguro que desea hacer el cambio? [Y] o [N]");
                sel = Console.ReadLine();

                if (sel == "Y" || sel == "y")
                {
                    Lista.ForEach(t => t.N_banco.nombreBanco = nuevoNombreBanco);

                    Console.WriteLine("Presione cualquier tecla para volver al menu.");
                }
                else if (sel == "N" || sel == "n")
                {
                    Console.Clear();
                    menuAdmin(Lista, User);
                }
                else
                {
                    Console.WriteLine("Debe elegir de entre las opciones dadas. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    menuAdmin(Lista, User);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void cambiarModoDispensacion(List<Usuarios> Lista, Usuarios User)
        {

            Console.WriteLine("A que modo de dispensacion desea cambiar?");
            Console.WriteLine("\n 1- Solo papeletas de 200.00 RD$ y 1,000.00 RD$\n 2- Solo papeletas de 100.00 RD$ y 500.00 RD$\n 3- Papeletas de 100.00 RD$, 200.00 RD$, 500.00 RD$ y 1,000.00 RD$");
            int modo = 0;

            try
            {
                modo = Convert.ToInt32(Console.ReadLine());

                Lista.ForEach(t => t.N_banco.modoDispensacion = modo);

                Console.WriteLine("Modo de dispensacion cambiado. Presione cualquier tecla para volver al menu.");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        
        public static void listar_usuarios_prueba()
        {
            List<Usuarios> Lista;

            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("banco.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
                Lista = (List<Usuarios>)formatter.Deserialize(stream);
                stream.Close();
                foreach (Usuarios item in Lista)
                {
                    Console.WriteLine("N. de tarjeta: {0}", item.numeroTarjeta);
                    Console.WriteLine("Nombre: {0}", item.nombre);
                    Console.WriteLine("Apellido: {0}", item.apellido);
                    Console.WriteLine("Saldo: {0}", item.saldo);
                    Console.WriteLine("Contrasena: {0}", item.contrasenya);
                    if (item.isAdmin)
                    {
                        Console.WriteLine("Es administrador");
                    }
                    else
                    {
                        Console.WriteLine("No es administrador");
                    }


                    if (item.isActive)
                    {
                        Console.WriteLine("Esta activo");
                    }
                    else
                    {
                        Console.WriteLine("No esta activo");
                    }
                    Console.WriteLine("");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu principal.", exp.Message);
                Console.ReadKey();
            }
        }
        public static void verLogTrans(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                string n_tarjeta = "";
                int contador = 0;
                int indice = 9;
                string respuesta = "";
                string mensajeLlegada = "Menu para ver log de transacciones\n";

                listarUsuarios(Lista, User);

                Console.Write("\nInserte el numero de tarjeta del cliente que desea ver transacciones: ");

                validacionTarjeta(User, mensajeLlegada, ref Lista, ref n_tarjeta, ref contador, ref indice, ref respuesta);

                Console.WriteLine("");
                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals(n_tarjeta))
                    {
                        foreach (Transacciones item2 in item.N_transacciones)
                        {
                            Console.Write(item2.tipoTransaccion + " - ");
                            Console.Write(item2.DateTime + " - ");
                            Console.Write(item2.montoTransaccion.ToString("n2") + " - ");
                            Console.Write(item2.balanceAnterior.ToString("n2") + " - ");
                            Console.Write(item2.balanceNuevo.ToString("n2"));
                            Console.WriteLine("");
                        }
                    }
                }

                Console.WriteLine("\n\nPresione cualquier tecla para volver al menu.");
                Console.ReadKey();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void reactivarUser(List<Usuarios> Lista, Usuarios User)
        {
            int contador = 0;

            try
            {
                Console.Clear();
                Console.Write("Inserte el numero de tarjeta que desea reactivar: ");
                string n_tarjeta = Console.ReadLine();

                foreach (Usuarios item in Lista)
                {

                    if (item.numeroTarjeta.Equals(n_tarjeta))
                    {
                        if (item.isActive == true)
                        {
                            Console.WriteLine("\nEl cliente que trata de activar, ya esta activo. Desea introducir un nuevo numero de tarjeta? [Y] o [N]");
                            string resp = Console.ReadLine();

                            if (resp == "Y" || resp == "y")
                            {
                                Console.Clear();
                                reactivarUser(Lista, User);
                            }
                            else if (resp == "N" || resp == "n")
                            {
                                Console.Clear();
                                menuAdmin(Lista, User);
                            }
                            else
                            {
                                Console.WriteLine("\nSeleccione solo dentro de las opciones dadas. Presione cualquier tecla para volver al menu.");
                                Console.ReadKey();
                                Console.Clear();
                                menuAdmin(Lista, User);
                            }
                        }
                        else if (item.isActive == false)
                        {
                            Console.WriteLine("\nEsta seguro que desea reactivar el usuario {0}? [Y] o [N]", item.nombre);
                            string resp = Console.ReadLine();

                            if (resp == "Y" || resp == "y")
                            {
                                contador++;
                                item.isActive = true;
                                Console.WriteLine("\nEl usuario {0} con numero de tarjeta {1} ha sido reactivado. Presione cualquier tecla para volver al menu.", item.nombre, item.numeroTarjeta);
                                Console.ReadKey();
                                serializacion(Lista);

                                Console.Clear();
                                menuAdmin(Lista, User);
                            }
                            else if (resp == "N" || resp == "n")
                            {
                                Console.Clear();
                                menuAdmin(Lista, User);
                            }
                            else
                            {
                                Console.WriteLine("\nSeleccione solo dentro de las opciones dadas. Presione cualquier tecla para volver al menu.");
                                Console.ReadKey();
                                Console.Clear();
                                menuAdmin(Lista, User);
                            }
                        }
                    }
                }

                if (contador == 0)
                {
                    Console.WriteLine("\nEl numero de tarjeta ingresado no existe. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void administrarUsuario(List<Usuarios> Lista, Usuarios User)
        {
            Console.Clear();
            Console.WriteLine("Menu para administrar usuarios. Seleccione la opcion deseada: \n");
            Console.WriteLine(" 1- Crear administrador\n 2- Editar administrador\n 3- Listar administradores\n 4- Eliminar administrador\n 5- Salir");

            int entry;

            try
            {
                entry = Convert.ToInt32(Console.ReadLine());

                switch (entry)
                {

                    case 1:
                        Console.Clear();
                        agregarAdmin(Lista, User);
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case 2:
                        Console.Clear();
                        editarAdmin(Lista, User);
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case 3:
                        Console.Clear();
                        listarAdmins(Lista, User);
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case 4:
                        Console.Clear();
                        eliminarAdmin(Lista, User);
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                        break;

                    case 5:
                        menuAdmin(Lista, User);
                        break;


                    default:
                        Console.WriteLine("Seleccione solo dentro de las opciones dadas. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        menuAdmin(Lista, User);
                        break;

                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Solo se aceptan entradas de tipo entero. Trate de nuevo.");
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                menuAdmin(Lista, User);
            }
        }
        public static void agregarAdmin(List<Usuarios> Lista, Usuarios User)
        {
            Usuarios usuarioNuevo = new Usuarios();
            Banco userBanco = new Banco();
            string tarjeta_n;

            Console.WriteLine("Menu para agregar administrador\n");

            try
            {
                Console.Write("Inserte el nombre del administrador nuevo: ");
                usuarioNuevo.nombre = Console.ReadLine();

                Console.Write("\nInserte el apellido del administrador nuevo: ");
                usuarioNuevo.apellido = Console.ReadLine();

                Console.Write("\nInserte el numero de la tarjeta del administrador nuevo: ");
                tarjeta_n = Console.ReadLine();

                if (tarjeta_n.Length < 19)
                {
                    Console.WriteLine("\nSolo se permite formato de 0000-0000-0000-0000. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                }

                foreach (Usuarios item in Lista)
                {
                    if (tarjeta_n.Equals(item.numeroTarjeta))
                    {
                        Console.WriteLine("\nEl numero de tarjeta ingresado ya esta en uso. Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                }

                usuarioNuevo.numeroTarjeta = tarjeta_n;

                Console.Write("\nInserte la contrasena del administrador nuevo: ");
                usuarioNuevo.contrasenya = ReadPassword();


                userBanco.nombreBanco = "ITLA";

                foreach (Usuarios item in Lista)
                {
                    if (item.numeroTarjeta.Equals("0000-0000-0000-0000"))
                    {
                        userBanco.modoDispensacion = item.N_banco.modoDispensacion;
                    }
                }

                usuarioNuevo.isAdmin = true;
                usuarioNuevo.isActive = true;
                usuarioNuevo.N_banco = userBanco;
                usuarioNuevo.N_transacciones = new List<Transacciones>();

                Lista.Add(usuarioNuevo);//Actualizando lista

                Console.WriteLine("Administrador agregado de manera exitosa! Presione cualquier tecla para volver al menu.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Tipo de dato ingresado equivocado.");
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu principal.", exp.Message);
                Console.ReadKey();
                menuAdmin(Lista, User);
            }

        }
        public static void listarAdmins(List<Usuarios> Lista, Usuarios User)
        {
            try
            {

                Console.WriteLine("Listando administradores: \n");
                foreach (Usuarios item in Lista)
                {
                    if (item.isAdmin == true)
                    {
                        Console.WriteLine("N. de tarjeta: {0}", item.numeroTarjeta);
                        Console.WriteLine("Nombre: {0}", item.nombre);
                        Console.WriteLine("Apellido: {0}", item.apellido);
                        Console.WriteLine("");
                    }
                }
                Console.WriteLine("Presione cualquier tecla para volver al menu.");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu principal.", exp.Message);
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }
        public static void eliminarAdmin(List<Usuarios> Lista, Usuarios User)
        {
            string n_tarjeta = "";
            int contador = 0;
            int pos = 0;

            Console.WriteLine("Listando administradores: \n");
            foreach (Usuarios item in Lista)
            {
                if (item.isAdmin == true)
                {
                    if (item.numeroTarjeta != User.numeroTarjeta)
                    {
                        contador++;
                        Console.WriteLine("N. de tarjeta: {0}", item.numeroTarjeta);
                        Console.WriteLine("Nombre: {0}", item.nombre);
                        Console.WriteLine("Apellido: {0}", item.apellido);
                        Console.WriteLine("");
                    }
                }
            }

            Console.Write("Ingrese el numero de tarjeta que desea eliminar: ");
            n_tarjeta = Console.ReadLine();

            foreach (Usuarios item in Lista)
            {
                if (n_tarjeta.Equals(User.numeroTarjeta))
                {
                    Console.WriteLine("Un usuario no se puede eliminar a si mismo! Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                }
                else if (item.numeroTarjeta.Equals(n_tarjeta))
                {
                    pos = Lista.IndexOf(item);
                    
                }
            }

            Lista.RemoveAt(pos);
            Console.WriteLine("\nAdministrador elminado. Presione cualquier tecla para volver al menu.");

            if (contador == 0)
            {
                Console.WriteLine("No hay mas administradores. Presione cualquier tecla para volver al menu.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }
        public static void editarAdmin(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                int contador = 0;
                string n_tarjeta = "";
                Console.WriteLine("Menu de edicion de administradores.\n");

                Console.WriteLine("Listando administradores: \n");
                foreach (Usuarios item in Lista)
                {
                    if (item.isAdmin == true)
                    {
                        if (item.numeroTarjeta != User.numeroTarjeta)
                        {
                            contador++;
                            Console.WriteLine("N. de tarjeta: {0}", item.numeroTarjeta);
                            Console.WriteLine("Nombre: {0}", item.nombre);
                            Console.WriteLine("Apellido: {0}", item.apellido);
                            Console.WriteLine("");
                        }
                    }
                }

                Console.Write("Ingrese el numero de tarjeta que desea editar: ");
                n_tarjeta = Console.ReadLine();

                foreach (Usuarios item in Lista)
                {
                    if (n_tarjeta.Equals(User.numeroTarjeta))
                    {
                        Console.WriteLine("\nUn usuario no se puede editar a si mismo! Presione cualquier tecla para volver al menu.");
                        Console.ReadKey();
                        Console.Clear();
                        menuAdmin(Lista, User);
                    }
                    else if (item.numeroTarjeta.Equals(n_tarjeta))
                    {
                        Console.WriteLine("\nEditando administrador: {0} {1}\n", item.nombre, item.apellido);

                        Console.Write("Inserte el nuevo nombre: ");
                        string nuevoNombre = Console.ReadLine();

                        Console.Write("\nInserte el nuevo apellido: ");
                        string nuevoApellido = Console.ReadLine();

                        Lista.Where(t => t.numeroTarjeta == n_tarjeta).ToList().ForEach(t => { t.nombre = nuevoNombre; t.apellido = nuevoApellido; });

                        Console.WriteLine("\nEdicion realizada de manera exitosa! Presione cualquier tecla para volver al menu.");
                    }
                }


            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}. Presione cualquier tecla para volver al menu.", exp.Message);
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }
        public static void cerrarSesion(List<Usuarios> Lista, Usuarios User)
        {
            try
            {
                Console.WriteLine("Esta seguro que desea cerrar sesion [Y] o [N]?");
                string resp = Console.ReadLine();
                if (resp == "Y" || resp == "y")
                {
                    Console.Clear();
                    serializacion(Lista);
                    login();
                }
                else if (resp == "N" || resp == "n")
                {
                    Console.Clear();
                    menuAdmin(Lista, User);
                }
                else
                {
                    Console.WriteLine("Debe elegir solo [Y] o [N]. Presione cualquier tecla para volver al menu.");
                    Console.ReadKey();
                    Console.Clear();
                    menuAdmin(Lista, User);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error: {0}", exp.Message);
                Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
                Console.ReadKey();
                Console.Clear();
                menuAdmin(Lista, User);
            }
        }

        

    }
}