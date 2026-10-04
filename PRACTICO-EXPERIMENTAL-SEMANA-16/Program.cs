// Programa para encuentrar vuelos baratos a partir de una base de datos a partir de una base de datos ficticia 
//usando un archivo vuelos.json
//Reqlizado por Miguel Aguas, Joel Murillo y Yensy Castillo

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MiProyectoConsola
{
    // Modelo de datos para un vuelo
    public class Vuelo
    {
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Aerolinea { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Origen} -> {Destino} | {Aerolinea} | ${Precio}";
        }
    }

    // Nodo para el Árbol Binario de Búsqueda
    public class NodoArbol
    {
        public Vuelo Vuelo { get; set; }
        public NodoArbol? Izquierdo { get; set; }
        public NodoArbol? Derecho { get; set; }

        public NodoArbol(Vuelo vuelo)
        {
            Vuelo = vuelo;
            Izquierdo = null;
            Derecho = null;
        }
    }

    // Implementación del Árbol Binario de Búsqueda (por Precio)
    public class ArbolVuelos
    {
        private NodoArbol? raiz;

        public void Insertar(Vuelo vuelo)
        {
            raiz = InsertarRecursivo(raiz, vuelo);
        }

        private NodoArbol InsertarRecursivo(NodoArbol? nodo, Vuelo vuelo)
        {
            if (nodo == null)
            {
                return new NodoArbol(vuelo);
            }

            if (vuelo.Precio < nodo.Vuelo.Precio)
            {
                nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, vuelo);
            }
            else
            {
                nodo.Derecho = InsertarRecursivo(nodo.Derecho, vuelo);
            }

            return nodo;
        }

        // Búsqueda de vuelos por debajo de un precio máximo
        public void MostrarVuelosBaratos(decimal precioMaximo)
        {
            Console.WriteLine($"\nVuelos con precio menor o igual a ${precioMaximo}:");
            bool encontrado = false;
            MostrarVuelosBaratosRecursivo(raiz, precioMaximo, ref encontrado);
            if (!encontrado)
            {
                Console.WriteLine("No se encontraron vuelos en ese rango de precio.");
            }
        }

        private void MostrarVuelosBaratosRecursivo(NodoArbol? nodo, decimal precioMaximo, ref bool encontrado)
        {
            if (nodo != null)
            {
                // Recorrido para mostrar de menor a mayor precio
                MostrarVuelosBaratosRecursivo(nodo.Izquierdo, precioMaximo, ref encontrado);

                if (nodo.Vuelo.Precio <= precioMaximo)
                {
                    Console.WriteLine(nodo.Vuelo);
                    encontrado = true;
                }

                // Solo exploramos la rama derecha si el nodo actual es menor o igual al precio máximo,
                // ya que los mayores estarán a la derecha.
                if (nodo.Vuelo.Precio <= precioMaximo)
                {
                    MostrarVuelosBaratosRecursivo(nodo.Derecho, precioMaximo, ref encontrado);
                }
            }
        }
    }

    // Implementación del Grafo de Vuelos
    public class GrafoVuelos
    {
        // Lista de adyacencia: Ciudad - Lista de Vuelos que salen
        private Dictionary<string, List<Vuelo>> adyacencia = new Dictionary<string, List<Vuelo>>(StringComparer.OrdinalIgnoreCase);

        public void AgregarVuelo(Vuelo vuelo)
        {
            if (!adyacencia.ContainsKey(vuelo.Origen))
            {
                adyacencia[vuelo.Origen] = new List<Vuelo>();
            }
            if (!adyacencia.ContainsKey(vuelo.Destino))
            {
                adyacencia[vuelo.Destino] = new List<Vuelo>();
            }

            adyacencia[vuelo.Origen].Add(vuelo);
        }

        // Encontrar ruta más barata usando el algoritmo de Dijkstra
        public void EncontrarRutaMasBarata(string origen, string destino)
        {
            if (!adyacencia.ContainsKey(origen) || !adyacencia.ContainsKey(destino))
            {
                Console.WriteLine("Una de las ciudades no existe en la base de datos.");
                return;
            }

            var distancias = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var previos = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var rutaVuelos = new Dictionary<string, Vuelo>(StringComparer.OrdinalIgnoreCase);
            var nodosNoVisitados = new List<string>(adyacencia.Keys);

            foreach (var nodo in adyacencia.Keys)
            {
                distancias[nodo] = decimal.MaxValue;
                previos[nodo] = null;
            }

            distancias[origen] = 0;

            while (nodosNoVisitados.Count > 0)
            {
                // Extraer el nodo con la menor distancia
                nodosNoVisitados.Sort((x, y) => distancias[x].CompareTo(distancias[y]));
                var actual = nodosNoVisitados.First();
                nodosNoVisitados.Remove(actual);

                if (actual == destino || distancias[actual] == decimal.MaxValue)
                {
                    break;
                }

                foreach (var vuelo in adyacencia[actual])
                {
                    var vecino = vuelo.Destino;
                    var costoAlternativo = distancias[actual] + vuelo.Precio;

                    if (costoAlternativo < distancias[vecino])
                    {
                        distancias[vecino] = costoAlternativo;
                        previos[vecino] = actual;
                        rutaVuelos[vecino] = vuelo;
                    }
                }
            }

            if (distancias[destino] == decimal.MaxValue)
            {
                Console.WriteLine($"No hay ruta disponible entre {origen} y {destino}.");
                return;
            }

            // Reconstruir la ruta
            var ruta = new List<Vuelo>();
            string? pasoActual = destino;

            while (pasoActual != null && previos[pasoActual] != null)
            {
                ruta.Add(rutaVuelos[pasoActual]);
                pasoActual = previos[pasoActual];
            }

            ruta.Reverse();

            Console.WriteLine($"\nRESULTADOS DE BÚSQUEDA: {origen.ToUpper()} -> {destino.ToUpper()} ");
            
            // Buscar vuelo directo más barato
            var vuelosDirectos = adyacencia[origen]
                                .Where(v => v.Destino.Equals(destino, StringComparison.OrdinalIgnoreCase))
                                .OrderBy(v => v.Precio)
                                .ToList();

            Console.WriteLine("\nVUELO DIRECTO MÁS BARATO");
            if (vuelosDirectos.Any())
            {
                Console.WriteLine(vuelosDirectos.First());
            }
            else
            {
                Console.WriteLine("No existen vuelos directos para esta ruta.");
            }

            // Mostrar la ruta más barata con escalas
            Console.WriteLine("\nRUTA MÁS BARATA - Puede incluir escalas");
            if (ruta.Count == 1)
            {
                Console.WriteLine("La ruta más barata es el vuelo directo:");
                Console.WriteLine(ruta[0]);
            }
            else
            {
                foreach (var paso in ruta)
                {
                    Console.WriteLine(paso);
                }
            }
            Console.WriteLine($"Costo Total de esta ruta: ${distancias[destino]}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string rutaArchivo = "vuelos.json";
            List<Vuelo> vuelos = new List<Vuelo>();

            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string jsonInfo = File.ReadAllText(rutaArchivo);
                    vuelos = JsonSerializer.Deserialize<List<Vuelo>>(jsonInfo) ?? new List<Vuelo>();
                }
                else
                {
                    Console.WriteLine($"No se encontró la base de datos de vuelos: {rutaArchivo}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error leyendo la base de datos de vuelos: " + ex.Message);
                return;
            }

            ArbolVuelos arbol = new ArbolVuelos();
            GrafoVuelos grafo = new GrafoVuelos();

            foreach (var vuelo in vuelos)
            {
                arbol.Insertar(vuelo);
                grafo.AgregarVuelo(vuelo);
            }

            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\nSISTEMA DE GESTIÓN DE VUELOS:\n");
                Console.WriteLine("1. Ver todos los vuelos");
                Console.WriteLine("2. Buscar vuelos por debajo de un precio");
                Console.WriteLine("3. Encontrar la ruta más barata entre dos ciudades");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine() ?? string.Empty;

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("\nTodos los Vuelos:");
                        foreach (var v in vuelos)
                        {
                            Console.WriteLine(v);
                        }
                        break;
                    case "2":
                        Console.Write("Ingrese el precio máximo deseado: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal precioMaximo))
                        {
                            arbol.MostrarVuelosBaratos(precioMaximo);
                        }
                        else
                        {
                            Console.WriteLine("Precio inválido.");
                        }
                        break;
                    case "3":
                        Console.Write("Ingrese ciudad de origen: ");
                        string origen = Console.ReadLine() ?? string.Empty;
                        Console.Write("Ingrese ciudad de destino: ");
                        string destino = Console.ReadLine() ?? string.Empty;
                        grafo.EncontrarRutaMasBarata(origen, destino);
                        break;
                    case "4":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }
    }
}
