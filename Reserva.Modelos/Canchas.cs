using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Reserva.Modelos
{
    public class Canchas
    {
        [Key ] public int Id { get; set; }  
        public string  nombre_cancha {  get; set; }
        public string descripcion_cancha { get; set; }
        public bool estado_cancha { get; set; }

        [ForeignKey("Tipo_CanchasId")]
        public int Tipo_CanchasId { get; set; }

        public Tipo_Canchas? Tipo_Canchas { get; set; }

       List<Reservas>? Reservas{ get; set; }    =new List<Reservas>();



    }
}
