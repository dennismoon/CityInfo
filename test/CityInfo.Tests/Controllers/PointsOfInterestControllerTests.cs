using AutoMapper;
using CityInfo.API.Controllers;
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.Tests.FakeData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CityInfo.Tests.Controllers
{
    public class PointsOfInterestControllerTests
    {
        private IMapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<API.Entities.City, API.Models.CityDto>().MaxDepth(1);
                cfg.CreateMap<API.Models.CityDto, API.Entities.City>().MaxDepth(1);
                cfg.CreateMap<API.Entities.PointOfInterest, API.Models.PointOfInterestDto>().MaxDepth(1);
                cfg.CreateMap<API.Models.PointOfInterestDto, API.Entities.PointOfInterest>().MaxDepth(1);
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        private PointsOfInterestController CreateSutWithDependencies()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var mapper = InitializeAutomapper();

            var controller = new PointsOfInterestController(mapper, null, null, repository);

            return controller;
        }

        [Fact]
        public void Get_All_Points_Of_Interest_for_City()
        {
            var controller = CreateSutWithDependencies();

            var result = controller.GetPointsOfInterest(2);

            Assert.NotNull(result);

            var pointsOfInterest = ((OkObjectResult)result).Value as IEnumerable<PointOfInterestDto>;

            Assert.NotEmpty(pointsOfInterest);
            Assert.True(pointsOfInterest.Count() > 1);
        }

        [Fact]
        public void Get_Single_Point_Of_Interest_for_City()
        {
            var controller = CreateSutWithDependencies();

            var result = controller.GetPointOfInterest(2, 3);

            Assert.NotNull(result);

            var pointOfInterest = ((OkObjectResult)result).Value as PointOfInterestDto;

            Assert.NotNull(pointOfInterest);
            Assert.True(pointOfInterest.City.Id == 2);
            Assert.NotNull(pointOfInterest.City);
        }
    }
}
