using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bopple.Core.Utilities;
using Scripts.Pegs;
using UnityEditor;

namespace Scripts.Inspectors
{
    public class WeightInspector
    {
        [MenuItem("Tools/Print All Weights")]
        public static void PrintAllWeights()
        {
            Type baseType = typeof(PegBase); // your abstract class
            IEnumerable<Type> derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);

            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(PegBase).IsAssignableFrom(t) && !t.IsAbstract);

            Dictionary<Type, int> weights = new Dictionary<Type, int>();

            foreach (Type t in types)
            {
                WeightAttribute attr = t.GetCustomAttribute<WeightAttribute>();
                if (attr != null)
                {
                    weights[t] = attr.Weight;
                }
                else
                {
                    LogUtil.LogWarning($"No WeightAttribute found on {t.Name}");
                }
            }

            int totalWeight = weights.Values.Sum();

            foreach (KeyValuePair<Type, int> w in weights)
            {
                float percentage = (float)w.Value / totalWeight * 100;

                LogUtil.Log($"{w.Key.Name} - Weight: {w.Value}, Percentage: {percentage:F2}%");
            }
        }
    }
}