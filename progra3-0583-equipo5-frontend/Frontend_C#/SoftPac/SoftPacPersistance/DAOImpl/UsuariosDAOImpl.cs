using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class UsuariosDAOImpl : UsuariosDAO
    {
        private static BindingList<UsuariosDTO> usuarios = new BindingList<UsuariosDTO>();


        static UsuariosDAOImpl()
        {
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 1,
                Nombre = "Juan Perez",
                NombreDeUsuario = "jperez",
                CorreoElectronico = "juan@email.com",
                PasswordHash = "12345", // ¡NO HACER ESTO EN PRODUCCIÓN! Solo para prueba.
                Activo = true,
                Superusuario = true,
                UsuarioEliminacion = null,
                FechaEliminacion = null,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> { /* ... */ }
            });
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 2,
                Nombre = "Ana Gomez",
                NombreDeUsuario = "agomez",
                CorreoElectronico = "ana@email.com",
                PasswordHash = "12345", // ¡NO HACER ESTO EN PRODUCCIÓN! Solo para prueba.
                Activo = true,
                Superusuario = false,
                UsuarioEliminacion = null,
                FechaEliminacion = null,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> { /* ... */ }
            });
            // (El contenido del constructor anterior va aquí)
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 1,
                Nombre = "Juan Perez",
                NombreDeUsuario = "jperez",
                Apellidos = "García",
                CorreoElectronico = "juan@email.com",
                PasswordHash = "12345",
                Activo = true,
                Superusuario = true,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> {
                new UsuarioPaisAccesoDTO { Pais = new PaisesDTO { PaisId = 1 }, Acceso = true },
                new UsuarioPaisAccesoDTO { Pais = new PaisesDTO { PaisId = 3 }, Acceso = true }
            }
            });
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 2,
                Nombre = "Ana Gomez",
                NombreDeUsuario = "agomez",
                Apellidos = "Lopez",
                CorreoElectronico = "ana@email.com",
                PasswordHash = "12345",
                Activo = true,
                Superusuario = false,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> {
                new UsuarioPaisAccesoDTO { Pais = new PaisesDTO { PaisId = 2 }, Acceso = true }
            }
            });
        }
        public int Insertar(UsuariosDTO usuario)
        {
            // Simulamos un autoincremento
            int nuevoId = (usuarios.Any() ? usuarios.Max(u => u.UsuarioId.Value) : 0) + 1;
            usuario.UsuarioId = nuevoId;
            usuarios.Add(usuario);
            return 1;
        }

        public int Modificar(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null) return 0;

            // Reemplazamos el objeto existente con el modificado
            int idx = usuarios.IndexOf(existing);
            usuarios[idx] = usuario;
            return 1;
        }

        public int eliminarLogico(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null) return 0;

            existing.FechaEliminacion = DateTime.Now;
            existing.UsuarioEliminacion = usuario.UsuarioEliminacion;
            existing.Activo = false; // Importante desactivar el usuario
            return 1;
        }

        public IList<UsuariosDTO> ListarTodos()
        {
            // Devolvemos solo los que no han sido eliminados lógicamente
            return usuarios.Where(u => u.FechaEliminacion == null).ToList();
        }

        public UsuariosDTO ObtenerPorId(int usuarioId)
        {
            return usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
        }


        public UsuariosDTO ObtenerPorCorreo(String correo)
        {
            return usuarios.FirstOrDefault(u => u.CorreoElectronico == correo);
        }

        public int eliminar(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null)
                return 0;
            usuarios.Remove(existing);
            return 1;
        }


        // --- MÉTODO NUEVO ---
        public UsuariosDTO ObtenerPorNombreUsuario(string nombreUsuario)
        {
            return usuarios.FirstOrDefault(u => u.NombreDeUsuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) && u.Activo);
        }
    }
}
