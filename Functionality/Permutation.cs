using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using extra;

namespace extra
{
   
    //Eric Ouellet, E. O. (2018, 3 julio). Permutations: Fast implementations and a new indexing algorithm allowing multithreading. 
    //Recuperado 30 septiembre, 2019, de https://www.codeproject.com/Articles/1250925/Permutations-Fast-implementations-and-a-new-indexi
    
    //It contains all the methods needed to compute the minimum cost in a parallel way


    public class Factorial
    {
        // ************************************************************************
        protected static long[] FactorialTable = new long[21];

        // ************************************************************************
        static Factorial()
        {
            FactorialTable[0] = 1; // To prevent divide by 0
            long f = 1;
            for (int i = 1; i <= 20; i++)
            {
                f = f * i;
                FactorialTable[i] = f;
            }
        }

        // ************************************************************************
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetFactorial(int val) // a long can only support up to 20!
        {
            if (val > 20)
            {
                throw new OverflowException($"{nameof(Factorial)} only support a factorial value <= 20");
            }

            return FactorialTable[val];
        }

        // ************************************************************************
    }

    public class PermutationSaniSinghHuttunen
    {
        public static bool NextPermutation(int[] numList)
        {
            /*
             Knuths
             1. Find the largest index j such that a[j] < a[j + 1]. If no such index exists, the permutation is the last permutation.
             2. Find the largest index l such that a[j] < a[l]. Since j + 1 is such an index, l is well defined and satisfies j < l.
             3. Swap a[j] with a[l].
             4. Reverse the sequence from a[j + 1] up to and including the final element a[n].

             */
            var largestIndex = -1;
            for (var i = numList.Length - 2; i >= 0; i--)
            {
                if (numList[i] < numList[i + 1])
                {
                    largestIndex = i;
                    break;
                }
            }

            if (largestIndex < 0) return false;

            var largestIndex2 = -1;
            for (var i = numList.Length - 1; i >= 0; i--)
            {
                if (numList[largestIndex] < numList[i])
                {
                    largestIndex2 = i;
                    break;
                }
            }

            var tmp = numList[largestIndex];
            numList[largestIndex] = numList[largestIndex2];
            numList[largestIndex2] = tmp;

            for (int i = largestIndex + 1, j = numList.Length - 1; i < j; i++, j--)
            {
                tmp = numList[i];
                numList[i] = numList[j];
                numList[j] = tmp;
            }

            return true;
        }
    }

    public class PermutationOuelletLexico3<T> // Enable indexing 
    {
        // ************************************************************************
        private T[] _sortedValues;

        private bool[] _valueUsed;

        public readonly long MaxIndex; // long to support 20! or less 

        // ************************************************************************
        public PermutationOuelletLexico3(T[] sortedValues)
        {
            _sortedValues = sortedValues;
            Result = new T[_sortedValues.Length];
            _valueUsed = new bool[_sortedValues.Length];

            MaxIndex = Factorial.GetFactorial(_sortedValues.Length);
        }

        // ************************************************************************
        public T[] Result { get; private set; }

        // ************************************************************************
        /// <summary>
        /// Sort Index is 0 based and should be less than MaxIndex. Otherwise you get an exception.
        /// </summary>
        /// <param name="sortIndex"></param>
        /// <param name="result">Value is not used as inpu, only as output. Re-use buffer in order to save memory</param>
        /// <returns></returns>
        public void GetSortedValuesFor(long sortIndex)
        {
            int size = _sortedValues.Length;

            if (sortIndex < 0)
            {
                throw new ArgumentException("sortIndex should greater or equal to 0.");
            }

            if (sortIndex >= MaxIndex)
            {
                throw new ArgumentException("sortIndex should less than factorial(the lenght of items)");
            }

            for (int n = 0; n < _valueUsed.Length; n++)
            {
                _valueUsed[n] = false;
            }

            long factorielLower = MaxIndex;

            for (int index = 0; index < size; index++)
            {
                long factorielBigger = factorielLower;
                factorielLower = Factorial.GetFactorial(size - index - 1);  //  factorielBigger / inverseIndex;

                int resultItemIndex = (int)(sortIndex % factorielBigger / factorielLower);

                int correctedResultItemIndex = 0;
                for (; ; )
                {
                    if (!_valueUsed[correctedResultItemIndex])
                    {
                        resultItemIndex--;
                        if (resultItemIndex < 0)
                        {
                            break;
                        }
                    }
                    correctedResultItemIndex++;
                }

                Result[index] = _sortedValues[correctedResultItemIndex];
                _valueUsed[correctedResultItemIndex] = true;
            }
        }
        // ************************************************************************
    }
    public class PermutationMixOuelletSaniSinghHuttunen
    {
        // ************************************************************************
        private long _indexFirst;
        private long _indexLastExclusive;
        private int[] _sortedValues;

