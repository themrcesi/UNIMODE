using extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StatisticsLP
{
    class Program
    {
        static void Main(string[] args)
        {
            var util = new Posets();

            var maxIt = 50;

            var lines = new List<String>();
            var times = new List<String>();
            var As = new List<String>();
            var pathLines = @"C:\Datos\costesGeneral.csv";
            var pathTimes = @"C:\Datos\timesGeneral.csv";

            TimeSpan start;
            TimeSpan stop;
            var timePM = 0.0;
            var timeLP = 0.0;
            var timeMin = 0.0;
            var timeSelection = 0.0;
            var timeInsertion = 0.0;
            var timeQuickSort = 0.0;
            var timeBubble = 0.0;
            var timeMergeSort = 0.0;
            var timePMR = 0.0;
            var timePMSA = 0.0;
            var timePMSAB = 0.0;

            var costesPM = new List<int>();
            var costesLP = new List<int>();
            var costesPMSA = new List<int>();
            var costesPMR = new List<int>();
            var costesInsertion = new List<int>();
            var costesSelection = new List<int>();
            var costesQuickSort = new List<int>();
            var costesMergeSort = new List<int>();
            var costesBubble = new List<int>();
            var costesPMSAB = new List<int>();
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

                    //Costes para el algoritmo PraggMinimals
                    costesPM = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.praggMinimals(ref A, n);
                        var coste = util.coste(ref aux, A, n);
                        costesPM.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timePM = stop.Subtract(start).TotalMilliseconds;

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

                    //Costes para el algoritmo PraggMinimals + SA baja
                    costesPMSA = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.praggMinimals(ref A, n);
                        var sa = util.simulatedAnnealing(ref aux, A, n, 3, 0.97);
                        costesPMSA.Add(sa.Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timePMSA = stop.Subtract(start).TotalMilliseconds;

                    //Costes para el algoritmo PraggMinimals + SA alta
                    costesPMSAB = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.praggMinimals(ref A, n);
                        var sa = util.simulatedAnnealing(ref aux, A, n, 100, 0.999);
                        costesPMSAB.Add(sa.Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timePMSAB = stop.Subtract(start).TotalMilliseconds;

                    //Costes LP
                    costesLP = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (var it = 0; it < maxIt; it++)
                    {
                        var aux = util.linearProgrammingB(ref A, n);
                        costesLP.Add(aux.Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeLP = stop.Subtract(start).TotalMilliseconds;

                    //Coste mínimo para esta combinación
                    start = new TimeSpan(DateTime.Now.Ticks);
                    var auxiliar = util.minCosteParallel(A, n);
                    //minCoste = util.coste(ref auxiliar, A, n);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("minCoste creados");
                    //var auxiliar = 0;


                    //Añadir la linea al StringBuilder
                    añadirMatrizA(ref As, n, pp, ref A);
                    añadirCombinación(ref lines, n, pp, ref costesPM, ref costesQuickSort, ref costesPMSA, ref costesPMSAB, ref costesLP, auxiliar.Item2);
                    añadirTiempo(times, n, pp, timePM/maxIt, timeQuickSort/maxIt, timePMSA/maxIt, timePMSAB/maxIt, timeLP, timeMin);

                    Console.WriteLine("DONE--> N = {0} PP = {1}", n, pp);
                }
            }

            //Archivo csv con los costes
            crearArchivoCsv(pathLines, lines);
            //Archivo csv con los tiempos
            crearArchivoCsv(pathTimes, times);
            //Archivo csv con las as
            //util.crearArchivo();
            Console.ReadLine();
        }

        private static void añadirMatrizA(ref List<string> As, int n, int pp, ref List<int> a)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes selection
            a.ForEach(coste => line.Append(coste + ";"));

            As.Add(line.ToString());
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

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeInsertion, double timeQuickSort, double timeBubble, double timeMergeSort, double timePM)
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
            time.Append(timeQuickSort + ";");
            //Tiempo para bubble
            time.Append(timeBubble + ";");
            //Tiempo para mergesort
            time.Append(timeMergeSort + ";");
            //Tiempo para minimals
            time.Append(timePM + ";");

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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, ref List<int> costesPMR, ref List<int> costesPMSA, ref List<int> costesPMSAB, ref List<int> costesLP, int minCoste)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes minimals
            costesPM.ForEach(coste => line.Append(coste + ";"));
            //Costes minimals random
            costesPMR.ForEach(coste => line.Append(coste + ";"));
            //Costes minimals + SA baja
            costesPMSA.ForEach(coste => line.Append(coste + ";"));
            //Costes minimals + SA alta
            costesPMSAB.ForEach(coste => line.Append(coste + ";"));
            //Costes LP
            costesLP.ForEach(coste => line.Append(coste + ";"));
            line.Append(minCoste + ",");

            lines.Add(line.ToString());
        }
    }
}
