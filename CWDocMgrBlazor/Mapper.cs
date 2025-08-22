using AutoMapper;
using CWDocMgrBlazor.Models;
using Microsoft.Extensions.Logging;
using SharedLib.Models;

namespace CWDocMgrBlazor
{
    public class Mapper
    {
        public static MapperConfiguration RegisterMaps(ILoggerFactory? loggerFactory)
        {
            var configurationOptions = new MapperConfigurationExpression();

            configurationOptions.CreateMap<Models.DocumentModel, SharedLib.ViewModels.DocumentVM>()
                .ForMember(dest => dest.DocumentDate, opt => opt.MapFrom(src => src.DocumentDate.ToUniversalTime().ToString("yyyy-MM-dd")));
            configurationOptions.CreateMap<SharedLib.ViewModels.DocumentVM, Models.DocumentModel>()
                .ForMember(dest => dest.DocumentDate, opt => opt.MapFrom(src => DateTime.Parse(src.DocumentDate).ToUniversalTime()));
            configurationOptions.CreateMap<DocumentModel, DocumentUploadVM>()
                .ForMember(dest => dest.DateString, opt => opt.MapFrom(src => src.DocumentDate.ToString("yyyy-MM-dd")));

            MapperConfiguration mappingConfig;
            mappingConfig = new MapperConfiguration(configurationOptions, loggerFactory);
            return mappingConfig;
        }
    }
}