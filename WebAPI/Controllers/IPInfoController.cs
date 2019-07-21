using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPInfo;
using IPInfo.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Service;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPInfoController : ControllerBase
    {
        private readonly IIPInfoService _ipInfoService;
        private readonly IIPDetailsUpdateBatchService _ipDetailsUpdateService;


        public IPInfoController(IIPInfoService ipInfoService, IIPDetailsUpdateBatchService ipDetailsUpdateService)
        {
            _ipInfoService = ipInfoService;
            _ipDetailsUpdateService = ipDetailsUpdateService;

        }

        // GET api/ipinfo/2.86.114.25
        [HttpGet("{ip}")]
        public async Task<IPDetails> Get(string ip)
        {
            return await _ipInfoService.GetIPDetails(ip);
        }

        [HttpPost("[action]")]
        public async Task<Guid> UpdateIPDetails([FromBody] List<IPDetailsModel> details)
        {
            return await _ipDetailsUpdateService.UpdateIPDetails(details);
        }

        // GET api/ipinfo/job/A35DF5C4-BE1F-4184-A454-6FC4CB3CCCE1
        [HttpGet("job/{id}")]
        public async Task<IActionResult> GetJob(Guid id)
        {
            BatchDetailModel result = await _ipDetailsUpdateService.GetJobProgress(id);
            if (result == null)
                return NotFound("Job not found");
            else
                return Ok(result);

        }
    }
}