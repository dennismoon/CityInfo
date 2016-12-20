using AutoMapper;
using CityInfo.API.Controllers;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.Tests.FakeData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace CityInfo.Tests.Controllers
{
    public class CitiesControllerTests
    {
        IMapper _mapper;

        public CitiesControllerTests()
        {
            _mapper = InitializeAutomapper();
        }

        private IMapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<API.Entities.City, API.Models.CityDto>();
                cfg.CreateMap<API.Models.CityDto, API.Entities.City>();
                cfg.CreateMap<API.Entities.PointOfInterest, API.Models.PointOfInterestDto>();
                cfg.CreateMap<API.Models.PointOfInterestDto, API.Entities.PointOfInterest>();
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        [Fact]
        public void Get_All_Cities()
        {
            using (var fakeCityInfoData = new CityInfoFakeData())
            {
                var context = fakeCityInfoData.CreateAndSeedContext();

                var repository = new CityInfoRepository(context);

                var controller = new CitiesController(_mapper, null, null, repository);

                var result = controller.GetCities();

                Assert.NotNull(result);

                var cities = ((OkObjectResult)result).Value as ICollection<CityDto>;

                Assert.NotEmpty(cities);
            }
        }

        [Fact]
        public void Get_City_Without_Points_Of_Interests()
        {
            using (var fakeCityInfoData = new CityInfoFakeData())
            {
                var context = fakeCityInfoData.CreateAndSeedContext();

                var repository = new CityInfoRepository(context);

                var controller = new CitiesController(_mapper, null, null, repository);

                var result = controller.GetCity(2, false);

                Assert.NotNull(result);

                var city = ((OkObjectResult)result).Value as CityDto;

                Assert.NotNull(city);
                Assert.Empty(city.PointsOfInterest);
            }
        }

        [Fact]
        public void Get_City_With_Points_Of_Interests()
        {
            using (var fakeCityInfoData = new CityInfoFakeData())
            {
                var context = fakeCityInfoData.CreateAndSeedContext();

                var repository = new CityInfoRepository(context);

                var controller = new CitiesController(_mapper, null, null, repository);

                var result = controller.GetCity(2, true);

                Assert.NotNull(result);

                var city = ((OkObjectResult)result).Value as CityDto;

                Assert.NotNull(city);
                Assert.NotEmpty(city.PointsOfInterest);
            }
        }

        [Fact]
        public void Get_All_Points_Of_Interest_for_City()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var controller = new PointsOfInterestController(_mapper, null, null, repository);

            var result = controller.GetPointsOfInterest(2);

            Assert.NotNull(result);

            var pointsOfInterest = ((OkObjectResult)result).Value as IEnumerable<PointOfInterestDto>;

            Assert.NotEmpty(pointsOfInterest);
            Assert.True(pointsOfInterest.Count() > 1);
        }

        [Fact]
        public void Get_Single_Point_Of_Interest_for_City()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var controller = new PointsOfInterestController(_mapper, null, null, repository);

            var result = controller.GetPointOfInterest(2, 3);

            Assert.NotNull(result);

            var pointOfInterest = ((OkObjectResult)result).Value as PointOfInterestDto;

            Assert.NotNull(pointOfInterest);
            //Assert.True(pointOfInterest.City.Id == 2);
            //Assert.NotNull(pointOfInterest.City);
        }
    }
}
