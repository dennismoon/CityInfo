using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Repositories
{
    public interface ICityInfoRepository : IDisposable
    {
        bool CityExists(int id);
        IEnumerable<City> GetCities();
        IEnumerable<City> GetCitiesPaged(int page, int pageSize, string searchText, string sortField, string sortDirection);
        City GetCity(int id, bool includePointsOfInterest);
        void AddCity(City city);
        void DeleteCity(City city);
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int id);
        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterestForCity(PointOfInterest pointOfInterest);
        bool Save();
    }
}
