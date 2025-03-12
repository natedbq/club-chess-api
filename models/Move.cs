namespace chess.api.models
{
    public class Move
    {
        public Move(string fEN, string name)
        {
            FEN = fEN;
            Name = name;
        }

        public string FEN { get; set; }
        public string Name { get; set; }
    }
}
