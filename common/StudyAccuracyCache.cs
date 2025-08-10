using chess.api.dal;
using chess.api.models;
using ChessApi.dal;
using System.Runtime.CompilerServices;

namespace chess.api.common
{
    public class StudyAccuracyCache
    {
        private static Dictionary<Guid, Double> Cache = new Dictionary<Guid, Double>();

        private static readonly StudyDal _studyDal = new StudyDal();
        private static readonly PositionDal _positionDal = new PositionDal();

        public static void Invalidate(Guid studyId)
        {
            Cache.Remove(studyId);
            _studyDal.UpdateScore(studyId, -1);
        }

        public static double GetAccuracy(Guid studyId)
        {
            if (Cache.ContainsKey(studyId))
            {
                return Cache[studyId];
            }

            var study = _studyDal.GetById(studyId);
            study.Position = _positionDal.GetById(study.PositionId.Value, 2000);


            var score = Calculate(study.Position);

            var v = ((double)score.Total) / ((double)score.Total + score.Mistakes) * 100;
            Cache.Add(studyId, v);

            return v;
        }

        private static Score Calculate(Position p)
        {
            var score = new Score()
            {
                Total = 1,
                Mistakes = p.Mistakes
            };

            foreach(var sp in p.Positions)
            {
                var s = Calculate(sp);
                score.Total += s.Total;
                score.Mistakes += s.Mistakes;
            }

            return score;
        }
        private class Score
        {
            public int Total { get; set; }
            public long Mistakes { get; set; }
        }
    }

}