        // ************************************************************************
        public PermutationMixOuelletSaniSinghHuttunen(int[] sortedValues, long indexFirst = -1, long indexLastExclusive = -1)
        {
            if (indexFirst == -1)
            {
                indexFirst = 0;
            }

            if (indexLastExclusive == -1)
            {
                indexLastExclusive = Factorial.GetFactorial(sortedValues.Length);
            }

            if (indexFirst >= indexLastExclusive)
            {
                throw new ArgumentException($"{nameof(indexFirst)} should be less than {nameof(indexLastExclusive)}");
            }

            _indexFirst = indexFirst;
            _indexLastExclusive = indexLastExclusive;
            _sortedValues = sortedValues;
        }

        // ************************************************************************
        public void ExecuteForEachPermutation(Action<int[]> action)
        {
            //          Console.WriteLine($"Thread {System.Threading.Thread.CurrentThread.ManagedThreadId} started: {_indexFirst} {_indexLastExclusive}");

            long index = _indexFirst;

            PermutationOuelletLexico3<int> permutationOuellet = new PermutationOuelletLexico3<int>(_sortedValues);

            permutationOuellet.GetSortedValuesFor(index);
            action(permutationOuellet.Result);
            index++;

            int[] values = permutationOuellet.Result;
            while (index < _indexLastExclusive)
            {
                PermutationSaniSinghHuttunen.NextPermutation(values);
                action(values);
                index++;
            }

            //          Console.WriteLine($"Thread {System.Threading.Thread.CurrentThread.ManagedThreadId} ended: {DateTime.Now.ToString("yyyyMMdd_HHmmss_ffffff")}");
        }

        // ************************************************************************
        public static void ExecuteForEachPermutationMT(int[] sortedValues, Action<int[]> action)
        {
            int coreCount = Environment.ProcessorCount; // Hyper treading are taken into account (ex: on a 4 cores hyperthreaded = 8)
            long itemsFactorial = Factorial.GetFactorial(sortedValues.Length);
            long partCount = (long)Math.Ceiling((double)itemsFactorial / (double)coreCount);
            long startIndex = 0;

            var tasks = new List<Task>();

            for (int coreIndex = 0; coreIndex < coreCount; coreIndex++)
            {
                long stopIndex = Math.Min(startIndex + partCount, itemsFactorial);

                PermutationMixOuelletSaniSinghHuttunen mix = new PermutationMixOuelletSaniSinghHuttunen(sortedValues, startIndex, stopIndex);
                Task task = Task.Run(() => mix.ExecuteForEachPermutation(action));
                tasks.Add(task);

                if (stopIndex == itemsFactorial)
                {
                    break;
                }

                startIndex = startIndex + partCount;
            }

            Task.WaitAll(tasks.ToArray());
        }

        // ************************************************************************

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static bool ForAllPermutation<T>(T[] items, Func<T[], bool> funcExecuteAndTellIfShouldStop)
        {
            int countOfItem = items.Length;

            if (countOfItem <= 1)
            {
                return funcExecuteAndTellIfShouldStop(items);
            }

            var indexes = new int[countOfItem];
            for (int i = 0; i < countOfItem; i++)
            {
                indexes[i] = 0;
            }

            if (funcExecuteAndTellIfShouldStop(items))
            {
                return true;
            }

            for (int i = 1; i < countOfItem;)
            {
                if (indexes[i] < i)
                { // On the web there is an implementation with a multiplication which should be less efficient.
                    if ((i & 1) == 1) // if (i % 2 == 1)  ... more efficient ??? At least the same.
                    {
                        Swap(ref items[i], ref items[indexes[i]]);
                    }
                    else
                    {
                        Swap(ref items[i], ref items[0]);
                    }

                    if (funcExecuteAndTellIfShouldStop(items))
                    {
                        return true;
                    }

                    indexes[i]++;
                    i = 1;
                }
                else
                {
                    indexes[i++] = 0;
                }
            }

            return false;
        }

    }
    
}
