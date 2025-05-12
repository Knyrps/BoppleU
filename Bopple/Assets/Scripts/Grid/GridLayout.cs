using System;
using UnityEditor;

namespace Scripts.Grid
{
    public enum GridLayout
    {
        /// <summary>
        /// Randomize the grid layout.
        /// </summary>
        Random = 0,
        /// <summary>
        /// ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬜ ⬛ ⬛ <br />
        /// ⬛ ⬜ ⬜ ⬜ ⬛ <br />
        /// ⬛ ⬛ ⬜ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬛ ⬛
        /// </summary>
        Grid5By5WithHole,
        /// <summary>
        /// ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬜ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬜ ⬜ ⬜ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬜ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ <br />
        /// ⬛ ⬛ ⬛ ⬛ ⬛ ⬛ ⬛
        /// </summary>
        Grid7By7WithHole,
    }

    public static class GridLayoutResolver
    {
        public static bool[][] Resolve(GridLayout layout)
        {
            if (layout == GridLayout.Random)
            {
                int layoutAmount = Enum.GetNames(typeof(GridLayout)).Length;
                if (layoutAmount == 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, "No GridLayouts defined");
                }

                // Range: From 1 (to ignore Random) to the Length of Layouts (null based)
                int randInt = new Random().Next(1, layoutAmount - 1);

                layout = (GridLayout)randInt;
            }

            return layout switch
            {
                GridLayout.Grid5By5WithHole => new []
                {
                    new []{true, true, true, true, true},
                    new []{true, true, false, true, true},
                    new []{true, false, false, false, true},
                    new []{true, true, false, true, true},
                    new []{true, true, true, true, true},
                },
                GridLayout.Grid7By7WithHole => new[]
                {
                    new []{true, true, true, true, true, true, true},
                    new []{true, true, true, true, true, true, true},
                    new []{true, true, true, false, true, true, true},
                    new []{true, true, false, false, false, true, true},
                    new []{true, true, true, false, true, true, true},
                    new []{true, true, true, true, true, true, true},
                    new []{true, true, true, true, true, true, true},
                },
                _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, null)
            };
        }
    }
}