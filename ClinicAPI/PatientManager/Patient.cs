using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientManager
{
    public class Patient
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; } // Cédula de Identidad como identificador único
        public string BloodGroup { get; set; } // Grupo sanguíneo asignado aleatoriamente
    }
}