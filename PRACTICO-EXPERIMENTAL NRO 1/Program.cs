using System;
using System.IO;
using System.Text.Json;

namespace AgendaTurnosClinica
{
    // Clase que representa un turno médico de un paciente
    public class TurnoPaciente
    {
        public int IdTurno { get; set; }
        public string NombrePaciente { get; set; }
        public int Edad { get; set; }
        public string Especialidad { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public bool Activo { get; set; }

        // Constructor vacío necesario para poder cargar el JSON correctamente
        public TurnoPaciente()
        {
            NombrePaciente = string.Empty;
            Especialidad = string.Empty;
            Fecha = string.Empty;
            Hora = string.Empty;
        }

        // Constructor principal para crear un nuevo turno con todos sus datos
        public TurnoPaciente(int idTurno, string nombrePaciente, int edad, string especialidad, string fecha, string hora)
        {
            IdTurno = idTurno;
            NombrePaciente = nombrePaciente;
            Edad = edad;
            Especialidad = especialidad;
            Fecha = fecha;
            Hora = hora;
            Activo = true; // Por defecto el turno está activo al crearse
        }

        // Método para imprimir los datos del turno en la pantalla
        public void MostrarInformacion()
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Código del turno: " + IdTurno);
            Console.WriteLine("Paciente: " + NombrePaciente);
            Console.WriteLine("Edad: " + Edad);
            Console.WriteLine("Especialidad: " + Especialidad);
            Console.WriteLine("Fecha: " + Fecha);
            Console.WriteLine("Hora: " + Hora);
            Console.WriteLine("Estado: " + (Activo ? "Activo" : "Cancelado"));
        }
    }

    internal class Program
    {
        // Arreglo para guardar hasta 20 turnos en memoria
        static TurnoPaciente[] turnos = new TurnoPaciente[20];
        // Contador para saber cuántos turnos llevamos registrados
        static int contador = 0;
        // Nombre del archivo donde se guardará la información
        const string archivoJson = "turnos.json";

        static void Main(string[] args)
        {
            // Cargamos los turnos guardados previamente al iniciar el programa
            CargarTurnos();
            int opcion = 0;

            do
            {
                // Mostramos el menú principal
                Console.WriteLine("============================================");
                Console.WriteLine(" SISTEMA DE AGENDA DE TURNOS DE CLÍNICA");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Registrar turno");
                Console.WriteLine("2. Visualizar todos los turnos");
                Console.WriteLine("3. Buscar turno por paciente");
                Console.WriteLine("4. Cancelar turno");
                Console.WriteLine("5. Reporte general");
                Console.WriteLine("6. Salir");
                Console.WriteLine("============================================");
                Console.Write("Seleccione una opción: ");

                // Leemos la opción del usuario
                bool esNumero = int.TryParse(Console.ReadLine(), out opcion);

                if (!esNumero)
                {
                    Console.WriteLine("Debe ingresar un número válido.");
                    Pausar();
                    continue; // Volvemos al inicio del menú
                }

                // Ejecutamos la acción según la opción elegida
                switch (opcion)
                {
                    case 1:
                        RegistrarTurno();
                        break;
                    case 2:
                        MostrarTurnos();
                        break;
                    case 3:
                        BuscarTurno();
                        break;
                    case 4:
                        CancelarTurno();
                        break;
                    case 5:
                        ReporteGeneral();
                        break;
                    case 6:
                        Console.WriteLine("Gracias por utilizar el sistema.");
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        Pausar();
                        break;
                }

            } while (opcion != 6); // Repetir mientras no elija salir
        }

        // Función para registrar un nuevo turno
        static void RegistrarTurno()
        {
            Console.WriteLine("=== REGISTRO DE TURNO ===");

            // Verificamos si hay espacio disponible
            if (contador >= turnos.Length)
            {
                Console.WriteLine("No se pueden registrar más turnos.");
                Pausar();
                return;
            }

            int idTurno = contador + 1;

            Console.Write("Ingrese el nombre del paciente: ");
            string nombre = Console.ReadLine() ?? "";

            Console.Write("Ingrese la edad del paciente: ");
            int edad;

            // Validamos que la edad sea un número mayor a 0
            while (!int.TryParse(Console.ReadLine(), out edad) || edad <= 0)
            {
                Console.Write("Edad inválida. Ingrese nuevamente: ");
            }

            Console.Write("Ingrese la especialidad médica: ");
            string especialidad = Console.ReadLine() ?? "";

            Console.Write("Ingrese la fecha del turno (dd/mm/aaaa): ");
            string fecha = Console.ReadLine() ?? "";

            Console.Write("Ingrese la hora del turno (hh:mm): ");
            string hora = Console.ReadLine() ?? "";

            // Guardamos el nuevo turno en el arreglo y aumentamos el contador
            turnos[contador] = new TurnoPaciente(idTurno, nombre, edad, especialidad, fecha, hora);
            contador++;
            
            // Guardamos los cambios en el archivo JSON
            GuardarTurnos();

            Console.WriteLine();
            Console.WriteLine("Turno registrado correctamente.");
            Pausar();
        }

