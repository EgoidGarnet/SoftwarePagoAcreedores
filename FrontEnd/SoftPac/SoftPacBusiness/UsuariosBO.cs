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

        public usuariosDTO AutenticarUsuario(string nombreUsuario, string password)
        {

            return this.usuariosBO.autenticarUsuarioPorNombreUsuario(nombreUsuario, password);

        }

        public IList<usuariosDTO> ListarTodos()
        {
            return this.usuariosBO.listarUsuarios();
        }

        public usuariosDTO ObtenerPorId(int usuarioId)
        {
            return usuariosBO.obtenerUsuario(usuarioId);
        }

        public int InsertarUsuario(usuariosDTO nuevoUsuario, usuariosDTO usuarioActual)
        {
            SetearSpecifiedTrue(nuevoUsuario);
            SetearSpecifiedTrue(usuarioActual);
            return usuariosBO.insertarUsuario(nuevoUsuario, usuarioActual);
        }

        public int ModificarAccesoUsuario(int usuarioId, string nuevoNombreUsuario, bool activo, List<int> paisesIds, usuariosDTO usuarioActual,
            string nuevaContrasenha)
        {
            SetearSpecifiedTrue(usuarioActual);
            return usuariosBO.modificarAccesoUsuario(usuarioId, nuevoNombreUsuario, activo, paisesIds.ToArray(), usuarioActual, nuevaContrasenha);
        }

        public int EliminarUsuario(int usuarioId, int usuarioEliminacionId)
        {
            usuariosDTO usuario = usuariosBO.obtenerUsuario(usuarioId);
            //usuario.usuario_id = usuarioId;
            usuariosDTO usuarioEliminacion = usuariosBO.obtenerUsuario(usuarioEliminacionId);

            SetearSpecifiedTrue(usuario);
            SetearSpecifiedTrue(usuarioEliminacion);
            return usuariosBO.eliminarUsuario(usuario, usuarioEliminacion);
        }
        public void SetearSpecifiedTrue(usuariosDTO usuario)
        {
            usuario.superusuarioSpecified = true;
            usuario.activoSpecified = true;
            usuario.usuario_idSpecified = true;
            if (usuario.usuario_pais != null)
            {
                foreach (var usuario_pais in usuario.usuario_pais)
                {
                    usuario_pais.pais.pais_idSpecified = true;
                }
            }
        }

    }
}