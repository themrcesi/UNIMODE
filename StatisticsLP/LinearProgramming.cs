//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using extra;

//namespace StatisticsLP
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var util = new Posets();

//            var maxIt = 50;

//            var lines = new List<String>();
//            var times = new List<String>();
//            var pathLines = @"C:\Datos\costesLP.csv";
//            var pathTimes = @"C:\Datos\timesLP.csv";

//            TimeSpan start;
//            TimeSpan stop;
//            var timeLP = 0.0;
//            var timeMin = 0.0;

//            var costesLP = new List<int>();

//            var vectorPosets = new List<List<bool>>();


//            //Objetos
//            for (int n = 3; n <= 12; n++)
//            {
//                //Posets
//                for (int pp = 2; pp <= 50; pp++)
//                {
//                    //Vector para los posets
//                    vectorPosets = new List<List<bool>>();
//                    for (int i = 0; i < pp; i++)
//                    {
//                        var poset = util.randomPoset(n);
//                        poset = util.randomPermutation(poset, n);
//                        vectorPosets.Add(poset);
//                    }
//                    //Console.WriteLine("Posets creados");

//                    //Vector A con la suma de todos los posets
//                    var A = util.sumaPosets(vectorPosets, n);
//                    //Console.WriteLine("A creados");

//                    //Costes LP
//                    costesLP = new List<int>();
//                    start = new TimeSpan(DateTime.Now.Ticks);
//                    for (var it = 0; it < maxIt; it++)
//                    {
//                        var aux = util.linearProgrammingB(ref A, n);
//                        costesLP.Add(aux.Item2);
//                    }
//                    stop = new TimeSpan(DateTime.Now.Ticks);
//                    timeLP = stop.Subtract(start).TotalMilliseconds;

//                    start = new TimeSpan(DateTime.Now.Ticks);
//                    var auxiliar = util.minCosteParallel(A, n);
//                    stop = new TimeSpan(DateTime.Now.Ticks);
//                    timeMin = stop.Subtract(start).TotalMilliseconds;


//                    //Añadir la linea al StringBuilder
//                    añadirCombinación(ref lines, n, pp, costesLP, auxiliar.Item2);
//                    añadirTiempo(times, n, pp, timeLP, timeMin * 50);

//                    Console.WriteLine("DONE--> N = {0} PP = {1}", n, pp);
//                }
//            }

//            //Archivo csv con los costes
//            crearArchivoCsv(pathLines, lines);
//            //Archivo csv con los tiempos
//            crearArchivoCsv(pathTimes, times);
//            //Archivo csv con las as
//            //util.crearArchivo();
//            Console.ReadLine();
//        }

//        private static void añadirMatrizA(ref List<string> As, int n, int pp, ref List<int> a)
//        {
//            StringBuilder line = new StringBuilder();

//            //Número de objetos
//            line.Append(n + ";");
//            //Número de posets
//            line.Append(pp + ";");
//            //Costes selection
//            a.ForEach(coste => line.Append(coste + ";"));

//            As.Add(line.ToString());
//        }

//        private static void añadirPoset(List<string> posets, List<bool> poset)
//        {
//            var sb = new StringBuilder();
//            poset.ForEach(element =>
//            {
//                sb.Append(element + ";");
//            });
//            sb.AppendLine("\n####################################################");
//            posets.Add(sb.ToString());
//        }

//        private static void añadirParcialPosets(List<string> posets, List<List<bool>> vps)
//        {
//            var sb = new StringBuilder();
//            vps.ForEach(pp =>
//            {
//                pp.ForEach(element =>
//                {
//                    sb.Append(element + ";");
//                });
//                sb.AppendLine("\n----------------------------------------------------------");
//            });
//            sb.AppendLine("********************************************************************");
//            posets.Add(sb.ToString());
//        }

//        private static void añadirTiempo(List<string> times, int n, int pp, double timeLP, double timeMin)
//        {
//            StringBuilder time = new StringBuilder();

//            //Número de objetos
//            time.Append(n + ";");
//            //Número de posets
//            time.Append(pp + ";");
//            //Tiempo para LP
//            time.Append(timeLP + ";");
//            time.Append(timeMin);

//            times.Add(time.ToString());
//        }

//        private static void crearArchivoCsv(string path, List<string> lines)
//        {
//            using (var file = new StreamWriter(path))
//            {
//                lines.ForEach(line => file.WriteLine(line));
//                file.Flush();
//            }
//        }

//        private static void añadirCombinación(ref List<string> lines, int n, int pp, List<int> costesLP, int minCoste)
//        {
//            StringBuilder line = new StringBuilder();

//            //Número de objetos
//            line.Append(n + ";");
//            //Número de posets
//            line.Append(pp + ";");
//            //Costes LP
//            costesLP.ForEach(coste => line.Append(coste + ";"));
//            line.Append(minCoste + ",");

//            lines.Add(line.ToString());
//        }
//    }
//}
