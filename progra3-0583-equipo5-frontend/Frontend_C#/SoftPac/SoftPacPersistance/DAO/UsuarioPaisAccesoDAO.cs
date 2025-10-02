using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface UsuarioPaisAccesoDAO
    {
        int Insertar(UsuarioPaisAccesoDTO usuarioPaisAcceso);
        int Modificar(UsuarioPaisAccesoDTO usuarioPaisAcceso);
        int Eliminar(UsuarioPaisAccesoDTO usuarioPaisAcceso);
        UsuarioPaisAccesoDTO ObtenerPorUsuarioYPais(int usuarioId, int paisId);
    }
}
