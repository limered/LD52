namespace Systems.Levels
{
    public class Level
    {
        /// <summary>
        /// set in LevelSystem
        /// </summary>
        public int Index { get; set; } = default;
        public string Name { get; set; }
        public string File { get; set; }
        public DrescherDirection StartDirection { get; set; }
        public int TopArrows { get; set; }
        public int LeftArrows { get; set; }
        public int RightArrows { get; set; }
        public int BottomArrows { get; set; }
    }
}