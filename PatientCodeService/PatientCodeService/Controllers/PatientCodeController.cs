using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Importar ILogger para logging
using System;

namespace PatientCodeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientCodeController : ControllerBase
    {
        private readonly ILogger<PatientCodeController> _logger;

        // Inyectar ILogger en el constructor para habilitar logging
        public PatientCodeController(ILogger<PatientCodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PatientInfo patientInfo)
        {
            try
            {
                if (patientInfo == null)
                {
                    _logger.LogError("Received a null object as patient information");
                    return BadRequest("Patient information cannot be null");
                }

                if (string.IsNullOrEmpty(patientInfo.Name) || string.IsNullOrEmpty(patientInfo.LastName) || string.IsNullOrEmpty(patientInfo.CI))
                {
                    _logger.LogError("Invalid patient information provided");
                    return BadRequest("Invalid patient information");
                }

                var patientCode = $"{patientInfo.Name[0]}{patientInfo.LastName[0]}-{patientInfo.CI}";
                _logger.LogInformation($"Generated patient code: {patientCode}");
                return Ok(new { PatientCode = patientCode });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while generating patient code: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }
    }

    public class PatientInfo
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
    }
}
