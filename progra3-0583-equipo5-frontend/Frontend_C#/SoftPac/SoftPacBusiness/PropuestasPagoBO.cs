using SoftPac.Model;
using SoftPac.Persistance.DAO;
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
        private PropuestasPagoDAO propuestasDAO;

        public PropuestasPagoBO()
        {
            this.propuestasDAO = new PropuestasPagoDAOImpl();
        }

        public IList<PropuestasPagoDTO> ListarUltimasPorUsuario(int usuarioId, int cantidad)
        {
            var todas = propuestasDAO.ListarTodos();
            return todas.Where(p => p.UsuarioCreacion != null && p.UsuarioCreacion.UsuarioId == usuarioId)
                        .OrderByDescending(p => p.FechaHoraCreacion)
                        .Take(cantidad)
                        .ToList();
        }

        // --- MÉTODO NUEVO ---
        public IList<PropuestasPagoDTO> ListarTodasPorUsuario(int usuarioId)
        {
            var todas = propuestasDAO.ListarTodos();
            return todas.Where(p => p.UsuarioCreacion != null && p.UsuarioCreacion.UsuarioId == usuarioId).ToList();
        }

        // --- MÉTODO NUEVO ---
        public IList<PropuestasPagoDTO> ListarActividadPorUsuario(int usuarioId)
        {
            // Llama al nuevo método del DAO que no filtra por borrado lógico
            return propuestasDAO.ListarTodaActividad()
                .Where(p =>
                    (p.UsuarioCreacion?.UsuarioId == usuarioId) ||
                    (p.UsuarioModificacion?.UsuarioId == usuarioId) ||
                    (p.UsuarioEliminacion?.UsuarioId == usuarioId)
                ).ToList();
        }

        public IList<PropuestasPagoDTO> ListarConFiltros(int? paisId, int? bancoId, string estado)
        {
            return propuestasDAO.ListarTodos().Where(p =>
                (paisId == null || (p.EntidadBancaria != null && p.EntidadBancaria.Pais != null && p.EntidadBancaria.Pais.PaisId == paisId)) &&
                (bancoId == null || (p.EntidadBancaria != null && p.EntidadBancaria.EntidadBancariaId == bancoId)) &&
                (string.IsNullOrEmpty(estado) || p.Estado == estado)
            ).ToList();
        }

        public PropuestasPagoDTO GenerarDetallesParciales(List<int> facturasSeleccionadas, int bancoId)
        {

            FacturasBO facturasBO = new FacturasBO();
            EntidadesBancariasBO bancosBO = new EntidadesBancariasBO();
            PropuestasPagoDTO propuestaParcial = new PropuestasPagoDTO();
            CuentasAcreedorBO cuentasAcreedorBO = new CuentasAcreedorBO();
            propuestaParcial.EntidadBancaria = bancosBO.ObtenerPorId(bancoId);
            propuestaParcial.Estado = "Pendiente";
            propuestaParcial.DetallesPropuesta = new BindingList<DetallesPropuestaDTO>();
            for (int i = 0; i < facturasSeleccionadas.Count; i++)
            {
                FacturasDTO factura = facturasBO.ObtenerPorId(facturasSeleccionadas[i]);
                if (factura == null || factura.Acreedor == null || factura.Acreedor.Pais == null)
                {
                    continue;
                }
                IList<CuentasAcreedorDTO> cuentasAcreedor = cuentasAcreedorBO.ObtenerPorAcreedor(factura.Acreedor.AcreedorId.Value);
                foreach (var cuenta in cuentasAcreedor)
                {
                    if (cuenta.EntidadBancaria.Pais.PaisId == propuestaParcial.EntidadBancaria.Pais.PaisId)
                    {
                        DetallesPropuestaDTO detalle = new DetallesPropuestaDTO();
                        detalle.Factura = factura;
                        detalle.CuentaAcreedor = cuenta;
                        detalle.MontoPago = factura.MontoRestante;
                        detalle.FormaPago = 'T'; // Transferencia
                        propuestaParcial.AddDetallePropuesta(detalle);
                        break; // Solo agregar una cuenta por factura
                    }
                }
            }

            return propuestaParcial;
        }

        public int Insertar(PropuestasPagoDTO propuestaCompleta)
        {
            return propuestasDAO.Insertar(propuestaCompleta);
        }


        public PropuestasPagoDTO GenerarDetallesPropuesta(PropuestasPagoDTO propuestaParcial, List<int> cuentasSeleccionadasIds)
        {
            CuentasPropiasDAO cuentasPropiasDAO = new CuentasPropiasDAOImpl();
            // 1️ Obtener las cuentas seleccionadas
            var cuentasSeleccionadas = new List<CuentasPropiasDTO>();
            foreach (var id in cuentasSeleccionadasIds)
            {
                var cuenta = cuentasPropiasDAO.ObtenerPorId(id);
                if(cuenta.Activa) cuentasSeleccionadas.Add(new CuentasPropiasDTO(cuenta));
            }

            // 2️ Limpiar cualquier asignación previa
            foreach (var detalle in propuestaParcial.DetallesPropuesta)
            {
                detalle.CuentaPropia = null;
            }

            // 3️ Agrupar los pagos por moneda
            var pagosPorMoneda = propuestaParcial.DetallesPropuesta
                .GroupBy(d => d.Factura.Moneda.CodigoIso);

            // 4️ Aplicar algoritmo greedy por moneda
            foreach (var grupo in pagosPorMoneda)
            {
                var cuentasMoneda = cuentasSeleccionadas
                    .Where(c => c.Moneda.CodigoIso == grupo.Key)
                    .OrderByDescending(c => c.SaldoDisponible)
                    .ToList();

                var pagos = grupo.OrderByDescending(p => p.MontoPago).ToList();

                foreach (var pago in pagos)
                {
                    // Buscar la primera cuenta que pueda cubrir el monto del pago
                    var cuenta = cuentasMoneda.FirstOrDefault(c => c.SaldoDisponible >= pago.MontoPago);

                    if (cuenta != null)
                    {
                        pago.CuentaPropia = cuenta;
                        cuenta.SaldoDisponible -= pago.MontoPago;
                    }
                }
            }

            return propuestaParcial;
        }

        public PropuestasPagoDTO ObtenerPorId(int propuestaId)
        {
            return propuestasDAO.ObtenerPorId(propuestaId);
        }

        public int Modificar(PropuestasPagoDTO propuesta)
        {
            return propuestasDAO.Modificar(propuesta);
        }
    }
}
