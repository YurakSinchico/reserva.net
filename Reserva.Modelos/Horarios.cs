using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reserva.Modelos
{
    public class Horarios
    {
        [Key] public int Id { get; set; }
        public TimeOnly hora_inicio {  get; set; }
        public TimeOnly hora_fin {  get; set; }
        public string dia {  get; set; }
        List<Reservas>? Reservas { get; set; } = new List<Reservas>();
    }
}
