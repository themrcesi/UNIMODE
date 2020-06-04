using extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalStatistic
{
    class Program
    {

     

        static void Main(string[] args)
        {
            var util = new Posets();

            var maxIt = 50;

            var lines = new List<String>();
            var times = new List<String>();
            var posets = new List<String>();
            var pathLines = @"C:\Datos\costesSorting.csv";
            var pathTimes = @"C:\Datos\timesSorting.csv";

            TimeSpan start;
            TimeSpan stop;
            var timeM = 0.0;
            var timeMR = 0.0;
            var timeMin = 0.0;
            var timeInsertion = 0.0;
            var timeQuickSort = 0.0;
            var timeMergesort = 0.0;

            var costesM = new List<int>();
            var costesMR = new List<int>();
            var costesMin = new List<int>();
            var costesInsertion = new List<int>();
            var costesQuickSort = new List<int>();
            var costesMergesort = new List<int>();


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

                    costesM = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.bubble(A, n, util.compareObjects);
                        costesM.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeM = stop.Subtract(start).TotalMilliseconds;

                    costesMR = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.selection(A, n, util.compareObjects);
                        costesMR.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMR = stop.Subtract(start).TotalMilliseconds;

                    costesInsertion = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.insertion(A, n, util.compareObjects);
                        costesInsertion.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeInsertion = stop.Subtract(start).TotalMilliseconds;

                    costesQuickSort = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.quickSort(A, n, util.compareObjects);
                        costesQuickSort.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeQuickSort = stop.Subtract(start).TotalMilliseconds;

                    costesMergesort = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.mergeSort(A, n, util.compareObjects);
                        costesMergesort.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMergesort = stop.Subtract(start).TotalMilliseconds;

                    var costesMB = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.bubble(A, n, util.compareObjectsB);
                        costesMB.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeMB = stop.Subtract(start).TotalMilliseconds;

                    var costesMRB = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.selection(A, n, util.compareObjectsB);
                        costesMRB.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeMRB = stop.Subtract(start).TotalMilliseconds;

                    var costesInsertionB= new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.insertion(A, n, util.compareObjectsB);
                        costesInsertionB.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeInsertionB = stop.Subtract(start).TotalMilliseconds;

                    var costesQuickSortB = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.quickSort(A, n, util.compareObjectsB);
                        costesQuickSortB.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeQuickSortB = stop.Subtract(start).TotalMilliseconds;

                    var costesMergesortB = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var coste = util.mergeSort(A, n, util.compareObjectsB);
                        costesMergesortB.Add(util.coste(ref coste, A, n));
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeMergesortB = stop.Subtract(start).TotalMilliseconds;

                    start = new TimeSpan(DateTime.Now.Ticks);
                    var auxiliar = util.minCosteParallel(A, n);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;


                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesM, ref costesMR, ref costesInsertion, ref costesQuickSort, ref costesMergesort, ref costesMB, ref costesMRB, ref costesInsertionB, ref costesQuickSortB, ref costesMergesortB, auxiliar.Item2);
                    añadirTiempo(times, n, pp, timeM, timeMR, timeInsertion, timeQuickSort, timeMergesort, timeMB, timeMRB, timeInsertionB, timeQuickSortB, timeMergesortB, timeMin *50);

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

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeInsertion, double timeI, double timeQ, double timeM, double timeSelectionB, double timeInsertionB, double timeIB, double timeQB, double timeMB, double timeMin)
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
            time.Append(timeI + ";");
            time.Append(timeQ + ";");
            time.Append(timeM + ";");

            time.Append(timeSelectionB + ";");
            //Tiempo para insertion
            time.Append(timeInsertionB + ";");
            time.Append(timeIB + ";");
            time.Append(timeQB + ";");
            time.Append(timeMB + ";");

            time.Append(timeMin + ";");

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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, ref List<int> costesPMR, ref List<int> costesinsertion, ref List<int> costesquick, ref List<int> costesMerge, ref List<int> costesPMB, ref List<int> costesPMRB, ref List<int> costesinsertionB, ref List<int> costesquickB, ref List<int> costesMergeB, int minCoste)
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
            costesinsertion.ForEach(coste => line.Append(coste + ";"));
            costesquick.ForEach(coste => line.Append(coste + ";"));
            costesMerge.ForEach(coste => line.Append(coste + ";"));
            costesPMB.ForEach(coste => line.Append(coste + ";"));
            //Costes para insertion
            costesPMRB.ForEach(coste => line.Append(coste + ";"));
            costesinsertionB.ForEach(coste => line.Append(coste + ";"));
            costesquickB.ForEach(coste => line.Append(coste + ";"));
            costesMergeB.ForEach(coste => line.Append(coste + ";"));
            line.Append(minCoste + ";");

            lines.Add(line.ToString());
        }
    }
}

