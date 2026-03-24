using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Servicios.Interfaces
{
    public interface IAuthService
    {
        Task<bool>Login (string username, string password);

        Task<bool> Register(
            string nombre_cliente,
            string apellido_cliente,
            string correo_cliente,
            string contrasena_cliente,
            string telefono_cliente,
            DateOnly fecha_nacimiento_cliente
            );
    }
}
