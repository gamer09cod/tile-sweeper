using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace TileSweeper
{
    /// <summary>
    /// Class to generate items at random tiles.
    /// Can be used in Edit mode.
    /// </summary>
    [ExecuteInEditMode]
    public class DebugHelper : MonoBehaviour
    {
        public List<CellMono> cells;
        public GameObject itemPrefab;
        public bool test = false;

        private int min = 0;
        private int max = 399;
        
#if UNITY_EDITOR
        void Update()
        {
            if (test)
            {
                test = false;
                AlterCellData();
                //GenerateRandomNumbers(10);
            }
        }
#endif
        /// <summary>
        /// Generates random numbers between min and max range.
        /// </summary>
        /// <param name="count">Count of random numbers to be generated</param>
        public void GenerateRandomNumbers(int count)
        {
            List<int> randomNumbers = new List<int>();
            System.Random random = new System.Random();
            for (int i = 0; i < count; i++)
            {
                int number;

                do number = random.Next(min, max);
                while (randomNumbers.Contains(number));

                randomNumbers.Add(number);
            }

            for (int i = 0; i < 10; i++)
            {
                Debug.Log("i = " + randomNumbers[i] + "\n");
            }

            AddItem(randomNumbers);
        }

        /// <summary>
        /// Add items to random cells(tiles)
        /// </summary>
        /// <param name="indexes">List of random indexes</param>
        void AddItem(List<int> indexes)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                GameObject obj = Instantiate(itemPrefab, cells[indexes[i]].transform);
                Item item = obj.GetComponent<Item>();
                if (item != null)
                {
                    cells[i].item = item;
                }
            }
        }

        /// <summary>
        /// Alter/Check cell info(In Edit time) based on a specific condition.
        /// </summary>
        void AlterCellData()
        {
            foreach (CellMono cell in cells)
            {
                //GameObject obj = Instantiate(itemPrefab, cell.transform);
                //Image[] comps = cell.GetComponentsInChildren<Image>(true);
                //if (comps.Length > 0)
                //{
                //cell.indexText = comps[comps.Length - 1];
                //DestroyImmediate(comps[comps.Length - 1].gameObject);
                //}

                //Change navigation to none.
                Navigation nav = cell.navigation;
                nav.mode = Navigation.Mode.None;
                cell.navigation = nav;

                //cell.cellBorder = obj;
#if UNITY_EDITOR
                //UnityEditor.Events.UnityEventTools.RemovePersistentListener(cell.onClick, 0);
                //UnityEditor.Events.UnityEventTools.AddPersistentListener(cell.onClick, new UnityAction(cell.OnClickedMe));
#endif

                if (cell.cellImage == null)
                    Debug.Log("No Cell Border assigned" + cell.name);
            }
        }
    }
}
