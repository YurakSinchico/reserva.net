using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reserva.Modelos
{
    public class Tipo_Canchas
    {
        [Key ] public int Id { get; set; }
        public string nombre_tip_cancha {  get; set; }

        List<Canchas>?Canchas { get; set; }= new List<Canchas>();

    }
}
