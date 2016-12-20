using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace CityInfo.API.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public void AddCity(City city)
        {
            _context.Cities.Add(city);
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);

            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int id)
        {
            var cityExists = _context.Cities.Any(c => c.Id == id);

            return cityExists;
        }

        public void DeleteCity(City city)
        {
            _context.Cities.Remove(city);
        }

        public void DeletePointOfInterestForCity(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public IEnumerable<City> GetCities()
        {
            var cities = _context.Cities.OrderBy(o => o.Name).ToList();

            return cities;
        }

        public City GetCity(int id, bool includePointsOfInterest)
        {
            City city = null;

            if (includePointsOfInterest)
            {
                city = _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == id).FirstOrDefault();
            }
            else
            {
                city = _context.Cities.Where(c => c.Id == id).FirstOrDefault();

                if (city != null)
                {
                    city.PointsOfInterest = new List<PointOfInterest>();
                }
            }

            return city;
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int id)
        {
            var pointOfInterest = _context.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == id).FirstOrDefault();

            return pointOfInterest;
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            var pointsOfInterest = _context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();

            return pointsOfInterest;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public IEnumerable<City> GetCitiesPaged(int page, int pageSize, string searchText, string sortField, string sortDirection)
        {
            if (pageSize <= 0)
            {
                pageSize = 1;
            }
            else if (page > 200)
            {
                pageSize = 200;
            }

            page = page <= 0 ? 1 : page;

            var cities = _context.Cities.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                cities = cities.Where(s => s.Name.Contains(searchText) || s.Description.Contains(searchText));
            }

            sortDirection = !string.IsNullOrWhiteSpace(sortDirection) ? sortDirection : "asc";

            if (!string.IsNullOrWhiteSpace(sortField))
            {
                cities = cities.OrderBy(sortField + " " + sortDirection);
            }
            else
            {
                cities = cities.OrderBy("Name asc");
            }

            PaginatedList<City> results = PaginatedList<City>.Create(cities.AsNoTracking(), page, pageSize);        

            return results.ToList();
        }
    }
}
