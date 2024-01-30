using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IInventoryService inventoryService;

        public ProjectController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpGet("GetBOMBySubJobNumber/{subJobNumber}")]
        public async Task<ActionResult<List<BOM>>> GetBOMBySubJobNumberAsync(int subJobNumber)
        {
            var boms = await inventoryService.GetBOMBySubJobNumberAsync(subJobNumber);
            return Ok(boms);
        }

        [HttpGet("GetJobsByProjectNumber/{projectNumber}")]
        public async Task<ActionResult<List<Job>>> GetJobsByProjectNumberAsync(int projectNumber)
        {
            var jobs = await inventoryService.GetJobsByProjectNumberAsync(projectNumber);
            return Ok(jobs);
        }

        [HttpGet("GetProjectData/{projectNumber}")]
        public async Task<ActionResult<Project>> GetProjectDataAsync(int projectNumber)
        {
            var project = await inventoryService.GetProjectDataAsync(projectNumber);
            return Ok(project);
        }

        [HttpGet("GetSubJobsByJobNumber/{jobNumber}")]
        public async Task<ActionResult<List<SubJob>>> GetSubJobsByJobNumberAsync(int jobNumber)
        {
            var subJobs = await inventoryService.GetSubJobsByJobNumberAsync(jobNumber);
            return Ok(subJobs);
        }

        [HttpGet("SearchByProjectNumber/{projectNumber}")]
        public async Task<ActionResult<Project>> SearchByProjectNumberAsync(int projectNumber)
        {
            var project = await inventoryService.SearchByProjectNumberAsync(projectNumber);
            return Ok(project);
        }
    }
}
 
    
