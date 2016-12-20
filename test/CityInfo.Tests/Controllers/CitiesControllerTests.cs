//using CityInfo.API.Controllers;
//using CityInfo.API.Entities;
//using CityInfo.API.Repositories;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Xunit;

//namespace CityInfo.Tests.Controllers
//{
//    public class CitiesControllerTests
//    {
//        CityInfoContext _context;
//        ICityInfoRepository _repository;

//        public CitiesControllerTests()
//        {
//            //services.AddAutoMapper(cfg =>
//            //{
//            //    cfg.CreateMap<Entities.City, Models.CityDto>();
//            //    cfg.CreateMap<Models.CityDto, Entities.City>();
//            //    cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
//            //    cfg.CreateMap<Models.PointOfInterestDto, Entities.PointOfInterest>();
//            //});
//        }

//        [Fact]
//        public void PassingTest()
//        {
//            _context = CreateAndSeedContext();
//            _repository = new CityInfoRepository(_context);
//            var controller = new CitiesController(null, null, null, _repository);

//            var results = controller.GetCities();

//            Assert.NotNull(results);
//        }

//        private CityInfoContext CreateAndSeedContext()
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<CityInfoContext>();

//            optionsBuilder.UseInMemoryDatabase();

//            var context = new CityInfoContext(optionsBuilder.Options);

//            context.Database.EnsureDeleted();

//            context.Database.EnsureCreated();

//            var cities = GetCityData();

//            context.Cities.AddRange(cities);

//            context.SaveChanges();

//            return context;

//        }

//        private ICollection<City> GetCityData()
//        {
//            // init seed data
//            var cities = new List<City>()
//            {
//                new City()
//                {
//                     Name = "New York City",
//                     Description = "The one with that big park.",
//                     PointsOfInterest = new List<PointOfInterest>()
//                     {
//                         new PointOfInterest() {
//                             Name = "Central Park",
//                             Description = "The most visited urban park in the United States."
//                         },
//                          new PointOfInterest() {
//                             Name = "Empire State Building",
//                             Description = "A 102-story skyscraper located in Midtown Manhattan."
//                          },
//                     }
//                },
//                new City()
//                {
//                    Name = "Antwerp",
//                    Description = "The one with the cathedral that was never really finished.",
//                    PointsOfInterest = new List<PointOfInterest>()
//                     {
//                         new PointOfInterest() {
//                             Name = "Cathedral",
//                             Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
//                         },
//                          new PointOfInterest() {
//                             Name = "Antwerp Central Station",
//                             Description = "The the finest example of railway architecture in Belgium."
//                          },
//                     }
//                },
//                new City()
//                {
//                    Name = "Paris",
//                    Description = "The one with that big tower.",
//                    PointsOfInterest = new List<PointOfInterest>()
//                     {
//                         new PointOfInterest() {
//                             Name = "Eiffel Tower",
//                             Description =  "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
//                         },
//                          new PointOfInterest() {
//                             Name = "The Louvre",
//                             Description = "The world's largest museum."
//                          },
//                     }
//                }
//            };

//            return cities;
//        }
//    }
//}
