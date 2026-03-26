using System;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Modelos
{
    public class Reservas
    {
        public int Id { get; set; }

        // Coincide con pgAdmin: fecha_reserva
        public DateTime fecha_reserva { get; set; }

        // Coincide con pgAdmin: ClientesId
        public int ClientesId { get; set; }

        // Coincide con pgAdmin: CanchasId
        public int CanchasId { get; set; }

        // Coincide con pgAdmin: HorariosId
        public int HorariosId { get; set; }

        // Objetos de navegación opcionales
        public Clientes? Clientes { get; set; }
        public Canchas? Canchas { get; set; }
        public Horarios? Horarios { get; set; }
    }
}