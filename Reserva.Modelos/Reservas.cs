using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Reserva.Modelos
{
    public class Reservas
    {
        [Key] public int Id { get; set; }
        public DateTime fecha_reserva {  get; set; }
        [ForeignKey("ClientesId")]
        public int ClientesId { get; set; }
        public Clientes? Clientes { get; set; }
        [ForeignKey("CanchasId")]
        public int CanchasId { get; set; }
        public Canchas? Canchas { get; set; }
        [ForeignKey("HorariosId")]
        public int HorariosId { get; set; }
        public Horarios? Horarios { get; set; } 
    }
}
