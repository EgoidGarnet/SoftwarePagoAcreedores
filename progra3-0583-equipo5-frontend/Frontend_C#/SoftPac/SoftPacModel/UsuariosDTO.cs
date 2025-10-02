using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class UsuariosDTO : EliminableDTOBase
    {
        private int? usuarioId;
        private String correoElectronico;
        private String nombreDeUsuario;
        private String nombre;
        private String apellidos;
        private Boolean activo;
        private String passwordHash;
        private Boolean superusuario;
        private BindingList<UsuarioPaisAccesoDTO> usuarioPais;

        public UsuariosDTO() : base()
        {
            this.UsuarioPais = new BindingList<UsuarioPaisAccesoDTO>();
            UsuarioId = null;
            CorreoElectronico = null;
            NombreDeUsuario = null;
            Nombre = null;
            Apellidos = null;
            Activo = false;
            PasswordHash = null;
            Superusuario = false;
        }

        public UsuariosDTO(int usuario_id, string correo_electronico, string nombre_de_usuario, string nombre, string apellidos, bool activo, string password_hash, bool superusuario, BindingList<UsuarioPaisAccesoDTO> usuario_pais) : base()
        {
            this.UsuarioId = usuario_id;
            this.CorreoElectronico = correo_electronico;
            this.NombreDeUsuario = nombre_de_usuario;
            this.Nombre = nombre;
            this.Apellidos = apellidos;
            this.Activo = activo;
            this.PasswordHash = password_hash;
            this.Superusuario = superusuario;
            this.UsuarioPais = usuario_pais;
        }

        public UsuariosDTO(UsuariosDTO other) : base(other)
        {
            this.UsuarioId = other.UsuarioId;
            this.CorreoElectronico = other.CorreoElectronico;
            this.NombreDeUsuario = other.NombreDeUsuario;
            this.Nombre = other.Nombre;
            this.Apellidos = other.Apellidos;
            this.Activo = other.Activo;
            this.PasswordHash = other.PasswordHash;
            this.Superusuario = other.Superusuario;
            this.UsuarioPais = other.usuarioPais;
        }

        public void AddUsuarioPais(UsuarioPaisAccesoDTO usuarioPais)
        {
            if (UsuarioPais == null)
            {
                UsuarioPais = new BindingList<UsuarioPaisAccesoDTO>();
            }
            UsuarioPais.Add(usuarioPais);
        }

        public int? UsuarioId { get => usuarioId; set => usuarioId = value; }
        public string CorreoElectronico { get => correoElectronico; set => correoElectronico = value; }
        public string NombreDeUsuario { get => nombreDeUsuario; set => nombreDeUsuario = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public bool Activo { get => activo; set => activo = value; }
        public string PasswordHash { get => passwordHash; set => passwordHash = value; }
        public bool Superusuario { get => superusuario; set => superusuario = value; }
        public BindingList<UsuarioPaisAccesoDTO> UsuarioPais { get => usuarioPais; set => usuarioPais = value; }

    }
}
