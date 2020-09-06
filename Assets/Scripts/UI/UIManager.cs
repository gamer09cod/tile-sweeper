using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace TileSweeper
{
    /// <summary>
    /// Class to manage UIs.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public GameObject gameOverUI;
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI movesText;

        //Singleton instance
        public static UIManager Instance;

        private float tweenTime = 0.2f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);
        }

        #region Public Methods
        public void ShowPopup(string message)
        {
            if (!gameOverUI.activeInHierarchy)
            {
                gameOverUI.SetActive(true);
                messageText.text = message;
                LeanTween.scale(gameOverUI, Vector3.one, tweenTime);                
            }
        }

        public void HidePopup()
        {
            if (gameOverUI.activeInHierarchy)
            {
                LeanTween.scale(gameOverUI, Vector3.zero, tweenTime).setOnComplete(()=> gameOverUI.SetActive(false));
            }
        }

        public void UpdateMoves(int moves)
        {
            movesText.text = moves.ToString();
        }
        #endregion
    }
}
