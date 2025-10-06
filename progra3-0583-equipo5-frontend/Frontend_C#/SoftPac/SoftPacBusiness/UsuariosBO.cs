using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class UsuariosBO
    {
        private UsuariosDAO usuariosDAO;
        private UsuarioPaisAccesoDAO usuarioPaisAccesoDAO;

        public UsuariosBO()
        {
            this.usuariosDAO = new UsuariosDAOImpl();
            this.usuarioPaisAccesoDAO = new UsuarioPaisAccesoDAOImpl();
        }

        // --- MÉTODO NUEVO PARA AUTENTICAR ---
        public UsuariosDTO AutenticarUsuario(string nombreUsuario, string password)
        {
            // 1. Buscamos al usuario por su nombre de usuario
            UsuariosDTO usuario = this.usuariosDAO.ObtenerPorNombreUsuario(nombreUsuario);

            if (usuario == null)
            {
                return null; // Usuario no encontrado
            }

            // 2. Verificamos la contraseña
            // !! ADVERTENCIA DE SEGURIDAD !!
            // Esto es solo para la demostración con datos hardcodeados.
            // En un sistema real, NUNCA guardes contraseñas en texto plano.
            // Deberías usar una librería como BCrypt.Net para comparar el hash.
            // Ejemplo real: bool esValido = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);

            bool esPasswordValido = (password == usuario.PasswordHash);

            if (esPasswordValido)
            {
                return usuario; // Autenticación exitosa
            }

            return null; // Contraseña incorrecta
        }

        // Aquí puedes añadir el resto de los métodos que necesites (Insertar, Modificar, etc.)
        public IList<UsuariosDTO> ListarTodos()
        {
            return usuariosDAO.ListarTodos();
        }

        public UsuariosDTO ObtenerPorId(int usuarioId)
        {
            return usuariosDAO.ObtenerPorId(usuarioId);
        }

        public int InsertarUsuario(UsuariosDTO nuevoUsuario)
        {
            if (string.IsNullOrWhiteSpace(nuevoUsuario.PasswordHash))
            {
                throw new Exception("La contraseña es obligatoria para nuevos usuarios.");
            }
            // Simulación de Hashing. En un proyecto real, usar BCrypt.Net.HashPassword(nuevoUsuario.PasswordHash);
            nuevoUsuario.PasswordHash = nuevoUsuario.PasswordHash;

            return usuariosDAO.Insertar(nuevoUsuario);
        }

        public int ModificarAccesoUsuario(int usuarioId, string nuevoNombreUsuario, bool activo, List<int> paisesIds)
        {
            UsuariosDTO usuario = usuariosDAO.ObtenerPorId(usuarioId);
            if (usuario == null) return 0;

            usuario.NombreDeUsuario = nuevoNombreUsuario;
            usuario.Activo = activo;

            // Actualizar la lista de países de acceso
            usuario.UsuarioPais.Clear();
            foreach (var paisId in paisesIds)
            {
                usuario.UsuarioPais.Add(new UsuarioPaisAccesoDTO
                {
                    Usuario = new UsuariosDTO { UsuarioId = usuarioId },
                    Pais = new PaisesDTO { PaisId = paisId },
                    Acceso = true
                });
            }

            return usuariosDAO.Modificar(usuario);
        }

        public int EliminarUsuario(int usuarioId, int usuarioEliminacionId)
        {
            UsuariosDTO usuario = usuariosDAO.ObtenerPorId(usuarioId);
            if (usuario == null) return 0;

            UsuariosDTO usuarioEliminador = new UsuariosDTO { UsuarioId = usuarioEliminacionId };
            usuario.UsuarioEliminacion = usuarioEliminador;

            return usuariosDAO.eliminarLogico(usuario);
        }



    }
}