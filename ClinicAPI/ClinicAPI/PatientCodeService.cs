using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Asegúrate de incluir el espacio de nombres para logging

namespace ClinicAPI
{
    public class PatientCodeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<PatientCodeService> _logger; // Agregar ILogger

        public PatientCodeService(HttpClient httpClient, IConfiguration configuration, ILogger<PatientCodeService> logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetValue<string>("Practice3ApiBaseUrl");
            _logger = logger;
        }

        public async Task<string> GetPatientCodeAsync(PatientInfo patientInfo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/PatientCode", patientInfo);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PatientCodeResult>();
                    return result.PatientCode;
                }
                else
                {
                    _logger.LogError($"Failed to retrieve patient code: {response.StatusCode}");
                    throw new HttpRequestException($"Server responded with error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred while contacting the patient code service: {ex.Message}");
                throw new Exception("Error contacting the patient code service.", ex);
            }
        }
    }

    public class PatientCodeResult
    {
        public string PatientCode { get; set; }
    }

    public class PatientInfo
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
    }
}
