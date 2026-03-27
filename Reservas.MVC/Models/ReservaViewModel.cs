using System;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Modelos
{
    public class Reservas
    {
        public int Id { get; set; }
        public DateTime fecha_reserva { get; set; }
        public int ClientesId { get; set; }
        public int CanchasId { get; set; }
        public int HorariosId { get; set; }

        // CAMBIO AQUÍ: Asegúrate de que coincidan con lo que devuelve tu API
        public virtual Clientes? Clientes { get; set; }
        public virtual Canchas? Canchas { get; set; }
        public virtual Horarios? Horarios { get; set; }
    }
}