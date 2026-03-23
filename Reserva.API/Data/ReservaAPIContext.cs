using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reserva.Modelos;

namespace Reserva.API.Data
{
    public class ReservaAPIContext : DbContext
    {
        public ReservaAPIContext (DbContextOptions<ReservaAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Reserva.Modelos.Clientes> Clientes { get; set; } = default!;
        public DbSet<Reserva.Modelos.Canchas> Canchas { get; set; } = default!;
        public DbSet<Reserva.Modelos.Horarios> Horarios { get; set; } = default!;
        public DbSet<Reserva.Modelos.Reservas> Reservas { get; set; } = default!;
        public DbSet<Reserva.Modelos.Tipo_Canchas> Tipo_Canchas { get; set; } = default!;
    }
}
