using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using extra;

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
            var pathLines = @"C:\Datos\SArandomcost.csv";
            var pathTimes = @"C:\Datos\SArandomtime.csv";

            TimeSpan start;
            TimeSpan stop;
            var timePM = 0.0;
            var timeLP = 0.0;
            var timeMin = 0.0;

            var costesPM = new List<int>();
            var costesLP = new List<int>();


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
                        costesPM.Add(util.simulatedAnnealing(ref aux, A, n, 3, 0.97).Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timePM = stop.Subtract(start).TotalMilliseconds;


                    //Costes LP
                    costesLP = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (var it = 0; it < maxIt; it++)
                    {
                        var aux = util.quickSort(A, n, util.compareObjects);
                        costesLP.Add(util.simulatedAnnealing(ref aux, A, n, 3 , 0.97).Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeLP = stop.Subtract(start).TotalMilliseconds;

                    //Costes random
                    var costesRandom = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (var it = 0; it < maxIt; it++)
                    {
                        var aux = util.randomOrder(n);
                        costesRandom.Add(util.simulatedAnnealing(ref aux, A, n, 3, 0.97).Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    var timeRandom = stop.Subtract(start).TotalMilliseconds;

                    //Coste mínimo para esta combinación
                    start = new TimeSpan(DateTime.Now.Ticks);
                    var auxiliar = util.minCosteParallel(A, n);
                    //minCoste = util.coste(ref auxiliar, A, n);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;
                    //Console.WriteLine("minCoste creados");
                    //var auxiliar = 0;


                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesPM,  ref costesLP, ref costesRandom, auxiliar.Item2);
                    añadirTiempo(times, n, pp,timePM/maxIt ,timeLP/maxIt, timeRandom/maxIt ,timeMin);

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

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeInsertion, double timeRandom, double timeMin)
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
            time.Append(timeRandom + ";");
            time.Append(timeMin);

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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesSelection, ref List<int> costesInsertion, ref List<int> costesRandom, int minCoste)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes selection
            costesSelection.ForEach(coste => line.Append(coste + ";"));
            //Costes insertion
            costesInsertion.ForEach(coste => line.Append(coste + ";"));
            costesRandom.ForEach(coste => line.Append(coste + ";"));
            line.Append(minCoste + ",");

            lines.Add(line.ToString());
        }
    }
}
