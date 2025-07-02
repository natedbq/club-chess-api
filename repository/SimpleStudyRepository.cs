using AutoMapper;
using chess.api.dal;
using chess.api.models;
using ChessApi.configuration;
using ChessApi.dal;

namespace ChessApi.repository
{
    public class SimpleStudyRepository
    {
        private StudyDal dal = new StudyDal();
        private PositionDal positionDal = new PositionDal();

        public IList<SimpleStudy> GetStudies()
        {
            var studies = dal.GetStudies();
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            foreach(var study in studies)
            {
                study.Position = positionDal.GetById(study.PositionId.Value);
            }

            return mapper.Map<IList<SimpleStudy>>(studies);
        }
    }
}
