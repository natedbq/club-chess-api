using AutoMapper;
using chess.api.models;

namespace ChessApi.configuration
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Study, SimpleStudy>()
                .ForMember(dest => dest.SummaryFEN,opt => opt.MapFrom((src) => GetSummaryFEN(src)));
        }

        private string GetSummaryFEN(Study study)
        {
            var farthestPosition = study.Position;
            var keepSearching = true;
            while (keepSearching)
            {
                if(farthestPosition.Positions.Count != 1) {
                    keepSearching = false;
                }
                if(farthestPosition.Positions.Count == 1)
                {
                    farthestPosition = farthestPosition.Positions[0];
                }
            }

            return farthestPosition.Move.FEN;
        }
    }
}
