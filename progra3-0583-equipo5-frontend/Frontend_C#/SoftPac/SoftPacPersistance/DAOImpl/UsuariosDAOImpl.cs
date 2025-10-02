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
        private BindingList<UsuariosDTO> usuarios = new BindingList<UsuariosDTO>();

        public UsuariosDAOImpl()
        {
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 1,
                Nombre = "Juan Perez",
                CorreoElectronico = "juan@email.com",
                UsuarioEliminacion = null,
                FechaEliminacion = null,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> {
                    new UsuarioPaisAccesoDTO
                    {
                        Usuario = new UsuariosDTO
                        {
                            UsuarioId = 1,
                            Nombre = "Juan Perez",
                            CorreoElectronico = "juan@email.com"
                        },
                        Pais = new PaisesDTO
                        {
                            PaisId = 1,
                            Nombre = "Costa Rica"
                        },
                        Acceso=true
                    }
                }
            });
            usuarios.Add(new UsuariosDTO
            {
                UsuarioId = 2,
                Nombre = "Ana Gomez",
                CorreoElectronico = "ana@email.com",
                UsuarioEliminacion = null,
                FechaEliminacion = null,
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO> {
                    new UsuarioPaisAccesoDTO
                    {
                        Usuario = new UsuariosDTO
                        {
                            UsuarioId = 2,
                            Nombre = "Ana Gomez",
                            CorreoElectronico = "ana@email.com"
                        },
                        Pais = new PaisesDTO
                        {
                            PaisId = 2,
                            Nombre = "España"
                        },
                        Acceso=true
                    }
                }
            });
        }

        public int Insertar(UsuariosDTO usuario)
        {
            if (usuario.UsuarioId == null || usuarios.Any(u => u.UsuarioId == usuario.UsuarioId))
                return 0; // ID must be unique and not null
            usuarios.Add(usuario);
            return 1;
        }

        public int Modificar(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null)
                return 0;
            int idx = usuarios.IndexOf(existing);
            usuarios[idx] = usuario;
            return 1;
        }

        public int eliminar(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null)
                return 0;
            usuarios.Remove(existing);
            return 1;
        }

        public int eliminarLogico(UsuariosDTO usuario)
        {
            var existing = usuarios.FirstOrDefault(u => u.UsuarioId == usuario.UsuarioId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = usuario;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }

        public UsuariosDTO ObtenerPorId(int usuarioId)
        {
            return usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
        }

        public IList<UsuariosDTO> ListarTodos()
        {
            return usuarios.ToList();
        }

        public UsuariosDTO ObtenerPorCorreo(String correo)
        {
            return usuarios.FirstOrDefault(u => u.CorreoElectronico == correo);
        }
    }
}
