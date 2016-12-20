using CityInfo.API.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CityInfo.Tests.FakeData
{
    public class CityInfoFakeData : IDisposable
    {
        CityInfoContext _context;
        SqliteConnection _connection;

        //public CityInfoContext CreateAndSeedContext()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<CityInfoContext>();

        //    optionsBuilder.UseInMemoryDatabase();

        //    var context = new CityInfoContext(optionsBuilder.Options);

        //    context.Database.EnsureDeleted();

        //    context.Database.EnsureCreated();

        //    var cities = GetCityData();

        //    context.Cities.AddRange(cities);

        //    context.SaveChanges();

        //    return context;
        //}

        public CityInfoContext CreateAndSeedContext()
        {
            _connection = new SqliteConnection("DataSource=:memory:");

            _connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<CityInfoContext>();

            optionsBuilder.UseSqlite(_connection);

            _context = new CityInfoContext(optionsBuilder.Options);

            _context.IsInMemoryDb = true;

            _context.Database.EnsureDeleted();

            _context.Database.EnsureCreated();

            var cities = GetCityData();

            _context.Cities.AddRange(cities);

            _context.SaveChanges();

            return _context;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }

            if (_context != null)
            {
                _context.Dispose();
            }
        }

        private ICollection<City> GetCityData()
        {
            // init seed data
            var cities = new List<City>()
            {
                new City()
                {
                     Name = "New York City",
                     Description = "The one with that big park.",
                     PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Central Park",
                             Description = "The most visited urban park in the United States."
                         },
                          new PointOfInterest() {
                             Name = "Empire State Building",
                             Description = "A 102-story skyscraper located in Midtown Manhattan."
                          },
                     }
                },
                new City()
                {
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Cathedral",
                             Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
                         },
                          new PointOfInterest() {
                             Name = "Antwerp Central Station",
                             Description = "The the finest example of railway architecture in Belgium."
                          },
                     }
                },
                new City()
                {
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Eiffel Tower",
                             Description =  "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                         },
                          new PointOfInterest() {
                             Name = "The Louvre",
                             Description = "The world's largest museum."
                          },
                     }
                }
            };

            return cities;
        }
    }
}
