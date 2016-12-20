using CityInfo.API.Entities;
using CityInfo.API.Repositories;
using CityInfo.Tests.FakeData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CityInfo.Tests.Repositories
{
    public class CityInfoRepositoryTests
    {
        [Fact]
        public void Get_All_Cities()
        {
            using (var fakeCityInfoData = new CityInfoFakeData())
            {
                var context = fakeCityInfoData.CreateAndSeedContext();

                var repository = new CityInfoRepository(context);

                var cities = repository.GetCities();

                Assert.NotNull(cities);
            }               
        }

        [Fact]
        public void Get_City_Without_Points_Of_Interests()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var city = repository.GetCity(2, false);

            Assert.NotNull(city);
            Assert.Empty(city.PointsOfInterest);
        }

        [Fact]
        public void Get_City_With_Points_Of_Interests()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var city = repository.GetCity(2, true);

            Assert.NotNull(city);
            Assert.NotEmpty(city.PointsOfInterest);
        }

        [Fact]
        public void Get_All_Points_Of_Interest_for_City()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var pointsOfInterest = repository.GetPointsOfInterestForCity(2);

            Assert.NotEmpty(pointsOfInterest);
            Assert.True(pointsOfInterest.Count() > 1);
        }

        [Fact]
        public void Get_Single_Point_Of_Interest_for_City()
        {
            var fakeCityInfoData = new CityInfoFakeData();

            var context = fakeCityInfoData.CreateAndSeedContext();

            var repository = new CityInfoRepository(context);

            var pointOfInterest = repository.GetPointOfInterestForCity(2, 3);

            Assert.NotNull(pointOfInterest);
            Assert.True(pointOfInterest.City.Id == 2);
            Assert.NotNull(pointOfInterest.City);
        }
    }
}
