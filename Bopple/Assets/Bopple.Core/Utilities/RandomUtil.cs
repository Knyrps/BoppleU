using System;
using System.Collections.Generic;
using System.Linq;

namespace Bopple.Core.Utilities
{
    public static class RandomUtil
    {
        private static readonly System.Random Random = new System.Random();
        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static float GetRandomFloat(float min, float max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }

        public static T GetRandomElement<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("List cannot be null or empty.");
            }
            return list[GetRandomInt(0, list.Count)];
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetRandomElementWeighted<T>(IList<T> list, IList<float> weights)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("List cannot be null or empty.");
            }

            if (weights == null || weights.Count != list.Count)
            {
                throw new ArgumentException("Weights must be the same length as the list.");
            }

            float totalWeight = weights.Sum();
            float randomValue = GetRandomFloat(0, totalWeight);

            for (int i = 0; i < list.Count; i++)
            {
                if (randomValue < weights[i])
                {
                    return list[i];
                }
                randomValue -= weights[i];
            }
            return list[^1];
        }
    }
}