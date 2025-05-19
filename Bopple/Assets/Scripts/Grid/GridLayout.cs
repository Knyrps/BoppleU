using System;
using System.Collections.Generic;
using System.Linq;
using Bopple.Core.Utilities;

namespace Scripts.Grid
{
    /// <summary>
    ///     Enum containing the different grid layouts.
    ///     When adding layouts, make sure to add them to the <see cref="GridLayoutResolver" /> class in the
    ///     <see cref="GridLayoutResolver.Layouts" /> method and the <see cref="GridLayoutResolver.Chances" /> dictionary.
    ///     Otherwise, they will never be used.
    /// </summary>
    public enum GridLayout
    {
        /// <summary>
        ///     Randomize the grid layout.
        /// </summary>
        Random = 0,

        /// <summary>
        ///     ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬜ ⬛ ⬛ <br />
        ///     ⬛ ⬜ ⬜ ⬜ ⬛ <br />
        ///     ⬛ ⬛ ⬜ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬛ ⬛
        /// </summary>
        Grid5By5WithHole,

        /// <summary>
        ///     ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬜ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬜ ⬜ ⬜ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬜ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        ///     ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛
        /// </summary>
        Grid7By7WithHole
    }

    public static class GridLayoutResolver
    {
        private static readonly Dictionary<GridLayout, int> Chances = new()
        {
            { GridLayout.Grid5By5WithHole, 10 },
            { GridLayout.Grid7By7WithHole, 10 }
        };

        private static readonly Dictionary<GridLayout, bool[][]> Layouts = new()
        {
            { GridLayout.Grid5By5WithHole, new[] {
                new[] { true, true, true, true, true },
                new[] { true, true, false, true, true },
                new[] { true, false, false, false, true },
                new[] { true, true, false, true, true },
                new[] { true, true, true, true, true }
            }},
            { GridLayout.Grid7By7WithHole, new[] {
                new[] { true, true, true, true, true, true, true },
                new[] { true, true, true, true, true, true, true },
                new[] { true, true, true, false, true, true, true },
                new[] { true, true, false, false, false, true, true },
                new[] { true, true, true, false, true, true, true },
                new[] { true, true, true, true, true, true, true },
                new[] { true, true, true, true, true, true, true }
            }}
        };

        public static bool[][] Resolve(GridLayout layout)
        {
            UnityEngine.Debug.Assert(
                DictionariesMatch(),
                "Layouts and Chances keys must match!"
            );

            if (layout == GridLayout.Random)
            {
                int layoutAmount = Enum.GetNames(typeof(GridLayout)).Length;
                if (layoutAmount <= 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, "No GridLayouts defined");
                }

                layout = RandomUtil.GetRandomElementWeighted<KeyValuePair<GridLayout, int>>(
                    Chances.Where(kvp => kvp.Key != GridLayout.Random).ToList(),
                    Chances.Where(kvp => kvp.Key != GridLayout.Random).Select(kvp => (float)kvp.Value).ToList()
                ).Key;
            }

            if (!Layouts.TryGetValue(layout, out bool[][] resolvedLayout))
            {
                throw new ArgumentOutOfRangeException(nameof(layout), layout, "GridLayout not defined");
            }

            return resolvedLayout;
        }

        private static bool DictionariesMatch()
        {
            return Chances.Keys.Count == Layouts.Keys.Count &&
                   Chances.Keys.All(key => Layouts.ContainsKey(key));
        }
    }
}