using chess.api.dal;
using chess.api.models;
using ChessApi.dal;

namespace chess.api.repository
{
    public class StudyRepository
    {
        private static StudyDal dal = new StudyDal();
        private static PositionDal positionDal = new PositionDal();

        public async Task Save(Study study)
        {
            await dal.Save(study);
            if(study.Position != null)
            {
                await positionDal.Save(study.Position,study.Owner.Id);
            }
        }

        public async Task Delete(Guid id)
        {
            var study = await dal.GetById(id);
            dal.Delete(id);

            positionDal.Delete(study.PositionId.Value);
        }

        public async Task<Study> GetStudyById(Guid id, Guid userId = default(Guid)) {
            var study = await dal.GetById(id, userId);
            study.Position = positionDal.GetById(study.PositionId.Value,userId,0);

            return study;
        }

        public void Study(Guid studyId, Guid userId)
        {
            dal.Study(studyId, userId);
        }
    }
}
