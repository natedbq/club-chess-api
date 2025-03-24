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

        public void Save(Position position)
        {
            positionDal.Save(position);
        }

        public Position GetById(Guid id, int depth = 0)
        {
            return positionDal.GetById(id, depth);
        }

        public IList<Position> GetByParentId(Guid id, int depth = 0)
        {
            return positionDal.GetByParentId(id, depth);
        }
    }
}
