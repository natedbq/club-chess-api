using chess.api.dal;
using chess.api.models;

namespace chess.api.repository
{
    public class PositionRepository
    {
        private PositionDal positionDal = new PositionDal();

        public void Delete(Guid id)
        {
            positionDal.Delete(id);
        }

        public async Task Save(Position position, Guid userId)
        {
            await positionDal.Save(position, userId);
        }

        public Position GetById(Guid id, Guid userId, int depth = 0)
        {
            return positionDal.GetById(id, userId, depth);
        }

        public IList<Position> GetByParentId(Guid id, Guid userId, int depth = 0)
        {
            return positionDal.GetByParentId(id,userId, depth);
        }

        public void StudyPosition(Guid positionId, Guid userId)
        {
            positionDal.StudyPosition(positionId, userId);
        }

        public void Mistake(Guid positionId, Guid userId)
        {
            positionDal.Mistake(positionId, userId);
        }

        public void Correct(Guid positionId, Guid userId)
        {
            positionDal.Correct(positionId, userId);
        }
    }
}
