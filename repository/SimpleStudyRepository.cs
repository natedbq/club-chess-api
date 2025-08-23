using AutoMapper;
using chess.api.common;
using chess.api.dal;
using chess.api.models;
using ChessApi.configuration;
using ChessApi.dal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ChessApi.repository
{
    public class SimpleStudyRepository
    {
        private StudyDal dal = new StudyDal();
        private PositionDal positionDal = new PositionDal();



        public async Task<IList<SimpleStudy>> GetStudies(Guid userId)
        {
            var studies = dal.GetStudiesByUserId(userId);
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            foreach(var study in studies)
            {
                study.Position = positionDal.GetById(study.PositionId.Value);
            }

            var simps = mapper.Map<IList<SimpleStudy>>(studies);

            if(userId != default(Guid))
            {
                foreach (var simp in simps)
                {
                    if (simp.Score == -1)
                    {
                        simp.Score = await StudyAccuracyCache.GetAccuracy(simp.Id, userId);
                        dal.UpdateScore(simp.Id, userId, simp.Score.Value);
                    }
                }
            }

            return simps;
        }


        public IList<SimpleStudy> GetStudiesByClubId(Guid clubId, [FromQuery] Guid userId = default(Guid))
        {
            var studies = dal.GetStudiesByClubId(clubId, userId);
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            foreach (var study in studies)
            {
                study.Position = positionDal.GetById(study.PositionId.Value);
            }

            var simps = mapper.Map<IList<SimpleStudy>>(studies);

            return simps;
        }
    }
}
