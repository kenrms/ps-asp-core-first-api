using System.Linq;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poi == null) return NotFound();

            return Ok(poi);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            // demo purpose; to be improved
            var maxId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var newPoi = new PointOfInterestDto
            {
                Id = ++maxId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(newPoi);

            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = newPoi.Id }, newPoi);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poi == null) return NotFound();

            // do the update
            poi.Name = pointOfInterest.Name;
            poi.Description = pointOfInterest.Description;

            // excellent, return a 204
            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poi == null) return NotFound();

            var poiToPatch = new PointOfInterestForUpdateDto { Name = poi.Name, Description = poi.Description };

            patchDoc.ApplyTo(poiToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (poiToPatch.Description == poiToPatch.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            TryValidateModel(poiToPatch);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // do the patch
            poi.Name = poiToPatch.Name;
            poi.Description = poiToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poi == null) return NotFound();

            city.PointsOfInterest.Remove(poi);

            return NoContent();
        }
    }
}
