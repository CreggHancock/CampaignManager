using AutoMapper;
using DndManager.Helpers;

namespace DndManager.Tests
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