using chess.api.models;
using ChessApi.dal;

namespace chess.api.repository
{
    public class StudyRepository
    {
        private StudyDal dal = new StudyDal();

        public void Save(Study study)
        {
            dal.Save(study);
        }



        public IList<Study> GetStub()
        {
            return dal.GetStudies();
        }

        public Study GetStudyById(Guid id) {
            return dal.GetById(id);
        }
    }
}
