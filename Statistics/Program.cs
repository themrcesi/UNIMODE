using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using extra;

namespace Statistics
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
            var pathLines = @"D:\Documentos\Universidad\Prácticas\testData.csv";
            var pathTimes = @"D:\Documentos\Universidad\Prácticas\timesDataYES.csv";

            TimeSpan start;
            TimeSpan stop;
            var timePM = 0.0;
            var timePMR = 0.0;
            var timeMin = 0.0;

            var costesPM = new List<int>();
            var costesPMR = new List<int>();
            var minCoste = 0;
            var vectorPosets = new List<List<bool>>();

            util = new Posets();


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

                    //Vector A con la suma de todos los posets
                    var A = util.sumaPosets(vectorPosets, n);

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

                    //Costes para el algoritmo PraggMinimalsRandom
                    costesPMR = new List<int>();
                    start = new TimeSpan(DateTime.Now.Ticks);
                    for (int it = 0; it < maxIt; it++)
                    {
                        var aux = util.praggMinimalsRandom(ref A, n);
                        var coste = util.coste(ref aux, A, n);
                        costesPMR.Add(coste);
                    }
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timePMR = stop.Subtract(start).TotalMilliseconds;

                    //Coste mínimo para esta combinación
                    start = new TimeSpan(DateTime.Now.Ticks);
                    var auxiliar = util.minCosteParallel(A, n);
                    //minCoste = util.coste(ref auxiliar, A, n);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    timeMin = stop.Subtract(start).TotalMilliseconds;

                    //Añadir la linea al StringBuilder
                    añadirCombinación(ref lines, n, pp, ref costesPM, ref costesPMR, auxiliar);
                    añadirTiempo(times, n, pp, timePM, timePMR, timeMin);

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

        private static void añadirTiempo(List<string> times, int n, int pp, double timePM, double timePMR, double timeMin)
        {
            StringBuilder time = new StringBuilder();

            //Número de objetos
            time.Append(n + ";");
            //Número de posets
            time.Append(pp + ";");
            //Tiempo para PraggMinimals
            time.Append(timePM + ";");
            //Costes para PraggMinimalsRandom
            time.Append(timePMR + ";");
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

        private static void añadirCombinación(ref List<string> lines, int n, int pp, ref List<int> costesPM, ref List<int> costesPMR, int minCoste)
        {
            StringBuilder line = new StringBuilder();

            //Número de objetos
            line.Append(n + ";");
            //Número de posets
            line.Append(pp + ";");
            //Costes para PraggMinimals
            costesPM.ForEach(coste => line.Append(coste + ";"));
            //Costes para PraggMinimalsRandom
            costesPMR.ForEach(coste => line.Append(coste + ";"));
            line.Append(minCoste);

            lines.Add(line.ToString());
        }
    }
}
