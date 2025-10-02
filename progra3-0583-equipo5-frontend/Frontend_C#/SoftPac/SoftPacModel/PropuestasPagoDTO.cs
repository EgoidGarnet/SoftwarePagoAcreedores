using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class PropuestasPagoDTO : EliminableDTOBase
    {
        private int? propuestaId;
        private UsuariosDTO usuarioCreacion;
        private DateTime? fechaHoraCreacion;
        private String estado;
        private EntidadesBancariasDTO entidadBancaria;
        private BindingList<DetallesPropuestaDTO> detallesPropuesta;
        private DateTime? fechaHoraModificacion;
        private UsuariosDTO usuarioModificacion;

        public PropuestasPagoDTO() : base()
        {
            this.EntidadBancaria = new EntidadesBancariasDTO();
            this.UsuarioCreacion = null;
            this.UsuarioModificacion = null;
            this.DetallesPropuesta = new BindingList<DetallesPropuestaDTO>();
            PropuestaId = null;
            FechaHoraCreacion = null;
            Estado = "I";
            FechaHoraModificacion = null;
        }

        public PropuestasPagoDTO(int propuesta_id, UsuariosDTO usuario_creacion, DateTime fecha_hora_creacion, string estado, EntidadesBancariasDTO entidad_bancaria, DateTime? fecha_hora_modificacion, UsuariosDTO usuario_modificacion) : base()
        {
            this.PropuestaId = propuesta_id;
            this.UsuarioCreacion = new UsuariosDTO(usuario_creacion);
            this.FechaHoraCreacion = fecha_hora_creacion;
            this.Estado = estado;
            this.EntidadBancaria = entidad_bancaria;
            this.DetallesPropuesta = new BindingList<DetallesPropuestaDTO>();
            this.FechaHoraModificacion = fecha_hora_modificacion;
            this.UsuarioModificacion = new UsuariosDTO(usuario_modificacion);
        }

        public PropuestasPagoDTO(int propuesta_id, UsuariosDTO usuario_creacion, DateTime fecha_hora_creacion, string estado, EntidadesBancariasDTO entidad_bancaria, BindingList<DetallesPropuestaDTO> detalles_propuesta, DateTime? fecha_hora_modificacion, UsuariosDTO usuario_modificacion) : base()
        {
            this.PropuestaId = propuesta_id;
            this.UsuarioCreacion = new UsuariosDTO(usuario_creacion);
            this.FechaHoraCreacion = fecha_hora_creacion;
            this.Estado = estado;
            this.EntidadBancaria = entidad_bancaria;
            this.DetallesPropuesta = new BindingList<DetallesPropuestaDTO>();
            this.FechaHoraModificacion = fecha_hora_modificacion;
            this.UsuarioModificacion = new UsuariosDTO(usuario_modificacion);
        }

        public PropuestasPagoDTO(PropuestasPagoDTO other) : base(other)
        {
            this.PropuestaId = other.PropuestaId;
            this.UsuarioCreacion = new UsuariosDTO(UsuarioCreacion);
            this.FechaHoraCreacion = other.FechaHoraCreacion;
            this.Estado = Estado;
            this.EntidadBancaria = new EntidadesBancariasDTO(other.EntidadBancaria);
            this.DetallesPropuesta = other.DetallesPropuesta;
            this.FechaHoraModificacion = other.FechaHoraModificacion;
            this.UsuarioModificacion = new UsuariosDTO(other.UsuarioModificacion);
        }

        public void AddDetallePropuesta(DetallesPropuestaDTO detalle)
        {
            if (DetallesPropuesta == null)
            {
                DetallesPropuesta = new BindingList<DetallesPropuestaDTO>();
            }
            DetallesPropuesta.Add(detalle);
        }

        public int? PropuestaId { get => propuestaId; set => propuestaId = value; }
        public UsuariosDTO UsuarioCreacion { get => usuarioCreacion; set => usuarioCreacion = value; }
        public DateTime? FechaHoraCreacion { get => fechaHoraCreacion; set => fechaHoraCreacion = value; }
        public string Estado { get => estado; set => estado = value; }
        public EntidadesBancariasDTO EntidadBancaria { get => entidadBancaria; set => entidadBancaria = value; }
        public BindingList<DetallesPropuestaDTO> DetallesPropuesta { get => detallesPropuesta; set => detallesPropuesta = value; }
        public DateTime? FechaHoraModificacion { get => fechaHoraModificacion; set => fechaHoraModificacion = value; }
        public UsuariosDTO UsuarioModificacion { get => usuarioModificacion; set => usuarioModificacion = value; }

    }
}
