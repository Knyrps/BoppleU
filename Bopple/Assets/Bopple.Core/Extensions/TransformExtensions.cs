using UnityEngine;

namespace Bopple.Core.Extensions
{
    public static class TransformExtensions
    {
        public static T GetAncestorComponent<T>(this Transform startTransform) where T : Component
        {
            Transform currentTransform = startTransform.parent;

            while (currentTransform != null)
            {
                T component = currentTransform.GetComponent<T>();
                if (component != null)
                    return component;

                currentTransform = currentTransform.parent;
            }

            return null;
        }
    }
}