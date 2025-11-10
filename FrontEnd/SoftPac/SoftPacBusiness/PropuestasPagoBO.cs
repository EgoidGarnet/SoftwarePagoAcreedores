using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class PropuestasPagoBO
    {
        private PropuestaPagoWSClient propuestaPagoClienteSOAP;

        public PropuestasPagoBO()
        {
            this.propuestaPagoClienteSOAP = new PropuestaPagoWSClient();
        }

        public IList<propuestasPagoDTO> ListarUltimasPorUsuario(int usuarioId, int cantidad)
        {
            return propuestaPagoClienteSOAP.listarUltimasPropuestasPorUsuario(usuarioId,cantidad);
            
        }

        // --- MÉTODO NUEVO ---
        public IList<propuestasPagoDTO> ListarTodasPorUsuario(int usuarioId)
        {
            return propuestaPagoClienteSOAP.listarPropuestasPorUsuario(usuarioId);
        }

        // --- MÉTODO NUEVO ---
        public IList<propuestasPagoDTO> ListarActividadPorUsuario(int usuarioId)
        {
            // Llama al nuevo método del DAO que no filtra por borrado lógico
            return propuestaPagoClienteSOAP.listarActividadPorUsuario(usuarioId);
        }

        public IList<propuestasPagoDTO> ListarConFiltros(int? paisId, int? bancoId, string estado)
        {
            return propuestaPagoClienteSOAP.listarConFiltros(bancoId??0, estado??"") ?? Array.Empty<propuestasPagoDTO>();
        }

        public propuestasPagoDTO GenerarDetallesParciales(List<int> facturasSeleccionadas, int bancoId)
        {

            return propuestaPagoClienteSOAP.generarDetallesParciales(facturasSeleccionadas.ToArray(), bancoId);
        }

        public int Insertar(propuestasPagoDTO propuestaCompleta)
        {
            LlenarSpecified(propuestaCompleta);
            return propuestaPagoClienteSOAP.insertarPropuesta(propuestaCompleta);
        }


        public propuestasPagoDTO GenerarDetallesPropuesta(propuestasPagoDTO propuestaParcial, List<int> cuentasSeleccionadasIds)
        {
            return propuestaPagoClienteSOAP.generarDetallesPropuesta(propuestaParcial, cuentasSeleccionadasIds.ToArray());
        }

        public propuestasPagoDTO ObtenerPorId(int propuestaId)
        {
            return propuestaPagoClienteSOAP.obtenerPropuesta(propuestaId);
        }

        public int Modificar(propuestasPagoDTO propuesta)
        {
            LlenarSpecified(propuesta);
            return propuestaPagoClienteSOAP.modificarPropuesta(propuesta);
        }

        public void LlenarSpecified(propuestasPagoDTO propuestaCompleta)
        {
            propuestaCompleta.fecha_hora_creacionSpecified = true;
            propuestaCompleta.fecha_hora_modificacionSpecified = true;
            propuestaCompleta.propuesta_idSpecified = true;
            propuestaCompleta.entidad_bancaria.entidad_bancaria_idSpecified = true;
            propuestaCompleta.usuario_creacion.usuario_idSpecified = true;
            propuestaCompleta.usuario_modificacion.usuario_idSpecified = true;
        }
    }
}
