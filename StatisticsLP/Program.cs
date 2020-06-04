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

            //In the next line you need to write the number of times you want to execute the non-deterministic algorithms (Minimals, MinimalRandom...)
            var maxIt = 50;

            //The next three variables are needed to store all the lines to be written to a .csv file
            var lines = new List<String>();
            var times = new List<String>();
            var As = new List<String>();

            //Paths of the -cvs files to be written
            var pathLines = @"YOUR_PATH.csv";
            var pathTimes = @"YOUR_PATH.csv";
            var pathA = @"YOUR_PATH.csv";

            //Varaibles needed to measure the time
            TimeSpan start;
            TimeSpan stop;


            //####################     IMPORTANT        ##############################
            //For each algorithm you want to test, you need to create a variable to store its time and its costs. Ex: below.
            //Times
            var timePM = 0.0;
            var timeLP = 0.0;
            var timeMin = 0.0;
            var timeQuickSort = 0.0;
            var timePMSA = 0.0;
            var timePMSAB = 0.0;
            //Costs
            var costesPM = new List<int>();
            var costesLP = new List<int>();
            var costesPMSA = new List<int>();
            var costesPMR = new List<int>();
            var costesQuickSort = new List<int>();
            var costesPMSAB = new List<int>();

            //Vector that stores all the posets of each iteration
            var vectorPosets = new List<List<bool>>();


            //Objects
            for (int n = 3; n <= 12; n++)   // Here you can change the size of the posets. Be aware that for MinCosteParallel, n=13 would take more than 12 hours.
            {
                //Posets
                for (int pp = 2; pp <= 50; pp++)    // Here you can change the number of posets to be aggregated
                {
                    //###################################            COMPULSORY PART       ######################################
                    // posets
                    vectorPosets = new List<List<bool>>();
                    for (int i = 0; i < pp; i++)
                    {
                        var poset = util.randomPoset(n);
                        poset = util.randomPermutation(poset, n);
                        vectorPosets.Add(poset);
                    }
                    //Console.WriteLine("Posets creados");      // FLOW EXECUTION

                    //Vector A con la suma de todos los posets
                    var A = util.sumaPosets(vectorPosets, n);           // Here, matrix A is computed
                    //Console.WriteLine("A creados");           // FLOW EXECUTION


                    //###################################            CUSTOMIZABLE PART       ######################################
                    /*
                     * Now, for each algorithm you want to test you need to:
                     *      1. Start measuring the time
                     *      2. Do a for loop: inside the loop you need to execute the algorithm and add its cost to its costs vector
                     *      3. Stop measuring the time
                     *      4. Compute time
                     *      
                     *  Currently, you can test: Minimals, QuickSort, Minimals+SA low temperature, Minimals+SA high temperature, Linear programming, MinCost MT
                     */

                    // PraggMinimals
                    costesPM = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);       // Start measuring time
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.praggMinimals(ref A, n);     // Algorithm
                        var coste = util.coste(ref aux, A, n);      // Cost
                        costesPM.Add(coste);                        // Add cost     
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);        // Stop measuring time
                    timePM = stop.Subtract(start).TotalMilliseconds;// Compute time

                    // quickSort
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

                    // PraggMinimals + SA baja
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

                    // PraggMinimals + SA alta
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
                    var auxiliar = util.linearProgrammingB(ref A, n);
                    for(var i=0; i<maxIt; i++) costesLP.Add(auxiliar.Item2);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeLP = stop.Subtract(start).TotalMilliseconds;

                    //Minimum cost
                    start = new TimeSpan(DateTime.Now.Ticks);
                    auxiliar = util.minCosteParallel(A, n);     // Multi-thread
                    //minCoste = util.coste(ref auxiliar, A, n);    // Single-thread
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("minCoste creados");        // FLOW EXECUTION


                    // ###########################################      COMPULSORY      ######################################################
                    añadirMatrizA(ref As, n, pp, ref A);                                                                                                    // Add matrix A to the global vector
                    añadirCombinación(ref lines, n, pp, ref costesPM, ref costesQuickSort, ref costesPMSA, ref costesPMSAB, ref costesLP, auxiliar.Item2);  // Add costs to the global vector of costs
                    añadirTiempo(times, n, pp, timePM/maxIt, timeQuickSort/maxIt, timePMSA/maxIt, timePMSAB/maxIt, timeLP, timeMin);                        // Add times to the global vector of times

                    Console.WriteLine("DONE--> N = {0} PP = {1}", n, pp);   // FLOW EXECUTION
                }
            }

            // ##############################################   CREATE THE CSV FILES       ######################################################
            //COSTS
            crearArchivoCsv(pathLines, lines);
            //TIMES
            crearArchivoCsv(pathTimes, times);
            //As
            crearArchivoCsv(pathA, As);
            Console.ReadLine();
        }

        // ###############################         AUXILIAR METHODS TO CREATE THE CSV FILES          ########################################3

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
