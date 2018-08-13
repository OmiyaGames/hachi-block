using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class PreviewCell : MonoBehaviour
    {
        [SerializeField]
        Image background;
        [SerializeField]
        Image symbol;

        Block block;

        public Block Block
        {
            get
            {
                return block;
            }
            set
            {
                block = value;
                if(block != null)
                {
                    // Setup background
                    background.sprite = block.Graphic.sprite;
                    background.color = block.Graphic.color;

                    // Setup symbol
                    symbol.sprite = block.Symbol.sprite;
                    symbol.color = block.Symbol.color;
                }

                // Update image visibility
                background.gameObject.SetActive(block != null);
                symbol.gameObject.SetActive(block != null);
            }
        }
    }
}
