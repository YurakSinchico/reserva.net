using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reserva.Modelos
{
    public class Clientes
    {
        [Key] public int Id { get; set; }
        public string nombre_cliente { get; set; }
        public string apellido_cliente { get; set; }
        public string correo_cliente { get; set; }
        public string telefono_cliente { get; set; }
        public DateOnly fecha_nacimiento_cliente { get; set; }

        List<Reservas>? Reservas { get; set; } = new List<Reservas>();


    }
}
