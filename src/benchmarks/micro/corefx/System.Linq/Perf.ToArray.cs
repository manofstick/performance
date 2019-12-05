// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using MicroBenchmarks;
using System.Collections.Generic;

namespace System.Linq.Tests
{
    [BenchmarkCategory(Categories.CoreFX, Categories.LINQ)]
    public class Perf_ToArray
    {
        public enum PredicateTypes
        {
            Interleaved,
            True,
            False,
            FirstHalf
        }

        public enum EnumerableTypes
        {
            Generator,
            Array,
            List
        }

        [Params(0, 1, 2, 3, 6, 12, 25, 51, 100, 500, 1000, 10000)]
        public int Len;

        [Params(PredicateTypes.Interleaved, PredicateTypes.True, PredicateTypes.False, PredicateTypes.FirstHalf)]
        public PredicateTypes Pred;

        [Params(EnumerableTypes.Generator, EnumerableTypes.Array, EnumerableTypes.List)]
        public EnumerableTypes Enum;

        public static IEnumerable<int> Generator(int count)
        {
            for (var i = 0; i < count; ++i)
                yield return i;
        }

        IEnumerable<int> Data;
        Func<int, bool> Predicate;

        [GlobalSetup]
        public void Setup()
        {
            Data = Enum switch
            {
                EnumerableTypes.Generator => Generator(Len),
                EnumerableTypes.Array => Generator(Len).ToArray(),
                EnumerableTypes.List => Generator(Len).ToList(),
                _ => throw new NotSupportedException()
            };

            Predicate = Pred switch
            {
                PredicateTypes.Interleaved => x => x % 2 == 0,
                PredicateTypes.True => x => true,
                PredicateTypes.False => x => false,
                PredicateTypes.FirstHalf => x => x < (Len / 2),
                _ => throw new NotSupportedException()
            };
        }

        [Benchmark(Baseline = true)]
        public int[] Linq() => Data.Where(Predicate).ToArray();
    }
}