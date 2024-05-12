using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PatientManager;

namespace ClinicAPI
{
    public class PatientFileRepository
    {
        private readonly string _filePath;

        public PatientFileRepository(IConfiguration configuration)
        {
            _filePath = configuration.GetValue<string>("PatientDataFile");
            // Verifica que el directorio exista, y si no, créalo
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            // Asegúrate de que el archivo existe
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close();
            }
        }

        public List<Patient> LoadPatients()
        {
            var patients = new List<Patient>();
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            patients.Add(new Patient
                            {
                                Name = parts[0],
                                LastName = parts[1],
                                CI = parts[2],
                                BloodGroup = parts[3],
                                PatientCode = parts[4]  // Cargar el PatientCode
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Failed to load patients", ex);
            }
            return patients;
        }

        public void SavePatients(IEnumerable<Patient> patients)
        {
            using (var writer = new StreamWriter(_filePath, false))
            {
                foreach (var patient in patients)
                {
                    writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodGroup},{patient.PatientCode}");
                }
            }
        }
    }
}
