using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class UsuarioPaisAccesoDTO
    {
        private UsuariosDTO usuario;
        private PaisesDTO pais;
        private Boolean acceso;

        public UsuarioPaisAccesoDTO()
        {
            this.Usuario = new UsuariosDTO();
            this.Pais = new PaisesDTO();
            Acceso = false;
        }

        public UsuarioPaisAccesoDTO(UsuariosDTO usuario, PaisesDTO pais, bool acceso)
        {
            this.Usuario = usuario;
            this.Pais = pais;
            this.Acceso = acceso;
        }

        public UsuariosDTO Usuario { get => usuario; set => usuario = value; }
        public PaisesDTO Pais { get => pais; set => pais = value; }
        public bool Acceso { get => acceso; set => acceso = value; }
    }
}
