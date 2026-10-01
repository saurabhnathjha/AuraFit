using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraFitDataAccessLayer.Models;
using Microsoft.Extensions.Logging;

namespace AuraFitWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseCatalogController : ControllerBase
    {
        private readonly AuraFitDbContext _context;
        private readonly ILogger<ExerciseCatalogController> _logger;

        public ExerciseCatalogController(AuraFitDbContext context, ILogger<ExerciseCatalogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExercises()
        {
            try
            {
                var exercises = await _context.ExerciseCatalog
                    .Select(e => new
                    {
                        e.ExerciseCatalogId,
                        e.Name
                    }).ToListAsync();

                _logger.LogInformation("Retrieved {Count} exercises successfully", exercises.Count);
                return Ok(exercises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving exercises");
                return BadRequest("An error occurred while fetching exercises.");
            }
        }
    }
}
