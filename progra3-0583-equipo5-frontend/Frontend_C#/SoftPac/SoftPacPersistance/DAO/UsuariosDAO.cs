using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface UsuariosDAO
    {
        int Insertar(UsuariosDTO usuario);
        int Modificar(UsuariosDTO usuario);
        int eliminar(UsuariosDTO usuario);
        int eliminarLogico(UsuariosDTO usuario);
        UsuariosDTO ObtenerPorId(int usuarioId);
        IList<UsuariosDTO> ListarTodos();
        UsuariosDTO ObtenerPorCorreo(String correo);

        UsuariosDTO ObtenerPorNombreUsuario(string nombreUsuario); // <-- AÑADIR ESTA LÍNEA

    }
}