        // Función para mostrar todos los turnos existentes
        static void MostrarTurnos()
        {
            Console.WriteLine("=== LISTADO DE TURNOS REGISTRADOS ===");

            if (contador == 0)
            {
                Console.WriteLine("No existen turnos registrados.");
            }
            else
            {
                // Recorremos todos los turnos registrados y los mostramos
                for (int i = 0; i < contador; i++)
                {
                    turnos[i].MostrarInformacion();
                }
            }

            Pausar();
        }

        // Función para buscar turnos por el nombre del paciente
        static void BuscarTurno()
        {
            Console.WriteLine("=== BÚSQUEDA DE TURNO ===");

            Console.Write("Ingrese el nombre del paciente: ");
            string nombreBuscar = Console.ReadLine() ?? "";

            bool encontrado = false;

            // Recorremos los turnos buscando coincidencias en el nombre
            for (int i = 0; i < contador; i++)
            {
                // Convertimos a minúsculas para que la búsqueda sea más flexible
                if (turnos[i].NombrePaciente.ToLower().Contains(nombreBuscar.ToLower()))
                {
                    turnos[i].MostrarInformacion();
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("No se encontró ningún turno con ese nombre.");
            }

            Pausar();
        }

        // Función para cancelar un turno (cambia su estado, pero no lo borra del historial)
        static void CancelarTurno()
        {
            Console.WriteLine("=== CANCELAR TURNO ===");

            Console.Write("Ingrese el código del turno: ");
            int id;

            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Código inválido.");
                Pausar();
                return;
            }

            bool encontrado = false;

            // Buscamos el turno por su ID
            for (int i = 0; i < contador; i++)
            {
                if (turnos[i].IdTurno == id)
                {
                    turnos[i].Activo = false; // Desactivamos el turno
                    GuardarTurnos(); // Actualizamos el archivo
                    Console.WriteLine("Turno cancelado correctamente.");
                    encontrado = true;
                    break;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("No existe un turno con ese código.");
            }

            Pausar();
        }

        // Función para mostrar un resumen de los turnos
        static void ReporteGeneral()
        {
            Console.WriteLine("=== REPORTE GENERAL ===");

            int activos = 0;
            int cancelados = 0;

            // Contamos cuántos están activos y cuántos cancelados
            for (int i = 0; i < contador; i++)
            {
                if (turnos[i].Activo)
                    activos++;
                else
                    cancelados++;
            }

            Console.WriteLine("Total de turnos registrados: " + contador);
            Console.WriteLine("Turnos activos: " + activos);
            Console.WriteLine("Turnos cancelados: " + cancelados);
            Console.WriteLine("Capacidad máxima del sistema: " + turnos.Length);
            Console.WriteLine("Espacios disponibles: " + (turnos.Length - contador));

            Pausar();
        }

        // Función que pausa la ejecución hasta que el usuario presione una tecla
        static void Pausar()
        {
            Console.WriteLine();
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            Console.WriteLine(); // Espacio extra para que no se pegue el menú enseguida al continuar
        }

        // Función para leer los turnos desde el archivo JSON al iniciar
        static void CargarTurnos()
        {
            if (File.Exists(archivoJson))
            {
                try
                {
                    string json = File.ReadAllText(archivoJson);
                    var turnosCargados = JsonSerializer.Deserialize<TurnoPaciente[]>(json);
                    
                    if (turnosCargados != null)
                    {
                        // Aseguramos no pasarnos del límite del arreglo (20)
                        contador = Math.Min(turnosCargados.Length, turnos.Length);
                        for (int i = 0; i < contador; i++)
                        {
                            turnos[i] = turnosCargados[i];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cargar los turnos: " + ex.Message);
                }
            }
        }

        // Función para guardar los turnos actuales en el archivo JSON
        static void GuardarTurnos()
        {
            try
            {
                // Extraemos solo los turnos registrados actualmente (hasta donde llegue el contador)
                var turnosAGuardar = new TurnoPaciente[contador];
                Array.Copy(turnos, turnosAGuardar, contador);
                
                // Configuramos para que el JSON se guarde de forma legible (con saltos de línea y sangrías)
                var opciones = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(turnosAGuardar, opciones);
                
                File.WriteAllText(archivoJson, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar los turnos: " + ex.Message);
            }
        }
    }
}
