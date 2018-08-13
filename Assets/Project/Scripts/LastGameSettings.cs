using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Settings;

namespace Project
{
    public class LastGameSettings : MonoBehaviour
    {
        public static LastGameSettings Instance
        {
            get;
            private set;
        }

        [SerializeField]
        BlockGrid grid;
        [SerializeField]
        PreviewRow preview;
        [SerializeField]
        InventoryCollection inventory;
        [SerializeField]
        BlockCollection allBlocks;
        [SerializeField]
        GameDifficulty allDifficulties;

        static GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        public int NumberOfMoves
        {
            get
            {
                return Settings.LastGameNumberOfMoves;
            }
            set
            {
                Settings.LastGameNumberOfMoves = value;
            }
        }

        public int Score
        {
            get
            {
                return Settings.LastGameScore;
            }
            set
            {
                Settings.LastGameScore = value;
            }
        }

        //public GameDifficulty.Difficulty

        public void Reset()
        {
            NumberOfMoves = 0;
            Score = 0;
            Settings.LastGameGrid = "";
            Settings.LastGameInventory = "";
            Settings.LastGamePreview = "";
        }

        public void SaveSettings()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            // Save grid
            SaveGrid(builder);

            // Save preview
            SavePreview(builder);

            // Save inventory
            SaveInventory(builder);

            Settings.SaveSettings();
        }

        private void SaveInventory(System.Text.StringBuilder builder)
        {
            Block block;
            builder.Clear();
            foreach (Inventory item in inventory.AllInventories)
            {
                block = item.TopLeftBlock;
                builder.Append(Encrypt(block));

                block = item.TopRightBlock;
                builder.Append(Encrypt(block));

                block = item.BottomLeftBlock;
                builder.Append(Encrypt(block));

                block = item.BottomRightBlock;
                builder.Append(Encrypt(block));
            }
            Settings.LastGameInventory = builder.ToString();
        }

        private void SavePreview(System.Text.StringBuilder builder)
        {
            Block block;
            builder.Clear();
            foreach (PreviewCell cell in preview.Cells)
            {
                block = cell.Block;
                builder.Append(Encrypt(block));
            }
            Settings.LastGamePreview = builder.ToString();
        }

        private void SaveGrid(System.Text.StringBuilder builder)
        {
            Block block;
            builder.Clear();
            for (int x = 0; x < grid.Width; ++x)
            {
                for (int y = 0; y < grid.Height; ++y)
                {
                    block = grid.Blocks[x, y];
                    builder.Append(Encrypt(block));
                }
            }
            Settings.LastGameGrid = builder.ToString();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        const char conversionBuffer = '1';

        private char Encrypt(Block block)
        {
            if (block == null)
            {
                return Encrypt(Block.BlockType.Invalid);
            }
            else
            {
                return Encrypt(block.Type);
            }
        }

        private char Encrypt(Block.BlockType type)
        {
            int convertTo = (int)type;
            convertTo += conversionBuffer;
            return (char)convertTo;
        }

        private Block.BlockType Decrypt(char type)
        {
            int convertTo = (type - conversionBuffer);
            return (Block.BlockType)convertTo;
        }
    }
}
