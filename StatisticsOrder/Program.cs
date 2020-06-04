using extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsOrder
{
    class Program
    {

        private static void printMatrix<T>(List<T> a, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(a[i + j * n] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("******************************");
        }

        static void Main(string[] args)
        {

            var util = new Posets();

            var maxIt = 50;

            var lines = new List<String>();
            var times = new List<String>();
            var posets = new List<String>();
            var pathLines = @"orderData.csv";
            var pathTimes = @"orderTimes.csv";

            TimeSpan start;
            TimeSpan stop;
            var timeSelection = 0.0;
            var timeInsertion = 0.0;
            var timeMin = 0.0;
            var timeQuickSort = 0.0;
            var timeBubble = 0.0;
            var timeMergeSort = 0.0;


            var costesSelection = new List<int>();
            var costesInsertion = new List<int>();
            var costesQuickSort = new List<int>();
            var costesBubble = new List<int>();
            var costesMergeSort = new List<int>();
            var minCoste = 0;
            var vectorPosets = new List<List<bool>>();


            //Objetos
            for (int n = 3; n <= 12; n++)
            {
                //Posets
                for (int pp = 2; pp <= 50; pp++)
                {
                    //Vector para los posets
                    vectorPosets = new List<List<bool>>();
                    for (int i = 0; i < pp; i++)
                    {
                        var poset = util.randomPoset(n);
                        poset = util.randomPermutation(poset, n);
                        vectorPosets.Add(poset);
                    }
                    //Console.WriteLine("Posets creados");

                    //Vector A con la suma de todos los posets
                    var A = util.sumaPosets(vectorPosets, n);
                    //Console.WriteLine("A creados");

                    //Costes para el algoritmo selection
                    costesSelection = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.selection(A, n,util.compareObjects);
                        var coste = util.coste(ref aux, A, n);
                        costesSelection.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeSelection = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("Selection creados");


                    //Costes para el algoritmo insertion
                    costesInsertion = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.insertion(A, n,util.compareObjects);
                        var coste = util.coste(ref aux, A, n);
                        costesInsertion.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeInsertion = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("Insertion creados");


                    //Costes para el algoritmo quickSort
                    costesQuickSort = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.quickSort(A, n, util.compareObjects);
                        var coste = util.coste(ref aux, A, n);
                        costesQuickSort.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeQuickSort = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("Quicksort creados");

                    //Costes para el algoritmo bubble
                    costesBubble = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.bubble(A, n, util.compareObjects);
                        var coste = util.coste(ref aux, A, n);
                        costesBubble.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeBubble = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("Bubble creados");

                    //Costes para el algoritmo quickSort
                    costesMergeSort = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.mergeSort(A, n, util.compareObjects);
                        var coste = util.coste(ref aux, A, n);
                        costesQuickSort.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeQuickSort = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("MergeSort creados");

                    //Coste mínimo para esta combinación
                    start = new TimeSpan(DateTime.Now.Ticks);
                    var auxiliar = util.minCosteParallel(A, n);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("minCoste creados");


                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesSelection, ref costesInsertion,ref costesQuickSort,ref costesBubble, ref costesMergeSort, auxiliar);
                    añadirTiempo(times, n, pp, timeSelection, timeInsertion, timeQuickSort,timeBubble,timeMergeSort, timeMin);

                    Console.WriteLine("DONE--> N = {0} PP = {1}", n, pp);
                }
            }

            //Archivo csv con los costes
            crearArchivoCsv(pathLines, lines);
            //Archivo csv con los tiempos
            crearArchivoCsv(pathTimes, times);
            //util.crearArchivo();
            Console.ReadLine();
        }

        private static void añadirPoset(List<string> posets, List<bool> poset)
        {
            var sb = new StringBuilder();
            poset.ForEach(element =>
            {
                sb.Append(element + ";");
            });
            sb.AppendLine("\n####################################################");
            posets.Add(sb.ToString());
        }

        private static void añadirParcialPosets(List<string> posets, List<List<bool>> vps)
        {
            var sb = new StringBuilder();
            vps.ForEach(pp =>
            {
                pp.ForEach(element =>
                {
                    sb.Append(element + ";");
                });
                sb.AppendLine("\n----------------------------------------------------------");
            });
            sb.AppendLine("********************************************************************");
            posets.Add(sb.ToString());
        }

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeInsertion, double timequickSort, double timeBubble, double timeMergeSort, double timeMin)
        {
            StringBuilder time = new StringBuilder();

            //Número de objetos
            time.Append(n + ";");
            //Número de posets
            time.Append(pp + ";");
            //Tiempo para selection
            time.Append(timeSelection + ";");
            //Tiempo para insertion
            time.Append(timeInsertion + ";");
            //Tiempo para quicksort
            time.Append(timequickSort + ";");
            //Tiempo para bubble
            time.Append(timeBubble + ";");
            //Tiempo para merge
            time.Append(timeMergeSort + ";");
            time.Append(timeMin + ",");

            times.Add(time.ToString());
        }

        private static void crearArchivoCsv(string path, List<string> lines)
        {
            using (var file = new StreamWriter(path))
            {
                lines.ForEach(line => file.WriteLine(line));
                file.Flush();
            }
        }

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, ref List<int> costesPMR, ref List<int> costesQuickSort, ref List<int> costesBubble, ref List<int> costesMergeSort, int minCoste)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes para selection
            costesPM.ForEach(coste => line.Append(coste + ";"));
            //Costes para insertion
            costesPMR.ForEach(coste => line.Append(coste + ";"));
            //Costes para quicksort
            costesQuickSort.ForEach(coste => line.Append(coste + ";"));
            //Costes bubble
            costesBubble.ForEach(coste => line.Append(coste + ";"));
            //Costes merge
            costesMergeSort.ForEach(coste => line.Append(coste + ";"));
            line.Append(minCoste + ",");

            lines.Add(line.ToString());
        }
    }
}

