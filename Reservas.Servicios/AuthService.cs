using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using API_Consumer;
using Reserva.Modelos;
using Reservas.Servicios.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.JsonPatch.Internal;


namespace Reservas.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor=httpContextAccessor;
        }

        public async Task<bool>Login (string username,string password)

        {
            username=username.Trim().ToLower();
            password = password.Trim();//Añadimos para la contraseña
            var usuarios = Crud<Clientes>.GetAll();

            foreach (var usuario in usuarios) { 
                if (usuario.correo_cliente != null &&
                     usuario.correo_cliente.ToLower() == username)//correcion
                {

                    //BCrypt compara el texto plano con el Hash de la base de datos
                    if (BCrypt.Net.BCrypt.Verify(password, usuario.contrasena_cliente))
                    {
                        var datosUsuario = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,usuario.nombre_cliente),
                            new Claim(ClaimTypes.Email,usuario.correo_cliente),
                            new Claim(ClaimTypes.NameIdentifier,usuario.Id.ToString()),

                        };

                        var credencialDigital = new ClaimsIdentity(datosUsuario, "Cookies");
                        var usuarioAutenticado=new ClaimsPrincipal(credencialDigital);

                        await _httpContextAccessor.HttpContext.SignInAsync("Cookies",usuarioAutenticado);
                        return true;

                            
                    }
                    return false;   
                }
            }
            return false;
        }



        public async Task<bool> Register(
            string nombre_cliente,
            string apellido_cliente,
            string correo_cliente,
            string contrasena_cliente,
            string telefono_cliente,
            DateOnly fecha_nacimiento_cliente)
        {
            //Añandimos para la contraseña
            correo_cliente=correo_cliente.Trim().ToLower();
            //Verificamos duplicados con endpoints específicos
            var usuarioExistente = Crud<Clientes>.GetAll()
                 .FirstOrDefault(u => u.correo_cliente == correo_cliente);

            if (usuarioExistente != null)
            {
                Console.WriteLine("Error: El correo ya está registrado.");
                return false;
            }

            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(contrasena_cliente);
                //CREACIÓN DEL OBJETO USUARIO CON HASH SEGURIDAD
                var nuevoUsuario = new Clientes
                {
                    Id = 0,
                    nombre_cliente = nombre_cliente,
                    apellido_cliente = apellido_cliente,
                    correo_cliente = correo_cliente,
                    contrasena_cliente = contrasena_cliente,
                    telefono_cliente = telefono_cliente,
                    fecha_nacimiento_cliente = fecha_nacimiento_cliente,

                };

                Crud<Clientes>.Create(nuevoUsuario);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return false;
            }
        }
    }

}
    

