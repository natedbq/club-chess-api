using AutoMapper;
using chess.api.common;
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



        public IList<SimpleStudy> GetStudies(Guid userId = default(Guid))
        {
            var studies = dal.GetStudies(userId);
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            foreach(var study in studies)
            {
                study.Position = positionDal.GetById(study.PositionId.Value);
            }

            var simps = mapper.Map<IList<SimpleStudy>>(studies);

            foreach(var simp in simps)
            {
                if(simp.Score == -1)
                {
                    simp.Score = StudyAccuracyCache.GetAccuracy(simp.Id);
                    dal.UpdateScore(simp.Id, simp.Score);
                }
            }

            return simps;
        }
    }
}
