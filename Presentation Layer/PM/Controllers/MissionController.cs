using Microsoft.AspNetCore.Mvc;
using PM.Models.missions;
using System.Text;
using System.Text.Json;

namespace PM.Controllers
{
    [Route("")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly ILogger<MissionController> _logger;
        private readonly HttpClient _httpClient;
        public MissionController(ILogger<MissionController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://localhost:7203");
        }
        [HttpGet("/mission/get-missions")]
        public async Task<IActionResult> GetMissions()
        {
            try
            {
                var response = await _httpClient.GetAsync("/mission/get-missions");
                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/project/plan/mission/get-missions-in-plan")]
        public async Task<IActionResult> GetMissionInPlan(string planId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/project/plan/mission/get-missions-in-plan?planId={planId}");
                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("/project/plan/mission/get-detail-mision")]
        public async Task<IActionResult> GetDetailMission(string missionId)
        {
            if (missionId == null) return BadRequest();
            try
            {
                var response = await _httpClient.GetAsync($"/project/plan/mission/get-detail-mision?missionId={missionId}");
                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("/project/plan/mission/add-missison")]
        public async Task<IActionResult> AddMission(AddMissonModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/project/plan/mission/add-missison", model);
                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }
                return BadRequest();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpPut("/project/plan/mission/update-mission")]
        public async Task<IActionResult> UpdateMission(UpdateMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/project/plan/mission/update-mission", model);
                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("/project/plan/mission/delete-mission")]
        public async Task<IActionResult> DeleteMission(DeleteMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {

                //_httpClient.DeleteFromJsonAsync("/project/plan/mission/delete-mission",null,null,null,)
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("/project/plan/mission/delete-mission", UriKind.Relative),
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"),
                };
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
