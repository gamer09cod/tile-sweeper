using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace TileSweeper
{
    /// <summary>
    /// UI class for cell. Inherits Unity.Button class.
    /// </summary>
    public class CellMono : Button
    {
        #region Walls
        public GameObject Left;
        public GameObject Right;
        public GameObject Top;
        public GameObject Bottom;
        #endregion

        public Image cellImage;
        public GameObject cellBorder;
        public TextMeshProUGUI indexText;//For debug purposes.
        public Vector2 gridIndex;
        public Item item;
        public Action<ItemColor> onProcessed;
 
        public bool ContainsItem { get { return item != null ? true : false; } }

        #region Public Methods
        public void Init(Vector2 pos, string indexID)
        {
            this.gridIndex = pos;
            //SetIndexText(indexID);//For debug purposes.
        }

        public void OnClickedMe()
        {
            Select();
            GameManager.Instance.OnClickedCell(this);
        }

        public void ResetData()
        {
            cellImage.gameObject.SetActive(false);
            cellBorder.gameObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        private void SetIndexText(string text)
        {
            indexText.text = text;
        }

        private void OnProcessed(ItemColor color)
        {
            switch (color)
            {
                case ItemColor.Red:
                    cellImage.color = Color.red;
                    break;
                case ItemColor.Yellow:
                    cellImage.color = Color.yellow;
                    break;
                case ItemColor.Green:
                    cellImage.color = Color.green;
                    break;

            }
            cellImage.gameObject.SetActive(true);
        }
        #endregion

        #region Overrides
        protected override void Start()
        {
            onProcessed += OnProcessed;
        }

        public override void Select()
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            GameManager.Instance.SetCurrentCell = this.gridIndex;
            cellBorder.SetActive(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            cellBorder.SetActive(false);
        }

        protected override void OnDisable()
        {
            onProcessed -= OnProcessed;
        }


        #endregion
    }
}
