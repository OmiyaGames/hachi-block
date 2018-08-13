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

        int lastScore = -1;
        GameDifficulty.Difficulty lastDifficulty = null;

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

        public GameDifficulty.Difficulty CurrentDifficulty
        {
            get
            {
                if ((lastDifficulty == null) || (lastScore != Score))
                {
                    lastDifficulty = allDifficulties.GetDifficulty(Score);
                    lastScore = Score;
                }
                return lastDifficulty;
            }
        }

        public void Reset()
        {
            NumberOfMoves = 0;
            Score = 0;

            Settings.LastGameGrid = "";
            Settings.LastGameInventory = "";
            Settings.LastGamePreview = "";
            Settings.SaveSettings();

            lastScore = -1;
            lastDifficulty = null;
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

        public bool RestoreGridSettings()
        {
            bool isRestored = false;
            if(string.IsNullOrEmpty(Settings.LastGameInventory) == false)
            {
                Block block;
                int s = 0;
                for (int x = 0; x < grid.Width; ++x)
                {
                    for (int y = 0; y < grid.Height; ++y)
                    {
                        // Decrypt block
                        block = DecryptBlock(Settings.LastGameGrid[s]);
                        ++s;

                        // Update grid
                        if(block == null)
                        {
                            grid.RemoveBlock(x, y);
                        }
                        else
                        {
                            grid.CreateBlock(block, x, y);
                        }
                    }
                }
                isRestored = true;
            }
            return isRestored;
        }

        public bool RestorePreviewSettings()
        {
            bool isRestored = false;
            if (string.IsNullOrEmpty(Settings.LastGamePreview) == false)
            {
                for (int x = 0; x < Settings.LastGamePreview.Length; ++x)
                {
                    preview.Cells[x].Block = DecryptBlock(Settings.LastGamePreview[x]);
                }
                isRestored = true;
            }
            return isRestored;
        }

        public bool RestoreInventorySettings()
        {
            bool isRestored = false;
            if (string.IsNullOrEmpty(Settings.LastGameInventory) == false)
            {
                int s = 0;
                foreach (Inventory item in inventory.AllInventories)
                {
                    item.TopLeftBlock = DecryptBlock(Settings.LastGameInventory[s]);
                    ++s;

                    item.TopRightBlock = DecryptBlock(Settings.LastGameInventory[s]);
                    ++s;

                    item.BottomLeftBlock = DecryptBlock(Settings.LastGameInventory[s]);
                    ++s;

                    item.BottomRightBlock = DecryptBlock(Settings.LastGameInventory[s]);
                    ++s;
                }
                isRestored = true;
            }
            return isRestored;
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
            return (char)(((int)type) + conversionBuffer);
        }

        private Block DecryptBlock(char type)
        {
            return allBlocks.GetBlockPrefab(Decrypt(type));
        }

        private Block.BlockType Decrypt(char type)
        {
            return (Block.BlockType)(type - conversionBuffer);
        }
    }
}
