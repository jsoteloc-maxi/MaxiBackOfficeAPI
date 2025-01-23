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
    public static class CCAgFeeCommResMapper
    {
        public static CCAgFeeCommRes Map(CCAgFeeCommResEntity entity)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CCAgFeeCommResEntity, CCAgFeeCommRes>();
            });

            IMapper iMapper = config.CreateMapper();
            //var source = new CCAgFeeCommResEntity();
            var destination = iMapper.Map<CCAgFeeCommResEntity, CCAgFeeCommRes>(entity);

            return destination;
        }
    }
}
