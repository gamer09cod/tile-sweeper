using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace TileSweeper
{
    /// <summary>
    /// Class managing user session and gameplay logic.
    /// </summary>
    public class GameManager : MonoBehaviour, IGameplayLogic
    {
        //Singleton instance.
        public static GameManager Instance;
        public bool test = false;

        public Vector2 SetCurrentCell 
        { 
            set 
            {
                if (gridInfo.ContainsKey(value))
                    currentCell = gridInfo[value];
            } 
        }

        private int moves;
        private int itemCellsFound = 0;
        private Cell currentCell;
        private CellMono lastClickedCell;
        //Holds references to all item cells.
        private List<CellMono> itemCells = new List<CellMono>();
        //Grid data.
        private Dictionary<Vector2, Cell> gridInfo = new Dictionary<Vector2, Cell>();

        #region Unity Callbacks
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);
        }

        void Start()
        {           
            InitializeGame();
        }

        void OnEnable()
        {
            InputHandler.onButtonClicked += OnKeyPressed;
        }

        void OnDisable()
        {
            InputHandler.onButtonClicked -= OnKeyPressed;
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Handle input specific to a key event from InputManager.
        /// </summary>
        /// <param name="key">Pressed Key</param>
        void OnKeyPressed(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.UpArrow:
                    Traverse(Side.Top);
                    break;
                case KeyCode.DownArrow:
                    Traverse(Side.Bottom);
                    break;
                case KeyCode.LeftArrow:
                    Traverse(Side.Left);
                    break;
                case KeyCode.RightArrow:
                    Traverse(Side.Right);
                    break;
                //case KeyCode.Space:
                //    break;
            }
        }

        /// <summary>
        /// Traverses to a neighbouring cell if it exists.
        /// </summary>
        /// <param name="side"></param>
        void Traverse(Side side)
        {
            if (!currentCell.neighbours.ContainsKey(side))
                return;
            
            if (gridInfo.ContainsKey(currentCell.neighbours[side].position))
            {
                currentCell.neighbours[side].cellMonobehaviour.Select();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called when user clicks on a cell.
        /// </summary>
        /// <param name="cell"></param>
        public void OnClickedCell(CellMono cell)
        {
            if (lastClickedCell.Equals(cell))
                return;

            //Check if moves are over.
            if (moves <= 0)
            {
                UIManager.Instance.ShowPopup(Constants.MOVES_OVER);
                return;
            }

            //Update moves
            moves--;
            UIManager.Instance.UpdateMoves(moves);

            ExamineCell(cell);

            //Check if all item cells found.
            if (itemCellsFound == Constants.MAX_ITEMS_CELLS)
                UIManager.Instance.ShowPopup(Constants.WIN_MESSAGE);
        }

        public void ResetGame()
        {
            UIManager.Instance.HidePopup();
            InitializeGame();           
        }

        public void Quit()
        {
            UIManager.Instance.HidePopup();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Interface Methods
        List<Cell> unVisited = new List<Cell>();
        List<Cell> visited = new List<Cell>();

        /// <summary>
        /// Finds a cell containing an item starting from position in the grid.
        /// </summary>
        /// <param name="position">Position of current cell</param>
        /// <returns>Index of cell</returns>
        public Vector2 FindItemCell(Vector2 position)
        {
            unVisited.Clear();
            visited.Clear();

            Cell currentCell = null;
            //Make all cells unvisited.
            foreach (KeyValuePair<Vector2, Cell> kvp in gridInfo)
                unVisited.Add(kvp.Value);

            currentCell = unVisited.Find(cell => cell.position.Equals(position));

            //Remove current cell from visited.
            unVisited.Remove(currentCell);
            //Make current cell as visited.
            visited.Add(currentCell);

            //If current cell contains an item return.
            if (currentCell.cellMonobehaviour.item != null)
                return currentCell.position;

            //Recursively iterate over neighbouring cells till we find a cell with an item.
            while (unVisited.Count > 0)
            {
                Side side = GetNeighbouringSide(currentCell);
                if (side != Side.None)
                {
                    currentCell = currentCell.neighbours[side];

                    unVisited.Remove(currentCell);

                    visited.Add(currentCell);

                    if (currentCell.cellMonobehaviour.item != null)
                        return currentCell.position;
                }
                else
                {
                    currentCell = visited[visited.Count - 1];
                    visited.Remove(currentCell);
                }
            
            }            
            return new Vector2(-1f, -1f);//If no item cell found return invalid index.
        }

        /// <summary>
        /// Checks if a cell contains an item, otherwise finds closest item cell.
        /// </summary>
        /// <param name="position">Position of current cell</param>
        /// <returns>Distance(Cell count) to closest item cell</returns>
        public int DistanceToNearestItemCell(CellMono cell)
        {
            //If current cell contains an item return.
            if (cell.ContainsItem)
                return 0;
            else
            {
                Vector2 pos = cell.gridIndex;
                Vector2 closestItemCellIndex = itemCells[0].gridIndex;
                //Assuming the minimum distance is between pos and first cell in itemCells.
                int minDistance = (int)Mathf.Max(Mathf.Abs(pos.x - itemCells[0].gridIndex.x), Mathf.Abs(pos.y - itemCells[0].gridIndex.y));
                int distance = 0;

                for (int i = 1; i < itemCells.Count; i++)
                {
                    distance = (int)Mathf.Max(Mathf.Abs(pos.x - itemCells[i].gridIndex.x), Mathf.Abs(pos.y - itemCells[i].gridIndex.y));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItemCellIndex = itemCells[i].gridIndex;
                    }
                }

                return minDistance;
            }
        }
        #endregion

        #region Private Methods
        private void InitializeGame()
        {
            //Cache grid data.
            gridInfo = GridGenerator.Instance.GridInfo;

            itemCells.Clear();
            foreach (KeyValuePair<Vector2, Cell> kvp in gridInfo)
            {
                //Reset cells.
                kvp.Value.cellMonobehaviour.ResetData();

                //CacheItemCells();
                if (kvp.Value.cellMonobehaviour.ContainsItem)
                    itemCells.Add(kvp.Value.cellMonobehaviour);
            }

            //Make cell(0,0) as selected on start.
            currentCell = gridInfo[new Vector2(0, 0)];
            currentCell.cellMonobehaviour.Select();
            lastClickedCell = currentCell.cellMonobehaviour;
            itemCellsFound = 0;
            moves = Constants.MAX_MOVES;
            UIManager.Instance.UpdateMoves(moves);         
        }

        /// <summary>
        /// Examines a cell and its neighbours for item cells and sets appropriate color.
        /// </summary>
        /// <param name="cell">Cell to be examined.</param>
        private void ExamineCell(CellMono cell)
        {
            Vector2 source = cell.gridIndex;
            int distance = DistanceToNearestItemCell(cell);

            if (distance == 0)
            {
                cell.onProcessed.Invoke(ItemColor.Red);
                itemCellsFound++;
            }
            else if (distance <= 2)
                cell.onProcessed.Invoke(ItemColor.Yellow);
            else
                cell.onProcessed.Invoke(ItemColor.Green);

            lastClickedCell = cell;
        }

        /// <summary>
        /// Gets a neighbour for a cell if it exists.
        /// </summary>
        /// <param name="cell">Cell whose neighbour is to be found</param>
        /// <returns></returns>
        private Side GetNeighbouringSide(Cell cell)
        {
            foreach (KeyValuePair<Side, Cell> kvp in cell.neighbours)
            {
                if (unVisited.Contains(kvp.Value))
                    return kvp.Key;
            }

            return Side.None;
        }
        #endregion
    }
}
