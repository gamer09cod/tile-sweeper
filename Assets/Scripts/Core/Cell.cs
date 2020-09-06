using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileSweeper
{
    /// <summary>
    /// Base class for cell(Tile).
    /// </summary>
    public class Cell
    {
        //Cell Index in the grid.
        public Vector2 position;
        //Reference to CellMono script.
        public CellMono cellMonobehaviour;

        //Dictionary containing neighbour info for this cell. Populated while computing links.
        public Dictionary<Side, Cell> neighbours = new Dictionary<Side, Cell>();

        /// <summary>
        /// Constructor to initialize cell object.
        /// </summary>
        /// <param name="pos">Index of the cell in grid</param>
        /// <param name="cellM">Reference to CellMono script</param>
        public Cell(Vector2 pos, CellMono cellM)
        {
            this.position = pos;
            this.cellMonobehaviour = cellM;
        }

        /// <summary>
        /// Finds distance between cells.
        /// </summary>
        /// <param name="c">Destination cell</param>
        /// <returns></returns>
        public float Distance(Cell c)
        {
            return Mathf.Max(Mathf.Abs(this.position.x - c.position.x), Mathf.Abs(this.position.y - c.position.y));
        }
    }
}
