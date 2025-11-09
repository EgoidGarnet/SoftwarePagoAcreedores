using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class UsuariosBO
    {
        private UsuariosWSClient usuariosBO;
        
        public UsuariosBO()
        {
            this.usuariosBO = new UsuariosWSClient();
            
        }

        // --- MÉTODO NUEVO PARA AUTENTICAR ---
        public usuariosDTO AutenticarUsuario(string nombreUsuario, string password)
        {

            return this.usuariosBO.autenticarUsuarioPorNombreUsuario(nombreUsuario, password);

        }   

        // Aquí puedes añadir el resto de los métodos que necesites (Insertar, Modificar, etc.)
        public IList<usuariosDTO> ListarTodos()
        {
            return this.usuariosBO.listarUsuarios();
        }

        public usuariosDTO ObtenerPorId(int usuarioId)
        {
            return usuariosBO.obtenerUsuario(usuarioId);
        }

        public int InsertarUsuario(usuariosDTO nuevoUsuario)
        {
            SetearSpecifiedTrue(nuevoUsuario);
            return usuariosBO.insertarUsuario(nuevoUsuario);
        }

        public int ModificarAccesoUsuario(int usuarioId, string nuevoNombreUsuario, bool activo, List<int> paisesIds)
        {

            return usuariosBO.modificarAccesoUsuario(usuarioId, nuevoNombreUsuario, activo, paisesIds.ToArray());
        }

        public int EliminarUsuario(int usuarioId, int usuarioEliminacionId)
        {
            usuariosDTO usuario = new usuariosDTO();
            usuario.usuario_id = usuarioId;
            usuariosDTO usuarioEliminacion = new usuariosDTO();
            usuarioEliminacion.usuario_id = usuarioEliminacionId;
            SetearSpecifiedTrue(usuario);
            SetearSpecifiedTrue(usuarioEliminacion);
            return usuariosBO.eliminarUsuario(usuario,usuarioEliminacion);
        }
        public void SetearSpecifiedTrue(usuariosDTO usuario)
        {
            usuario.superusuarioSpecified = true;
            usuario.activoSpecified = true;
            usuario.usuario_idSpecified = true;
            if (usuario.usuario_pais != null)
            {
                foreach(var usuario_pais in usuario.usuario_pais)
                {
                    usuario_pais.pais.pais_idSpecified = true;
                }
            }
        }

    }
}