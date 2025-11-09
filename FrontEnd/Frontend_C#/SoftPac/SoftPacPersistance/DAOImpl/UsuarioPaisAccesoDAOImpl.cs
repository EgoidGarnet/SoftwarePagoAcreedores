using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class UsuarioPaisAccesoDAOImpl : UsuarioPaisAccesoDAO
    {
        private BindingList<UsuarioPaisAccesoDTO> usuarioPaisAcceso = new BindingList<UsuarioPaisAccesoDTO>();

        public UsuarioPaisAccesoDAOImpl()
        {
            usuarioPaisAcceso.Add(new UsuarioPaisAccesoDTO {
                Usuario = new UsuariosDTO {
                    UsuarioId = 1,
                    Nombre = "Juan Perez",
                    CorreoElectronico = "juan@email.com"
                },
                Pais = new PaisesDTO {
                    PaisId = 1,
                    Nombre = "Costa Rica"
                }
            });
            usuarioPaisAcceso.Add(new UsuarioPaisAccesoDTO {
                Usuario = new UsuariosDTO {
                    UsuarioId = 2,
                    Nombre = "Ana Gomez",
                    CorreoElectronico = "ana@email.com"
                },
                Pais = new PaisesDTO {
                    PaisId = 2,
                    Nombre = "España"
                }
            });
        }

        public int Insertar(UsuarioPaisAccesoDTO usuarioPaisAccesoDTO)
        {
            if (usuarioPaisAccesoDTO.Usuario == null || usuarioPaisAccesoDTO.Pais == null || usuarioPaisAcceso.Any(u => u.Usuario.UsuarioId == usuarioPaisAccesoDTO.Usuario.UsuarioId && u.Pais.PaisId == usuarioPaisAccesoDTO.Pais.PaisId))
                return 0;
            usuarioPaisAcceso.Add(usuarioPaisAccesoDTO);
            return 1;
        }
        public int Modificar(UsuarioPaisAccesoDTO usuarioPaisAccesoDTO)
        {
            var existing = usuarioPaisAcceso.FirstOrDefault(u => u.Usuario.UsuarioId == usuarioPaisAccesoDTO.Usuario.UsuarioId && u.Pais.PaisId == usuarioPaisAccesoDTO.Pais.PaisId);
            if (existing == null)
                return 0;
            int idx = usuarioPaisAcceso.IndexOf(existing);
            usuarioPaisAcceso[idx] = usuarioPaisAccesoDTO;
            return 1;
        }
        public int Eliminar(UsuarioPaisAccesoDTO usuarioPaisAccesoDTO)
        {
            var existing = usuarioPaisAcceso.FirstOrDefault(u => u.Usuario.UsuarioId == usuarioPaisAccesoDTO.Usuario.UsuarioId && u.Pais.PaisId == usuarioPaisAccesoDTO.Pais.PaisId);
            if (existing == null)
                return 0;
            usuarioPaisAcceso.Remove(existing);
            return 1;
        }
        public UsuarioPaisAccesoDTO ObtenerPorUsuarioYPais(int usuarioId, int paisId)
        {
            return usuarioPaisAcceso.FirstOrDefault(u => u.Usuario.UsuarioId == usuarioId && u.Pais.PaisId == paisId);
        }
    }
}
