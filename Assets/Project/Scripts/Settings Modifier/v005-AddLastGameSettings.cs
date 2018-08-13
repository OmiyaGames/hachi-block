using OmiyaGames.Settings;

namespace Project.Settings
{
    public class AddLastGameSettings : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 5;

        public override ushort Version
        {
            get
            {
                return AppVersion;
            }
        }

        protected override string[] GetKeysToRemove()
        {
            // Do nothing!
            return null;
        }

        protected override IGenerator[] GetNewGenerators()
        {
            return new IGenerator[]
            {
            /////////////////////////////////////////////////////
            // Last Game Stuff
            /////////////////////////////////////////////////////
            new StoredStringGenerator("LastGameGrid", "")
            {
                TooltipDocumentation = new string[]
                {
                    "Encrypted String storing grid state."
                }
            },
            new StoredStringGenerator("LastGamePreview", "")
            {
                TooltipDocumentation = new string[]
                {
                    "Encrypted String storing preview row."
                }
            },
            new StoredStringGenerator("LastGameInventory", "")
            {
                TooltipDocumentation = new string[]
                {
                    "Encrypted String storing inventory."
                }
            },
            new StoredIntGenerator("LastGameNumberOfMoves", 0)
            {
                TooltipDocumentation = new string[]
                {
                    "Number of moves made from the last game."
                }
            },
            new StoredIntGenerator("LastGameScore", 0)
            {
                TooltipDocumentation = new string[]
                {
                    "Score from last game."
                }
            },
            };
        }
    }
}
