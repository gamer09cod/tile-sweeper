using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TileSweeper
{
    public class InputHandler : MonoBehaviour
    {
        public delegate void OnButtonclicked(KeyCode key);
        public static OnButtonclicked onButtonClicked;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                onButtonClicked.Invoke(KeyCode.UpArrow);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                onButtonClicked.Invoke(KeyCode.DownArrow);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                onButtonClicked.Invoke(KeyCode.LeftArrow);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                onButtonClicked.Invoke(KeyCode.RightArrow);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                onButtonClicked.Invoke(KeyCode.Space);
            }
        }
    }
}
