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
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    patients.Add(new Patient { Name = parts[0], LastName = parts[1], CI = parts[2], BloodGroup = parts[3] });
                }
            }
            return patients;
        }

        public void SavePatients(IEnumerable<Patient> patients)
        {
            using (var writer = new StreamWriter(_filePath, false))
            {
                foreach (var patient in patients)
                {
                    writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodGroup}");
                }
            }
        }
    }
}
