using AutoMapper;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Infrastructure.Mappings
{
    public class CC_GetMakerByAccResMapper
    {
        public static List<CC_GetMakerByAccEntity> Map(List<CC_GetMakerByAccRes> dto)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CC_GetMakerByAccRes, CC_GetMakerByAccEntity>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<CC_GetMakerByAccRes>, List<CC_GetMakerByAccEntity>>(dto);

            return destination;
        }

        public static List<CC_GetMakerByAccRes> Map(List<CC_GetMakerByAccEntity> dto)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CC_GetMakerByAccEntity, CC_GetMakerByAccRes>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<CC_GetMakerByAccEntity>, List<CC_GetMakerByAccRes>>(dto);

            return destination;
        }

        public static CC_GetMakerByAccRes Map(CC_GetMakerByAccEntity dto)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CC_GetMakerByAccEntity, CC_GetMakerByAccRes>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<CC_GetMakerByAccEntity, CC_GetMakerByAccRes>(dto);

            return destination;
        }
    }
}
