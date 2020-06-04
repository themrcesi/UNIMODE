using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using extra;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var util = new Posets();
            foreach (var p in util.Permutate(new List<int> { 1, 2, 3 }, 3))
            {
                p.ForEach(v => Console.Write(v));
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        private static void crearArchivoCsv(string path, List<string> lines)
        {
            using (var file = new StreamWriter(path))
            {
                lines.ForEach(line => file.WriteLine(line));
                file.Flush();
            }
        }

        private static void añadirCombinación(List<string> lines, int n, int pp, List<int> costesPM, List<int> costesPMR, int minCoste)
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
            line.Append(minCoste + ",");

            lines.Add(line.ToString());
        }




        private static int countTrue(List<bool> poset)
        {
            return poset.Aggregate(0, (i, element) =>
            {
                if (element) i++;
                return i;
            });
        }

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

        private static List<List<bool>> getVectorPosets(List<List<int>> vps)
        {
            var vectorPosets = new List<List<bool>>();
            vps.ForEach(
                poset =>
                {
                    var list = new List<bool>();
                    poset.ForEach(
                        number =>
                        {
                            if (number == 1)
                                list.Add(true);
                            else
                                list.Add(false);
                        });
                    vectorPosets.Add(list);
                });
            return vectorPosets;
        }
    }

    class ListValueComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            return x.SequenceEqual(y, EqualityComparer<T>.Default);
        }

        public int GetHashCode(List<T> obj)
        {
            return obj.Aggregate(0, (total, next) => total ^ next.GetHashCode());
        }
    }
}
