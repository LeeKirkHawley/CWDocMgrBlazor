using AutoMapper;
using Microsoft.Extensions.Logging;

namespace CWDocMgrBlazor
{
    public class Mapper
    {
        public static MapperConfiguration RegisterMaps(ILoggerFactory? loggerFactory)
        {
            // Create configuration options first
            var configurationOptions = new MapperConfigurationExpression();

            // Configure the mappings
            configurationOptions.CreateMap<Models.DocumentModel, SharedLib.ViewModels.DocumentVM>()
                .ForMember(dest => dest.DocumentDate, opt => opt.MapFrom(src => src.DocumentDate.ToUniversalTime().ToString("yyyy-MM-dd")));
            configurationOptions.CreateMap<SharedLib.ViewModels.DocumentVM, Models.DocumentModel>()
                .ForMember(dest => dest.DocumentDate, opt => opt.MapFrom(src => DateTime.Parse(src.DocumentDate).ToUniversalTime()));

            // Create the configuration with logger (if provided)
            MapperConfiguration mappingConfig;
            mappingConfig = new MapperConfiguration(configurationOptions, loggerFactory);
            return mappingConfig;
        }
    }
}