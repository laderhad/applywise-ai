using ApplyWise.Api.Mappings;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ApplyWise.Api.Tests.Mappings;

public sealed class MappingProfileTests
{
    [Fact]
    public void ConfigurationIsValid()
    {
        var configuration = new MapperConfiguration(
            options => options.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }
}
