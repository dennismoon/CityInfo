﻿using AutoMapper;
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
    public class CitiesControllerTests
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

        private CitiesController CreateSutWithDependencies()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var mapper = InitializeAutomapper();

            var controller = new CitiesController(mapper, null, null, repository);

            return controller;
        }

        [Fact]
        public void Get_All_Cities()
        {
            var controller = CreateSutWithDependencies();

            var result = controller.GetCities();

            Assert.NotNull(result);

            var cities = ((OkObjectResult)result).Value as ICollection<CityDto>;

            Assert.NotEmpty(cities);
        }

        [Fact]
        public void Get_City_Without_Points_Of_Interests()
        {
            var controller = CreateSutWithDependencies();

            var result = controller.GetCity(2, false);

            Assert.NotNull(result);

            var city = ((OkObjectResult)result).Value as CityDto;

            Assert.NotNull(city);
            Assert.Empty(city.PointsOfInterest);
        }

        [Fact]
        public void Get_City_With_Points_Of_Interests()
        {
            var controller = CreateSutWithDependencies();

            var result = controller.GetCity(2, true);

            Assert.NotNull(result);

            var city = ((OkObjectResult)result).Value as CityDto;

            Assert.NotNull(city);
            Assert.NotEmpty(city.PointsOfInterest);
        }
    }
}
