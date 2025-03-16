using AutoMapper;
using chess.api.models;

namespace ChessApi.configuration
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Study, SimpleStudy>();
        }
    }
}
