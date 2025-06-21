using chess.api.dal;
using chess.api.models;
using ChessApi.dal;

namespace chess.api.repository
{
    public class StudyRepository
    {
        private StudyDal dal = new StudyDal();
        private PositionDal positionDal = new PositionDal();

        public async Task Save(Study study)
        {
            await dal.Save(study);
            if(study.Position != null)
            {
                await positionDal.Save(study.Position);
            }
        }

        public void Delete(Guid id)
        {
            var study = dal.GetById(id);
            dal.Delete(id);

            positionDal.Delete(study.PositionId.Value);
        }

        public IList<Study> GetStub()
        {
            return dal.GetStudies();
        }

        public Study GetStudyById(Guid id) {
            var study = dal.GetById(id);
            study.Position = positionDal.GetById(study.PositionId.Value,0);
            return study;
        }
    }
}
