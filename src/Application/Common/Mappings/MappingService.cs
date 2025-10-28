using Mapster;
using SGS.TaskTracker.Application.Common.Interfaces;

namespace SGS.TaskTracker.Application.Common.Mappings
{
    public class MappingService : IMappingService
    {
        public TDestination Map<TDestination>(object source)
        {
            return source.Adapt<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return source.Adapt<TSource, TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return source.Adapt(destination);
        }
    }
}
