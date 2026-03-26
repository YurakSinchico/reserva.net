using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Modelos
{
    public class Horarios
    {
        [Key]
        public int Id { get; set; }

        // Estos deben coincidir con los nombres de columna en tu BD
        public TimeOnly hora_inicio { get; set; }
        public TimeOnly hora_fin { get; set; }
        public string dia { get; set; }

        // La lista de reservas puede ser nula para no causar errores al crear
        public List<Reservas>? Reservas { get; set; } = new List<Reservas>();
    }
}

