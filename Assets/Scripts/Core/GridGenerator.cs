using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileSweeper
{
    /// <summary>
    /// Class to create grid of N x M size.
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        //List containing references of pre-generated grid cells. Assign from inspector.
        public List<CellMono> cells;

        public List<Cell> unVisited = new List<Cell>();
        public List<Cell> stackedCells = new List<Cell>();
        public GameObject cellPrefab;
        public Transform gridParent;

        //Assign from inspector.
        public int columns = 50;
        public int rows = 50;

        private float xOffset = 50f;
        private float yOffset = 50f;
        private float cellSize;

        //Dictionary containing data of all the cells in the grid.
        private Dictionary<Vector2, Cell> gridInfo = new Dictionary<Vector2, Cell>();

        //Cells
        private Cell currentCell;
        private Cell nextCell;
        private List<Side> sides = new List<Side>() { Side.Top, Side.Bottom, Side.Left, Side.Right };
        private Vector2 initialPosition;

        public static GridGenerator Instance;

        public Dictionary<Vector2, Cell> GridInfo { get { return gridInfo; } }

        #region Unity Callbacks
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);
        }

        private void Start()
        {
            InitDefaults();

            //Initialize grid info for already created grid(Editor).
            InitializeGridInfo();
        }
        #endregion

        #region Private Methods
        private void InitDefaults()
        {
            cellSize = 50;

            //Start from bottom left
            initialPosition = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)) + new Vector3(xOffset, yOffset, 0f);

            unVisited.Clear();
        }

        /// <summary>
        /// Initialzes grid and computes links between cells.
        /// </summary>
        private void InitializeGridInfo()
        {
            int cellIndex = 0;
            for (int i = 0; i < rows; i++)//Rows
            {
                for (int j = 0; j < columns; j++)//Columns
                {
                    Vector2 index = new Vector2(i, j);

                    CellMono cellMono = cells[cellIndex];

                    //Create cell object
                    Cell cell = new Cell(index, cellMono);
                    gridInfo.Add(index, cell);
                    cellIndex++;

                    //Making all the cells as unvisited by default.
                    unVisited.Add(cell);

                }
            }

            ComputeLinks();
        }

        /// <summary>
        /// Generates a grid of specified size(rows and columns).
        /// </summary>
        private void GenerateGrid()
        {
            Vector2 currentSpawnPosition = initialPosition;

            for (int i = 0; i < rows; i++)//Rows
            {
                for (int j = 0; j < columns; j++)//Columns
                {
                    Vector2 index = new Vector2(i, j);
                    GameObject obj = Instantiate(cellPrefab, currentSpawnPosition, cellPrefab.transform.rotation, gridParent);
                    obj.name = "Cell(" + i + "," + j + ")";

                    CellMono cellM = obj.GetComponent<CellMono>();
                    cellM.Init(index, "(" + i + ", " + j + ")");

                    Cell cell = new Cell(index, cellM);

                    gridInfo.Add(index, cell);

                    //Making all the cells as unvisited by default.
                    unVisited.Add(cell);

                    //Set y as per cell size.
                    currentSpawnPosition.y += cellSize;
                }

                //Reset y for next column
                currentSpawnPosition.y = initialPosition.y;

                currentSpawnPosition.x += cellSize;
            }

            ComputeLinks();

            //DisplayNeighbourInfo();
        }

        /// <summary>
        /// Function to set-up links between differnt cells by populating neighbours(Top, Bottom, Left, Right) of individual cells.
        /// </summary>
        private void ComputeLinks()
        {
            Vector2 posToCheck;
            Vector2 currentKey;

            for (int i = 0; i < rows; i++)//Horizontal column iteration left to right,
            {
                for (int j = 0; j < columns; j++)//vertical row iteration bottom to top
                {
                    currentKey = new Vector2(i, j);
                    foreach (Side side in sides)
                    {
                        if (i > rows || j > columns)
                            break;

                        switch (side)
                        {
                            case Side.Top:
                                posToCheck = new Vector2(i, j + 1);
                                if (gridInfo.ContainsKey(posToCheck))
                                {
                                    Cell cell;
                                    gridInfo.TryGetValue(posToCheck, out cell);
                                    gridInfo[currentKey].neighbours.Add(Side.Top, cell);
                                }
                                break;
                            case Side.Bottom:
                                posToCheck = new Vector2(i, j - 1);
                                if (gridInfo.ContainsKey(posToCheck))
                                {
                                    Cell cell;
                                    gridInfo.TryGetValue(posToCheck, out cell);
                                    gridInfo[currentKey].neighbours.Add(Side.Bottom, cell);
                                }
                                break;
                            case Side.Left:
                                posToCheck = new Vector2(i - 1, j);
                                if (gridInfo.ContainsKey(posToCheck))
                                {
                                    Cell cell;
                                    gridInfo.TryGetValue(posToCheck, out cell);
                                    gridInfo[currentKey].neighbours.Add(Side.Left, cell);
                                }
                                break;
                            case Side.Right:
                                posToCheck = new Vector2(i + 1, j);
                                if (gridInfo.ContainsKey(posToCheck))
                                {
                                    Cell cell;
                                    gridInfo.TryGetValue(posToCheck, out cell);
                                    gridInfo[currentKey].neighbours.Add(Side.Right, cell);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void DisplayNeighbourInfo()
        {
            foreach (KeyValuePair<Vector2, Cell> cell in gridInfo)
            {
                Debug.Log("Cell" + cell.Value.position.ToString() + "\n");

                foreach (KeyValuePair<Side, Cell> side in cell.Value.neighbours)
                {
                    Debug.Log(side.ToString() + "" + side.Value.position.ToString() + "\n");
                }
            }
        }
        #endregion

        #region Public Methods
        public List<Cell> GetUnvisitedNeighbours(Cell currentCell)
        {
            List<Cell> cells = new List<Cell>();
            foreach (KeyValuePair<Side, Cell> pair in currentCell.neighbours)
            {
                if (unVisited.Contains(pair.Value))
                    cells.Add(pair.Value);
            }

            return cells;
        }
        #endregion
    }

    public enum Wall
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum Side
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }

    public enum ItemColor
    {
        Red,
        Yellow,
        Green
    }
}