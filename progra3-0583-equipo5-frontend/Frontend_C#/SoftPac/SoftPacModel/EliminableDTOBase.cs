using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public abstract class EliminableDTOBase
    {
        private UsuariosDTO usuarioEliminacion;
        private DateTime? fechaEliminacion;

        public EliminableDTOBase()
        {
            this.UsuarioEliminacion = null;
            this.FechaEliminacion = null; 
        }

        public EliminableDTOBase(EliminableDTOBase other)
        {
            this.UsuarioEliminacion = other.UsuarioEliminacion;
            this.FechaEliminacion = other.FechaEliminacion;
        }

        public UsuariosDTO UsuarioEliminacion { get => usuarioEliminacion; set => usuarioEliminacion = value; }
        public DateTime? FechaEliminacion { get => fechaEliminacion; set => fechaEliminacion = value; }
    }
}
