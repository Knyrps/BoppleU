using System;
using System.Linq;
using Scripts.Grid;
using UnityEngine;
using GridLayout = Scripts.Grid.GridLayout;

namespace Scripts.GameObjects
{
    /// <summary>
    /// Since this is a controller exclusive to the Game Scene, it will be destroyed on load.
    /// </summary>
    public class PlayFieldController : MonoBehaviour
    {
        private GridCell[][] _grid;

        [SerializeField]
        [Tooltip("Margins of the play field in which no actual fields will be generated.")]
        private Vector2 margins = new(0.2f, 0.2f);

        [SerializeField]
        [Tooltip("The default grid layout.")]
        private GridLayout defaultGridLayout = GridLayout.Random;

        [SerializeField]
        [Tooltip("The prefab used to generate the grid cells.")]
        private GameObject gridCellPrefab;

        [SerializeField]
        [Tooltip("The minimum gap between grid columns and rows.")]
        private float minGridGap = .125f;

        private (int x, int y) _bounds;

        private Vector2 _absSize;

        private Vector2 _absPos;

        private Vector2 UsableSize => new Vector2(this._absSize.x - this.margins.x, this._absSize.y - this.margins.y);

        void Awake()
        {
            this._absSize = this.gameObject.GetComponent<RectTransform>().rect.size;
            this._absPos = this.gameObject.GetComponent<RectTransform>().transform.position;

            Debug.Log("Absolute Size: (" + this._absSize.x + ", " + this._absSize.y + ")");
            Debug.Log("Absolute Position: (" + this._absPos.x + ", " + this._absPos.y + ")");
        }

        public GridCell GetGridCellAtPos(int x, int y)
        {
            return PositionInBoundsOfGrid(x, y) ? this._grid[x][y] : null;
        }

        public bool TryInitializeGrid(GridLayout layout = (GridLayout)int.MinValue)
        {
            if (layout == (GridLayout)int.MinValue)
            {
                layout = this.defaultGridLayout;
            }

            bool[][] pattern = GridLayoutResolver.Resolve(layout);

            this.PopulateBoundsFromEnumerable(pattern);
            this.PrepareGridFromEnumerable(pattern);

            this.GenerateGridCells(pattern);

            return true;
        }

        private void PopulateBoundsFromEnumerable<T>(T[][] array)
        {
            this._bounds = (array.Length, array.Max(rows => rows.Length));
        }

        private void PrepareGridFromEnumerable<T>(T[][] array)
        {
            this._grid = new GridCell[array.Length][];
            for (int i = 0; i < this._grid.Length; i++)
            {
                this._grid[i] = new GridCell[array[i].Length];
            }
        }

        private bool PositionInBoundsOfGridOneBased(int x, int y) => this.PositionInBoundsOfGrid(x - 1, y - 1);

        private bool PositionInBoundsOfGrid(int x, int y)
        {
            if (x < 0 || x >= this._bounds.x)
            {
                return false;
            }

            return y >= 0 && y < this._bounds.y;
        }

        private void GenerateGridCells(bool[][] pattern)
        {
            for (int iX = 0; iX < pattern.Length; iX++)
            {
                bool[] row = pattern[iX];
                for (int iY = 0; iY < row.Length; iY++)
                {
                    bool cellEnabled = pattern[iX][iY];
                    GameObject cellObject = Instantiate(this.gridCellPrefab);
                    GridCell gridCell = cellObject.GetComponent<GridCell>();
                    if (!cellEnabled)
                    {
                        gridCell.Disable();
                    }

                    if (!TryCalculateAbsolutePositionAndSizeOfCell(
                            // iX, 1-based
                            iX + 1,
                            // iY, 1-based
                            iY + 1,
                            // out: vector
                            out (Vector2? position, float? cellSize) cellTransform
                        ) || !cellTransform.position.HasValue || !cellTransform.cellSize.HasValue)
                    {
                        continue;
                    }

                    // Validate positive
                    if (cellTransform.cellSize.Value <= 0)
                    {
                        continue;
                    }

                    Debug.DrawLine(cellTransform.position.Value - Vector2.one * 0.2f,
                        cellTransform.position.Value + Vector2.one * 0.2f,
                        Color.green,
                        10f);

                    gridCell.UpdatePositionAndSize(cellTransform.position.Value, cellTransform.cellSize.Value);

                    this._grid[iX][iY] = gridCell;
                }
            }
        }

        private bool TryCalculateAbsolutePositionAndSizeOfCell(int xInGrid, int yInGrid, out (Vector2? position, float? cellSize) cellTransform)
        {
            cellTransform.position = null;
            cellTransform.cellSize = null;

            if (!PositionInBoundsOfGridOneBased(xInGrid, yInGrid))
            {
                return false;
            }

            // Calculate the position of the left and top border of the usable area for the grid
            float leftBorderPos = this._absPos.x - (this.UsableSize.x / 2);
            float topBorderPos = this._absPos.y + (this.UsableSize.y / 2);

            float maxCellSpaceX = (this.UsableSize.x - (this.minGridGap * (this._bounds.x - 1))) / this._bounds.x;
            float maxCellSpaceY = (this.UsableSize.y - (this.minGridGap * (this._bounds.y - 1))) / this._bounds.y;

            // Calculate the size of a cell
            float cellSize = Math.Min(maxCellSpaceY, maxCellSpaceX);

            // Calculate the gap between rows
            float gridRowGap =
                this._bounds.y > 1 ?
                    (this.UsableSize.y - (cellSize * this._bounds.y)) / (this._bounds.y - 1)
                    : 0;
            float gridColGap =
                this._bounds.x > 1 ?
                    (this.UsableSize.x - (cellSize * this._bounds.x)) / (this._bounds.x - 1)
                    : 0;
            // Calculate the positions of the cell's left and top borders
            float xPosLeftBorder = leftBorderPos + (cellSize * (xInGrid - 1)) + (gridColGap * (xInGrid - 1));
            float yPosTopBorder = topBorderPos - (cellSize * (yInGrid - 1)) - (gridRowGap * (yInGrid - 1));

            // Calculate the position of the cell's center point
            cellTransform.position = new Vector2(xPosLeftBorder + cellSize / 2, yPosTopBorder - cellSize / 2);
            cellTransform.cellSize = cellSize;

            Debug.Log($"Cell in row {xInGrid}, col {yInGrid} Position: ({cellTransform.position.Value.x}, {cellTransform.position.Value.y}) Size: {cellTransform.cellSize}");

            return true;
        }
    }
}
