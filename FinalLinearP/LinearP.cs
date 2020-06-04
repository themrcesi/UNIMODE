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
            var pathLines = @"D:\datosCesar\costesLinearProgramming.csv";
            var pathTimes = @"D:\datosCesar\timesLinearProgramming.csv";

            TimeSpan start;
            TimeSpan stop;
            var timeM = 0.0;
            var timeMin = 0.0;

            var costesM = new List<int>();
            var costesMin = new List<int>();


            var vectorPosets = new List<List<bool>>();

            //Objetos
            for (int n = 3; n <= 8; n++)
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
                        var coste = util.linearProgrammingB(ref A, n);
                        costesM.Add(coste.Item2);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeM = stop.Subtract(start).TotalMilliseconds;

                    var auxiliar = util.minCosteParallel(A, n);
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        util.minCosteParallel(A, n);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;


                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesM, auxiliar.Item2);
                    añadirTiempo(times, n, pp, timeM, timeMin);

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

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeMin)
        {
            StringBuilder time = new StringBuilder();

            //Número de objetos
            time.Append(n + ";");
            //Número de posets
            time.Append(pp + ";");
            //Tiempo para selection
            time.Append(timeSelection + ";");
            //Tiempo para insertion
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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, int minCoste)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes para selection
            costesPM.ForEach(coste => line.Append(coste + ";"));
            //Costes para insertion
            line.Append(minCoste + ";");

            lines.Add(line.ToString());
        }
    }
}

