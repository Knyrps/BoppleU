using System;

namespace Scripts.Pegs
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WeightAttribute : Attribute
    {
        public int Weight { get; }

        public WeightAttribute(int weight)
        {
            this.Weight = weight;
        }
    }
}