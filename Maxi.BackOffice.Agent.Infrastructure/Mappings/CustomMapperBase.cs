using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;



namespace Maxi.BackOffice.Agent.Infrastructure.Mappings
{
    public class CustomMapperBase<T1, T2>
        where T1 : class
        where T2 : class
    {
        //DE T1 => T2
        public static T2 Map(T1 dto)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<T1, T2>(); });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<T1, T2>(dto);

            return destination;
        }

        //De List T1  => List T2
        public static List<T2> Map(List<T1> dto)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<T1, T2>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<T1>, List<T2>>(dto);

            return destination;
        }


        //DE T2 => T1
        public static T1 Map(T2 dto)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<T2, T1>(); });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<T2, T1>(dto);

            return destination;
        }

        //De List T2  => List T1
        public static List<T1> Map(List<T2> dto)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<T2, T1>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<T2>, List<T1>>(dto);

            return destination;
        }

    }


    public static class CustomMapper
    {
        public static T Map<T>(Object dto)
            where T : class
        {
            //var config = new MapperConfiguration(cfg => { });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<string, string>().ConvertUsing(s => s ?? string.Empty);
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<T>(dto);
        }

        //De T1  => T2
        public static T2 Map<T1,T2>(T1 dto)
            where T1 : class
            where T2 : class
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<T1, T2>(); });
            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<T1, T2>(dto);
            return destination;
        }

        //De List T1  => List T2
        public static List<T2> Map<T1,T2>(List<T1> dto)
            where T1 : class
            where T2 : class
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<T1, T2>(); });
            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<T1>, List<T2>>(dto);
            return destination;
        }

    }


}
