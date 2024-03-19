using AutoMapper;
using CampaignManager.Helpers;

namespace CampaignManager.Tests
{
    [TestClass]
    public class AutoMapperTests
    {
        [TestMethod]
        public void AutoMapperConfigurationIsValid()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = new Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}