using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManager;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ClinicAPI;

namespace ClinicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientFileRepository _patientRepository;
        private readonly PatientCodeService _patientCodeService;
        private List<Patient> patients;

        // Inyectar PatientCodeService en el constructor
        public PatientsController(IConfiguration configuration, PatientCodeService patientCodeService)
        {
            _patientRepository = new PatientFileRepository(configuration);
            _patientCodeService = patientCodeService;
            patients = _patientRepository.LoadPatients();  // Carga los pacientes al iniciar
        }

        // POST: /Patients
        [HttpPost]
        public async Task<IActionResult> Create(Patient patient) // Haz que el método sea asíncrono
        {
            var bloodGroups = new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            patient.BloodGroup = bloodGroups[new System.Random().Next(bloodGroups.Length)];  // Asigna un grupo sanguíneo aleatorio

            // Generar y asignar código de paciente
            try
            {
                var patientInfo = new PatientInfo { Name = patient.Name, LastName = patient.LastName, CI = patient.CI };
                patient.PatientCode = await _patientCodeService.GetPatientCodeAsync(patientInfo);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error generating patient code: {ex.Message}");
            }

            patients.Add(patient);
            _patientRepository.SavePatients(patients);  // Guarda los cambios en el archivo
            return CreatedAtAction(nameof(GetByCI), new { ci = patient.CI }, patient);
        }

        // PUT: /Patients/{ci}
        [HttpPut("{ci}")]
        public async Task<IActionResult> Update(string ci, Patient updatePatient)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }

            bool needNewCode = patient.Name != updatePatient.Name || patient.LastName != updatePatient.LastName;
            patient.Name = updatePatient.Name;
            patient.LastName = updatePatient.LastName;

            if (needNewCode)  // Si se decide que el cambio de nombre o apellido requiere nuevo código
            {
                try
                {
                    var patientInfo = new PatientInfo { Name = patient.Name, LastName = patient.LastName, CI = patient.CI };
                    patient.PatientCode = await _patientCodeService.GetPatientCodeAsync(patientInfo);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error updating patient code: {ex.Message}");
                }
            }

            _patientRepository.SavePatients(patients);  // Guarda los cambios en el archivo
            return Ok(patient);
        }

        // DELETE: /Patients/{ci}
        [HttpDelete("{ci}")]
        public IActionResult Delete(string ci)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patients.Remove(patient);
            _patientRepository.SavePatients(patients);  // Guarda los cambios en el archivo
            return NoContent();
        }

        // GET: /Patients
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(patients);
        }

        // GET: /Patients/{ci}
        [HttpGet("{ci}")]
        public IActionResult GetByCI(string ci)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            return Ok(patient);
        }
    }
}
