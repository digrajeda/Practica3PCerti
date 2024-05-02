using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManager;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ClinicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientFileRepository _patientRepository;
        private List<Patient> patients;

        public PatientsController(IConfiguration configuration)
        {
            _patientRepository = new PatientFileRepository(configuration);
            patients = _patientRepository.LoadPatients();  // Carga los pacientes al iniciar
        }

        // POST: /Patients
        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            var bloodGroups = new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            patient.BloodGroup = bloodGroups[new System.Random().Next(bloodGroups.Length)];  // Assign a random blood group
            patients.Add(patient);
            _patientRepository.SavePatients(patients);  // Guarda los cambios en el archivo
            return CreatedAtAction(nameof(GetByCI), new { ci = patient.CI }, patient);
        }

        // PUT: /Patients/{ci}
        [HttpPut("{ci}")]
        public IActionResult Update(string ci, Patient updatePatient)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patient.Name = updatePatient.Name;
            patient.LastName = updatePatient.LastName;
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
