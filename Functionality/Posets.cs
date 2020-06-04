using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.SolverFoundation.Common;
using System.Runtime.InteropServices;
using System.Reflection;
using LpSolveDotNet;
using System.Collections;

namespace extra
{
    
    public class Posets
    {

        static void Main(string[] args) {}
        Random rg = new Random();

        //#####################################################POSETS###############################################################

        /// <summary>
        /// Method to initialize a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<T> createListWithLimit<T>(T data, double size)
        {
            var returned = new List<T>();
            for (int i = 0; i < size; i++)
            {
                returned.Add(data);
            }
            return returned;
        }

        public List<bool> randomPermutation(List<bool> poset, int n)
        {
            var perm = createListWithLimit(0, n);
            for (int i = 0; i < n; i++)
            {
                perm[i] = i;
            }
            perm.randomShuffle(rg);
            return transforma(poset, perm, n);
        }

        private List<bool> transforma(List<bool> poset, List<int> perm, int n)
        {
            var returned = poset.Select(x => x).ToList();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    returned[i * n + j] = poset[perm[i] * n + perm[j]];
                }
            }
            return returned;
        }

        public List<bool> randomPoset(int n)
        {
            var poset = createListWithLimit(false, n * n);
            //Ponemos true los elementos de la diagonal
            for (int i = 0; i < n; i++)
            {
                poset[i * n + i] = true;
            }
            for (int i = 0; i < n; i++)
            {
                work(ref poset, i, n);
            }
            return poset;
        }

        private void work(ref List<bool> poset, int i, int n)
        {
            int u = 0;
            int w;
            var M = createListWithLimit(0, 50);
            var q = 2;
            var Q = createListWithLimit(false, n);
            while (q * q < n - i)
                q++;
            if (i == 0)
                u = 0;
            else
            {
                var aux = rg.Next(q);
                if (aux == 0)
                    u = findMax(ref poset, i, n, ref Q, ref M);
            }

            for (int j = 0; j < u; j++)
            {
                if (Q[j])
                    poset[M[j] * n + i] = true;
            }
            w = 1;
            while (w == 1)
            {
                w = 0;
                for (int j = 0; j < i; j++)
                {
                    if (poset[j * n + i])
                        for (int s = 0; s < i; s++)
                        {
                            if (poset[s * n + j] && !poset[s * n + i])
                            {
                                w = 1;
                                poset[s * n + i] = true;
                            }
                        }
                }
            }
        }

        private int findMax(ref List<bool> poset, int i, int n, ref List<bool> q, ref List<int> m)
        //Algoritmo para generar un poset aleatorio según descrito en 
        //RANDOM POSETS, LATTICES, AND LATTICES TERMS
        //Jaroslav Ježek, Václav Slavík
        //MATHEMATICA BOHEMICA 125 (2000) No. 2, 129–133
        {
            m = new List<int>();
            int s, a;
            int k = 0;
            for (int j = 0; j < i; j++)
            {
                s = 1;
                for (a = 0; a < i; a++)
                {
                    if (a != j && poset[j * n + a])
                        s = 0;
                }
                if (s == 1)
                {
                    m.Insert(k, j);
                    k++;
                }
            }
            a = rg.Next(k + 1);
            for (int j = 0; j < k; j++)
            {
                q[j] = false;
            }
            for (s = 0; s < a; s++)
            {
                var j = rg.Next(k);
                if (q[j])
                    s--;
                else
                    q[j] = true;
            }
            return k;
        }

        public List<int> randomOrder(int n)
        {
            var order = new List<int>();
            for (var i = 0; i < n; i++)
            {
                order.Add(i);
            }
            order.randomShuffle(rg);
            return order;
        }

        /// <summary>
        /// Method to compute the aggregation matrix
        /// </summary>
        /// <param name="vectorPosets"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public List<int> sumaPosets(List<List<bool>> vectorPosets, int n)
        {
            var A = createListWithLimit(0, n * n);
            vectorPosets.ForEach(
                poset =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            var pos = i + n * j;
                            if (poset[pos])
                            {
                                A[pos]++;
                            }
                        }
                    }
                });
            return A;
        }

        //#################################################COSTE##############################################

        /// <summary>
        /// Method to compute the cost of an aggregation
        /// </summary>
        /// <param name="orden">Particular poset</param>
        /// <param name="A">Aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <returns>Cost</returns>
        public int coste(ref List<int> orden, List<int> A, int n)
        //Devuelve el coste del orden según la matriz A que codifica:
        // La posición (i,j) está en i+j*n
        // La posición (i,j) indica en cuántos posets i es menor o igual que j 
        {
            var c = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var a = orden[i];
                    var b = orden[j];
                    c += A[b + n * a];
                }
            }
            return c;
        }

        /// <summary>
        /// Method to compute the minimum cost of partially ordered sets
        /// </summary>
        /// <param name="A">Aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <returns>The best aggregation and its cost</returns>
        public (List<int>,int) minCoste(List<int> A, int n)
        //Devuelve el orden de menor coste para la matriz A que codifica:
        // La posición (i,j) está en i+j*n
        // La posición (i,j) indica en cuántos posets i es menor o igual que j 
        //VERSIÓN SINGLE THREAD
        {
            var perm = createListWithLimit(-1, n);
            for (int i = 0; i < n; i++)
            {
                perm[i] = i;
            }
            var min = coste(ref perm, A, n);
            var max = min;
            var min_perm = perm;

            foreach (var com in Permutate(perm, perm.Count))
            {
                var l = com;
                var c = coste(ref l, A, n);
                if (c < min)
                {
                    min = c;
                    min_perm = com;
                }
                if (c > max)
                    max = c;
            }

            return (min_perm,min);
        }

        private void RotateRight(List<int> sequence, int count)
        { 
            var tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        /// <summary>
        /// Method to get all the permutations of a given sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<List<int>> Permutate(List<int> sequence, int count)
        {
            //https://www.codeproject.com/Articles/43767/A-C-List-Permutation-Iterator
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }

        public IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        /// <summary>
        /// Method to compute the minimum cost of an aggregation but this time parallely
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public (List<int>,int) minCosteParallel(List<int> A, int n)
        //Devuelve el orden de menor coste para la matriz A que codifica:
        // La posición (i,j) está en i+j*n
        // La posición (i,j) indica en cuántos posets i es menor o igual que j 
        {
            var perm = createListWithLimit(-1, n);
            for (int i = 0; i < n; i++)
            {
                perm[i] = i;
            }
            var min = coste(ref perm, A, n);
            var min_perm = perm;

            var costes = new List<int>();

            PermutationMixOuelletSaniSinghHuttunen.ExecuteForEachPermutationMT(
              perm.ToArray(),
              p =>
              {
                  var l = p.ToList();
                  var c = coste(ref l, A, n);
                  lock (costes)
                      if (c < min) min = c;
              });

            return (min_perm,min);
        }

        //#####################################################MINIMALS##########################################################################
        
        /// <summary>
        /// Minimals algorithm
        /// </summary>
        /// <param name="a">aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <returns>Aggregation</returns>
        public List<int> praggMinimals(ref List<int> a, int n)
        //Recibe una matriz de enteros que codifica la información
        //de todos los posets: 
        // La posición (i,j) está en i+j*n
        // La posición (i,j) indica en cuántos posets i es menor o igual que j  
        //Devuelve una ordenación que intenta minimizar el coste
        {
            var orden = new List<int>();
            var usado = createListWithLimit(false, n);
            //Calculamos elementos que tiene cada uno por encima y por debajo
            var up = createListWithLimit(0, n);
            var down = createListWithLimit(0, n);
            var bound = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    up[i] += a[j * n + i];
                    down[i] += a[i * n + j];
                    bound += a[i + j * n];
                }
            }
            //Buscamos los elementos con menos número de elementos por debajo
            for (int i = 0; i < n; i++)
            {
                var min = bound;
                for (int j = 0; j < n; j++)
                {
                    if (!usado[j])
                    {
                        if (down[j] < min)
                        {
                            min = down[j];
                        }
                    }
                }
                //Seleccionamos uno de los minimales
                var posibilidades = new List<int>();
                for (int j = 0; j < n; j++)
                {
                    if (down[j] == min && !usado[j]) //Minimal
                    {
                        for (int k = 0; k < up[j]; k++)
                        {
                            posibilidades.Add(j);
                        }
                    }
                }
                //Elemento escogido
                int chosen = posibilidades[rg.Next(posibilidades.Count)];
                //Eliminamos el elemento escogido
                usado[chosen] = true;
                for (int j = 0; j < n; j++)
                {
                    up[j] -= a[j + chosen * n];
                    down[j] -= a[chosen + j * n];
                }
                orden.Add(chosen);
            }
            return orden;
        }

        /// <summary>
        /// MinimalsRandom
        /// </summary>
        /// <param name="a">Aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <returns>Aggregation</returns>
        public List<int> praggMinimalsRandom(ref List<int> a, int n)
        //Recibe una matriz de enteros que codifica la información
        //de todos los posets: 
        // La posición (i,j) está en i+j*n
        // La posición (i,j) indica en cuántos posets i es menor o igual que j  
        //Devuelve una ordenación que intenta minimizar el coste
        {
            var orden = new List<int>();
            var usado = createListWithLimit(false, n);
            //Calculamos elementos que tiene cada uno por encima y por debajo
            var up = createListWithLimit(-1, n);
            var down = createListWithLimit(-1, n);
            var bound = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //up[i] += a[i * n + j];
                    down[i] += a[i * n + j];
                    bound += a[j + i * n];
                }
            }
            //Buscamos los elementos con menos número de elmentos por debajo
            for (int i = 0; i < n; i++)
            {
                var min = bound;
                for (int j = 0; j < n; j++)
                {
                    if (!usado[j])
                    {
                        if (down[j] < min)
                        {
                            min = down[j];
                        }
                    }
                }
                //Seleccionamos uno de los minimales
                var posibilidades = new List<int>();
                for (int j = 0; j < n; j++)
                {
                    if (down[j] == min && !usado[j]) //Minimal
                    {
                        posibilidades.Add(j);
                    }
                }
                //Elemento escogido
                int chosen = posibilidades[rg.Next(posibilidades.Count)];
                //Eliminamos el elemento escogido
                usado[chosen] = true;
                for (int j = 0; j < n; j++)
                {
                    down[j] -= a[chosen + j * n];
                    //up[j] -= a[chosen + j * n];
                }
                orden.Add(chosen);
            }
            return orden;
        }

        //#########################################################ORDENACIÓN##################################################

        /// <summary>
        /// Implementation of the QuickSort algorithm
        /// </summary>
        /// <param name="a">Aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public List<int> quickSort(List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var objects = new List<int>();
            for (int i = 0; i < n; i++)
            {
                objects.Add(i);
            }
            quickSortRecursive(ref objects, 0, objects.Count - 1, a, n, comparator);
            return objects;
        }

        private void quickSortRecursive(ref List<int> objects, int left, int right, List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var i = left;
            var j = right - 1;
            var pivot = 0;

            if (left < right)
            {
                var center = median_of_three(a, ref objects, left, right, n, comparator);
                if ((right - left) >= 3)
                {
                    pivot = objects[center];
                    objects.Swap(center, right);
                    do
                    {
                        while (comparator(a, objects[i], pivot, n) && i < right)
                        {
                            i++;
                        }
                        while (comparator(a, pivot, objects[j], n) && j > left)
                        {
                            j--;
                        }
                        if (i < j)
                        {
                            objects.Swap(i, j);
                        }
                    } while (i < j);

                    objects.Swap(i, right);
                    quickSortRecursive(ref objects, left, i - 1, a, n, comparator);
                    quickSortRecursive(ref objects, i + 1, right, a, n, comparator);
                }
            }
        }

        private int median_of_three(List<int> a, ref List<int> objects, int left, int right, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var center = (left + right) / 2;
            if (comparator(a, objects[center], objects[left], n))
                objects.Swap(left, center);
            if (comparator(a, objects[right], objects[left], n))
                objects.Swap(left, right);
            if (comparator(a, objects[right], objects[center], n))
                objects.Swap(center, right);
            return center;
        }

        /// <summary>
        /// Way of comparing A
        /// </summary>
        /// <param name="a"></param>
        /// <param name="objectsToBeOrdered"></param>
        /// <param name="objectToBeCompared"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool compareObjects(List<int> a, int objectsToBeOrdered, int objectToBeCompared, int n)
        {
            //número de posets
            var pp = a[0];

            //# veces que a <=b
            var smaller = a[objectsToBeOrdered + objectToBeCompared * n];
            //# veces que a > b 
            var bigger = a[objectToBeCompared + objectsToBeOrdered * n];
            var total = smaller + bigger;

            if (total == 0)
                return rg.Next(2) == 1;
            else
            {
                var random = rg.Next(total);
                return random < smaller;
            }
        }

        /// <summary>
        /// Way of comparing B
        /// </summary>
        /// <param name="a"></param>
        /// <param name="objectsToBeOrdered"></param>
        /// <param name="objectToBeCompared"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool compareObjectsB(List<int> a, int objectsToBeOrdered, int objectToBeCompared, int n)
        {
            //número de posets
            var pp = a[0];

            //# veces que a <=b
            double smaller = a[objectsToBeOrdered + objectToBeCompared * n];
            //# veces que a > b 
            double bigger = a[objectToBeCompared + objectsToBeOrdered * n];

            double notCompared = pp - (smaller + bigger);
            double compared = smaller + bigger;

            var probability = (smaller + 0.5 * notCompared) / pp;
            return rg.NextDouble() < probability;

        }

        /// <summary>
        /// Implementation of the selection algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public List<int> selection(List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var objects = new List<int>();
            for (int i = 0; i < n; i++)
            {
                objects.Add(i);
            }
            for (int i = 0; i < objects.Count; i++)
            {
                objects.Swap(i, chooseSmaller(objects, a, i, n, comparator));
            }
            return objects;
        }


        private int chooseSmaller(List<int> objects, List<int> a, int fistIndex, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var pos = n - 1;
            for (int i = fistIndex; i < objects.Count; i++)
            {
                if (comparator(a, i, pos, n))
                    pos = i;
            }
            return pos;
        }

        /// <summary>
        /// Implementation of the insertion algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public List<int> insertion(List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var objects = new List<int>();
            for (int i = 0; i < n; i++)
            {
                objects.Add(i);
            }
            var objectToBeOrdered = 0;
            var posToBeCompared = 0;
            for (int i = 1; i < objects.Count; i++)
            {
                objectToBeOrdered = objects[i];
                posToBeCompared = i - 1;
                while (posToBeCompared >= 0 && comparator(a, objectToBeOrdered, objects[posToBeCompared], n))
                {
                    objects[posToBeCompared + 1] = objects[posToBeCompared];
                    posToBeCompared--;
                }
                objects[posToBeCompared + 1] = objectToBeOrdered;
            }
            return objects;
        }

        /// <summary>
        /// Implementation of the bubble algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public List<int> bubble(List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var objects = new List<int>();
            for (int i = 0; i < n; i++)
            {
                objects.Add(i);
            }
            for (int i = 1; i < n; i++)
            {
                for (int j = n - 1; j >= i; j--)
                {
                    if (comparator(a, objects[j], objects[j - 1], n))
                    {
                        objects.Swap(j, j - 1);
                    }
                }
            }
            return objects;
        }

        /// <summary>
        /// Implementation of the MergeSort algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public List<int> mergeSort(List<int> a, int n, Func<List<int>, int, int, int, bool> comparator)
        {
            var unsorted = new List<int>();
            for (int i = 0; i < n; i++)
            {
                unsorted.Add(i);
            }
            return mergeSortRecursive(a, n, ref unsorted, comparator);
        }

        private List<int> mergeSortRecursive(List<int> a, int n, ref List<int> unsorted, Func<List<int>, int, int, int, bool> comparator)
        {
            if (unsorted.Count <= 1)
                return unsorted;

            var left = new List<int>();
            var right = new List<int>();

            int middle = unsorted.Count / 2;
            for (int i = 0; i < middle; i++)  //Dividing the unsorted list
            {
                left.Add(unsorted[i]);
            }
            for (int i = middle; i < unsorted.Count; i++)
            {
                right.Add(unsorted[i]);
            }

            left = mergeSortRecursive(a, n, ref left, comparator);
            right = mergeSortRecursive(a, n, ref right, comparator);
            return merge(a, n, left, right, comparator);
        }

        private List<int> merge(List<int> a, int n, List<int> left, List<int> right, Func<List<int>, int, int, int, bool> comparator)
        {
            var result = new List<int>();

            while (left.Count > 0 || right.Count > 0)
            {
                if (left.Count > 0 && right.Count > 0)
                {
                    if (comparator(a, left.First(), right.First(), n))  //Comparing First two elements to see which is smaller
                    {
                        result.Add(left.First());
                        left.Remove(left.First());      //Rest of the list minus the first element
                    }
                    else
                    {
                        result.Add(right.First());
                        right.Remove(right.First());
                    }
                }
                else if (left.Count > 0)
                {
                    result.Add(left.First());
                    left.Remove(left.First());
                }
                else if (right.Count > 0)
                {
                    result.Add(right.First());

                    right.Remove(right.First());
                }
            }
            return result;
        }


        //####################################################SIMULATED ANNEALING##########################################

        /// <summary>
        /// Implementation of the simulated annealing algorithm
        /// </summary>
        /// <param name="original">Initial solution</param>
        /// <param name="A">Aggregation matrix</param>
        /// <param name="n">Size</param>
        /// <param name="temp">Temperature</param>
        /// <param name="beta">Cooling constant</param>
        /// <returns></returns>
        public (List<int>, int) simulatedAnnealing(ref List<int> original, List<int> A, int n, double temp, double beta)
        //http://www.theprojectspot.com/tutorial-post/simulated-annealing-algorithm-for-beginners/6
        {
            var current = original.Select(x => x).ToList();
            var currentCost = coste(ref current, A, n);
            Func<int, int, double, double> umbral = (energy, newEnergy, temperature) =>
               {
                   if (newEnergy < energy) return 1;
                   return Math.Exp((energy - newEnergy) / temperature);
               };

            var best = original.Select(x => x).ToList();
            var bestCost = coste(ref current, A, n);

            while (temp > 1)
            {
                //Compute new solution
                var positionA = rg.Next(n);
                var positionB = positionA + 1;
                var valueA = current[positionA];
                var valueB = current[positionB % n];
                var newSolution = current.Select(x => x).ToList();
                newSolution.Swap(positionA, positionB % n);
                //Compute cost of solution
                var newCost = coste(ref newSolution, A, n);
                //Decide acceptance
                if (umbral(currentCost, newCost, temp) > rg.NextDouble())
                {
                    current = newSolution.Select(x => x).ToList();
                    currentCost = newCost;
                }
                //Keep track of best solution
                if (currentCost < bestCost)
                {
                    best = current.Select(x => x).ToList();
                    bestCost = currentCost;
                    //Console.WriteLine("UPDATED = " + bestCost);
                }
                //Cool system
                temp *= beta;
            }
            return (best, bestCost);
        }

        private (List<int>, int) changeSomething(List<int> estimated, int costEstimated, List<int> a, int n)
        {
            var positionA = rg.Next(n);
            var positionB = (positionA + 1) % n;

            var valueA = estimated[positionA];
            var valueB = estimated[positionB];

            estimated.Swap(positionA, positionB);

            //var newCost = a[valueA+valueB*n] - a[valueB+valueA*n];

            // return (estimated, costEstimated + newCost);
            return (estimated, coste(ref estimated, a, n));
        }
        //#############################################LINEAR PROGRAMMING################################

        //public (List<int>, int) linearProgramming(ref List<int> a, int n)
        ////https://www.appliedis.com/constraint-satisfaction-programming-with-microsoft-solver-foundation/
        //{
        //    //CargarLibreriasExternas();
        //    //Initializing the solver
        //    var solver = SolverContext.GetContext();
        //    var model = solver.CreateModel();

        //    //Defining the cells
        //    var cells = new List<Cell>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            var cell = new Cell(
        //                String.Format("Cell{0}", i + j * n),
        //                a[i + j * n]);
        //            cells.Insert(j+i*n,cell);
        //        }
        //    }
        //    //Defining decisions
        //    var decisions = cells.Select(cell => new Decision(Domain.IntegerRange(0, 1), cell.Name)).ToList(); ;
        //    model.AddDecisions(decisions.ToArray());

        //    //CONSTRAINTS
        //    //Transitivity
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j)
        //            {
        //                if(i<j)
        //                {
        //                    for (var k = 0; k < n; k++)
        //                    {
        //                        if (i != k && j != k)
        //                        {
        //                            if(j < k && i < k) //i<j j<k i<k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, decisions[i + j * n] + decisions[j + k * n] - decisions[i + k * n] <= 1);
        //                            }
        //                            else if (j > k && i < k) // i<j j>k i<k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, decisions[i + j * n] + (1-decisions[k + j * n]) - decisions[i + k * n] <= 1);
        //                            }
        //                            else if (j > k && i > k) //i<j j>k i>k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, decisions[i + j * n] + (1-decisions[k + j * n]) - (1-decisions[k + i * n]) <= 1);
        //                            }
        //                            else if (j < k && i > k) //i<j j<k i>k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, decisions[i + j * n] + decisions[j + k * n] - (1-decisions[k + i * n]) <= 1);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (var k = 0; k < n; k++)
        //                    {
        //                        if (i != k && j != k)
        //                        {
        //                            if (j < k && i < k) //i>j j<k i<k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, (1-decisions[j + i * n]) + decisions[j + k * n] - decisions[i + k * n] <= 1);
        //                            }
        //                            else if (j > k && i < k) // i>j j>k i<k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, (1-decisions[j + i * n]) + (1 - decisions[k + j * n]) - decisions[i + k * n] <= 1);
        //                            }
        //                            //else if (j > k && i > k) //i>j j>k i>k
        //                            //{
        //                            //    var name = String.Format("Transitivity{0}", Guid.NewGuid());      NOT NEEDED
        //                            //    name = name.Replace("-", "_");
        //                            //    model.AddConstraint(name, (1-decisions[j + i * n]) + (1 - decisions[k + j * n]) - (1 - decisions[k + i * n]) <= 1);
        //                            //}
        //                            else if (j < k && i > k) //i>j j<k i>k
        //                            {
        //                                var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                                name = name.Replace("-", "_");
        //                                model.AddConstraint(name, (1-decisions[j + i * n]) + decisions[j + k * n] - (1 - decisions[k + i * n]) <= 1);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //Define goal to minimize cost
        //    var objective = new SumTermBuilder(decisions.Count());
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j && i < j)
        //            {
        //                var cellDecision = model.Decisions.First(dec => dec.Name == cells[i + j * n].Name);
        //                objective.Add(cellDecision * cells[i + j * n].Cost + (1-cellDecision) * a[i+j*n]);
        //            }
        //        }
        //    }
        //    model.AddGoal("Cost", GoalKind.Minimize, objective.ToTerm());
        //    var solution = solver.Solve();
        //    solver.ClearModel();
        //    return solveFinalCostB(decisions, cells, n, a);
        //}

        //private static void CargarLibreriasExternas()
        //{
        //    AppDomain currentDomain = AppDomain.CurrentDomain;
        //    currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
        //}

        //private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        //{
        //    //This handler is called only when the common language runtime tries to bind to the assembly and fails. 

        //    //Retrieve the list of referenced assemblies in an array of AssemblyName. 
        //    Assembly MyAssembly, objExecutingAssemblies;
        //    string strTempAssmbPath = "";

        //    objExecutingAssemblies = Assembly.GetExecutingAssembly();
        //    AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

        //    //Loop through the array of referenced assembly names. 
        //    foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
        //    {
        //        //Check for the assembly names that have raised the "AssemblyResolve" event. 
        //        if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
        //        {
        //            //Build the path of the assembly from where it has to be loaded. 
        //            strTempAssmbPath = @"D:\Documentos\Universidad\Prácticas\UNIMODE\CopiaProyectoElias\CopiaProyectoElias\bin\x86\Release\NativeBinaries\win32\lpsolve55.dll";
        //            break;
        //        }

        //    }
        //    //Load the assembly from the specified path.  
        //    MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

        //    //Return the loaded assembly. 
        //    return MyAssembly;
        //}

        //public (List<int>, int) linearProgrammingA(ref List<int> a, int n)
        ////https://www.appliedis.com/constraint-satisfaction-programming-with-microsoft-solver-foundation/
        //{
        //    //Initializing the solver
        //    var solver = SolverContext.GetContext();
        //    var model = solver.CreateModel();
        //    var constraints = 0;
        //    var function = 0;

        //    //Defining the cells
        //    var cells = new List<Cell>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            var cell = new Cell(
        //                String.Format("Cell{0}{1}", j, i),
        //                a[i + j * n]);
        //            cells.Add(cell);
        //        }
        //    }
        //    //Defining decisions
        //    var decisions = cells.Select(cell => new Decision(Domain.IntegerRange(0, 1), cell.Name)).ToList(); ;
        //    model.AddDecisions(decisions.ToArray());

        //    //CONSTRAINTS
        //    //Elements in the diagonal Xii=1
        //    for (var i = 0; i < n; i++)
        //    {
        //        //Console.WriteLine(String.Format("Constraint diagonal i = {0} ", i));
        //        model.AddConstraint(String.Format("Diagonal{0}", i), decisions[i * n + i] == 1);
        //        constraints++;
        //    }
        //    //No cycles
        //    for (var i = 0; i < n-1; i++)
        //    {
        //        for (var j = i+1; j < n; j++)
        //        {
        //            if (i != j)
        //            {
        //                //Console.WriteLine(String.Format("Constraint no cycle i = {0} j = {1}", i, j));
        //                var name = String.Format("NotCycle{0}", i + j * n);
        //                model.AddConstraint(name, decisions[i + j * n] + decisions[j + i * n] == 1);
        //                constraints++;
        //            }
        //        }
        //    }
        //    //Transitivity
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j)
        //            {
        //                for (var k = 0; k < n; k++)
        //                {
        //                    if (i != k && j != k)
        //                    {
        //                        //Console.WriteLine(String.Format("Constraint transitividad i = {0} j = {1} k = {2}", i, j, k));
        //                        var name = String.Format("Transitivity{0}", Guid.NewGuid());
        //                        name = name.Replace("-", "_");
        //                        model.AddConstraint(name, decisions[i + j * n] + decisions[j + k * n] - decisions[i + k * n] <= 1);
        //                        constraints++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //Define goal to minimize cost
        //    var objective = new SumTermBuilder(decisions.Count());
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j)
        //            {
        //                function++;
        //                var cellDecision = model.Decisions.First(dec => dec.Name == cells[i + j * n].Name);
        //                objective.Add(cellDecision * cells[i + j * n].Cost);
        //            }
        //        }
        //    }
        //    model.AddGoal("Cost", GoalKind.Minimize, objective.ToTerm());

        //    var solution = solver.Solve();
        //    solver.ClearModel();
        //    Console.WriteLine("Constraints = " + constraints);
        //    Console.WriteLine("Function = " + function);

        //    return solveFinalCost(decisions, cells, n);
        //}

        //private (List<int>, int) solveFinalCost(List<Decision> decisions, List<Cell> cells, int n)
        //{
        //    //order
        //    var dict = new Dictionary<int, int>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        var elements = 0;
        //        for (var j = 0; j < n; j++)
        //        {
        //            elements += (int)decisions[j + i * n].ToDouble();
        //        }
        //        dict.Add(i, elements);
        //    }
        //    var order = new List<int>();
        //    dict.OrderBy(par => par.Value).ToList().ForEach(par => order.Add(par.Key));

        //    //cost
        //    var cost = 0;
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j)
        //            {
        //                var cellDecision = decisions.First(dec => dec.Name == cells[i + j * n].Name);
        //                cost += ((int)cellDecision.ToDouble() * cells[i + j * n].Cost);
        //            }
        //        }
        //    }
        //    return (order, cost);
        //}

        //private (List<int>, int) solveFinalCostB(List<Decision> decisions, List<Cell> cells, int n, List<int> a)
        //{
        //    //order
        //    //var dict = new Dictionary<int, int>();
        //    //for (var i = 0; i < n; i++)
        //    //{
        //    //    var elements = 0;
        //    //    for (var j = 0; j < n; j++)
        //    //    {
        //    //        elements += (int)decisions[i + j * n].ToDouble();
        //    //    }
        //    //    dict.Add(i, elements);
        //    //}
        //    var order = new List<int>();
        //    //dict.OrderBy(par => par.Value).ToList().ForEach(par => order.Add(par.Key));

        //    //cost
        //    var cost = 0;
        //    for (var i = 0; i < n; i++)
        //    {
        //        for (var j = 0; j < n; j++)
        //        {
        //            if (i != j && i < j)
        //            {
        //                var cellDecision = decisions.First(dec => dec.Name == cells[i + j * n].Name);
        //                cost += ((int)cellDecision.ToDouble() * cells[i + j * n].Cost + (1 - (int)cellDecision.ToDouble()) * a[i + j * n]);
        //            }
        //        }
        //    }
        //    return (order, cost);
        //}

        private class Cell
        {
            public Cell(string name, int cost)
            {
                this.Name = name;
                this.Cost = cost;
            }

            public string Name { get; }
            public int Cost { get; }
        }

        /// <summary>
        /// Final implementation of the linear programming algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public (List<int>, int) linearProgrammingB(ref List<int> a, int n)
        {
            LpSolve.Init();
            //Defining the cells
            var cells = new List<Cell>();
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var cell = new Cell(
                        String.Format("Cell{0}", i + j * n),
                        a[i + j * n]);
                    cells.Insert(j + i * n, cell);
                }
            }
            int constraints = 0;
            int nCol;
            int[] colNo;
            int l, ret = 0;
            double[] row;
            var variables = new List<int>();
            var cost = 0;
            var function = 0;

            //Model built row by row

            //So we start with 0 rows and n*n columns
            nCol = n*n;
            var lp = LpSolve.make_lp(0, nCol);
            if(lp == null)
            {
                ret = 1; //Couldn´t construct a new model
            }
            if(lp != null)
            {
                for(var i=0; i<n; i++)
                {
                    for(var j=0; j<n; j++)
                    {
                        lp.set_col_name(i + j * n + 1, cells[i + j * n].Name);
                    }
                }
            }

            //Create space large enough for one row
            colNo = new int[nCol];
            row = new double[nCol];
            //Elements 0 or 1
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    lp.set_binary(i + j * n + 1, true);
                }
            }
            if (ret == 0)
            {
                lp.set_add_rowmode(true); //makes bulding model faster
                //Console.WriteLine("Contraints transitivity se añaden");
                //Transitivity
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            for (var k = 0; k < n; k++)
                            {
                                if (i != k && j != k)
                                {
                                    l = 0;
                                    colNo = new int[nCol];
                                    row = new double[nCol];
                                    colNo[l] = i + j * n + 1;
                                    row[l++] = 1;
                                    colNo[l] = j + k * n + 1;
                                    row[l++] = 1;
                                    colNo[l] = i + k * n + 1;
                                    row[l++] = -1;
                                    //Console.WriteLine(String.Format("Constraint transitividad i = {0} j = {1} k = {2}", i, j, k));
                                    //add the row to the model
                                    if (lp.add_constraintex(l, row, colNo, lpsolve_constr_types.LE, 1) == false)
                                    {
                                        ret = 3;
                                    }
                                    else
                                        constraints++;
                                }
                            }                                                                         
                        }
                    }
                }
            }
            if(ret == 0)
            {
                //Console.WriteLine("Contraints no cycle se añaden");
                //No cycles 
                for (var i = 0; i < n - 1; i++)
                {
                    for (var j = i + 1; j < n; j++)
                    {
                        l = 0;
                        colNo = new int[nCol];
                        row = new double[nCol];

                        colNo[0] = i + j * n + 1;
                        row[0] = 1;
                        colNo[1] = j + i * n + 1;
                        row[1] = 1;
                        //Console.WriteLine(String.Format("Constraint no cycle i = {0} j = {1}", i, j));
                        //add row
                        if (lp.add_constraintex(2, row, colNo, lpsolve_constr_types.EQ, 1) == false)
                        {
                            ret = 3;
                        }
                        else constraints++;
                    }
                }
            }
            if(ret == 0)
            {
                //Console.WriteLine("Contraints diagonal se añaden");
                //Elements in the diagonal Xii=1 
                for (var i = 0; i < n; i++)
                {
                    l = 0;
                    colNo = new int[nCol];
                    row = new double[nCol];

                    colNo[l] = i + i * n + 1;
                    row[l++] = 1;
                    //Console.WriteLine(String.Format("Constraint diagonal i = {0} ", i));
                    //add row
                    if (lp.add_constraintex(l, row, colNo, lpsolve_constr_types.EQ, 1) == false)
                    {
                        ret = 3;
                    }
                    else constraints++;
                }
            }
            
            if (ret == 0)
            {
                lp.set_add_rowmode(false);
                l = 0;
                colNo = new int[nCol];
                row = new double[nCol];

                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            function++;
                            colNo[l] = i + j * n + 1;
                            row[l++] = cells[i + j * n].Cost;
                        }
                    }
                }
                if (lp.set_obj_fnex(l, row, colNo) == false)
                {
                    ret = 4;
                }
            }
            if (ret == 0)
            {
                //set minimize
                lp.set_minim();

                //show model
                //lp.print_lp();

                //important messages
                lp.set_verbose(3);

                //compute solution
                var s = lp.solve();
                //System.Diagnostics.Debug.WriteLine(s);
                if (s == lpsolve_return.OPTIMAL)
                    ret = 0;
                else
                    ret = 5;
            }
            if(ret == 0)
            {
                //get results
                //objective value
                //Console.WriteLine((int) lp.get_objective());

                //variable values
                lp.get_variables(row);
                variables = new List<int>();
                for(var j = 0; j < nCol; j++)
                {
                    variables.Add((int)row[j]);
                }
                cost = 0;
                for(var i = 0;i<n;i++)
                {
                    for(var j=0;j<n;j++)
                    {
                        if(i!=j) cost += variables[i+j*n] * cells[i+j*n].Cost;
                    }
                }
            }
            /* free allocated memory */

            if (lp != null)
            {
                /* clean up such that all used memory by lpsolve is freed */
                lp.delete_lp();
            }
            //Console.WriteLine("Constraints=" + constraints);
            //Console.WriteLine("Function=" + function);
            return (variables,cost);
        }
    }
    //##################################################EXTENSION METHODS##########################################

    public static class Extensions
    {
        /// <summary>
        /// Method to randomize a poset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rg"></param>
        public static void randomShuffle<T>(this IList<T> list, Random rg)
        {
            if (list.Count() > 1)
            {
                for (int i = list.Count() - 1; i >= 0; i--)
                {
                    var tmp = list[i];
                    var randomIndex = rg.Next(i + 1);

                    //Swap elements
                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

        /// <summary>
        /// Method to swap two positions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(
            this IList<T> list,
            int firstIndex,
            int secondIndex
        )
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
            {
                return;
            }
            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }
    }
}
    

