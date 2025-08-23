using chess.api.dal;
using chess.api.models;
using ChessApi.dal;
using System.Runtime.CompilerServices;

namespace chess.api.common
{
    public class StudyAccuracyCache
    {

        private static readonly StudyDal _studyDal = new StudyDal();
        private static readonly PositionDal _positionDal = new PositionDal();

        public static void Invalidate(Guid studyId, Guid userId)
        {
            _studyDal.UpdateScore(studyId, userId, -1);
        }

        public static async Task<double> GetAccuracy(Guid studyId,Guid userId)
        {

            var study = await _studyDal.GetById(studyId, userId);

            if(study.Score == null)
            {
                return 0;
            }

            study.Position = _positionDal.GetById(study.PositionId.Value, userId, depth: 2000);


            var score = Calculate(study.Position);

            var v = ((double)score.Total) / ((double)score.Total + score.Mistakes) * 100;

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
