using AutoMapper;
using chess.api.models;
using ChessApi.configuration;
using ChessApi.dal;

namespace ChessApi.repository
{
    public class SimpleStudyRepository
    {
        private StudyDal dal = new StudyDal();

        public IList<SimpleStudy> GetStub()
        {
            var studies = dal.GetStudies();
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            return mapper.Map<IList<SimpleStudy>>(studies);
        }
    }
}
