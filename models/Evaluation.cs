namespace chess.api.models
{
    public class Evaluation
    {
        public string FEN { get; set; }
        public long KNodes { get; set; }
        public int Depth { get; set; }
        public List<PV> Pvs { get; set; }
    }

    public class PV
    {
        public int Index { get; set; }
        public string Moves { get; set; }
        public string Eval { get; set; }
    }

    public class SinglePointEval
    {
        public string Value { get; set; }
    }
}
