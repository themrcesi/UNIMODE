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
            //################################################IMPORTANTE###################################
            //
            //      PRIMERA EJECUCIÓN N=3 HASTA N=10, PONER NOMBRE DE FICHERO ACABADO EN 10
            //      SEGUNDA N=11, PONER NOMBRE DE FICHERO ACABADO EN 11
            //      TERCERA N=12, PONER NOMBRE DE FICHERO ACABADO EN 12
            var util = new Posets();

            var maxIt = 50;

            var lines = new List<String>();
            var times = new List<String>();
            var posets = new List<String>();
            var pathLines = @"C:\Datos\costesOrder12.csv";
            var pathTimes = @"C:\Datos\timesOrder12.csv";

            TimeSpan start;
            TimeSpan stop;
            var timeST = 0.0;
            var timeMT = 0.0;

            var costesST = new List<int>();
            var costesMT = new List<int>();

            var vectorPosets = new List<List<bool>>();


            //Objetos
            for (int n = 12; n <= 12; n++)
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

                    costesST = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    var coste = util.minCoste(A, n);
                    costesST.Add(coste.Item2);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeST = stop.Subtract(start).TotalMilliseconds;
                    for (var i = 0; i < 49; i++) costesST.Add(coste.Item2);

                    costesMT = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    coste = util.minCosteParallel(A, n);
                    costesMT.Add(coste.Item2);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMT = stop.Subtract(start).TotalMilliseconds;
                    for (var i = 0; i < 49; i++) costesMT.Add(coste.Item2);

                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesST, ref costesMT);
                    añadirTiempo(times, n, pp, timeST, timeMT);

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

        private static void añadirTiempo(List<string> times, int n, int pp, double timeSelection, double timeInsertion)
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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, ref List<int> costesPMR)
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

            lines.Add(line.ToString());
        }
    }
}

