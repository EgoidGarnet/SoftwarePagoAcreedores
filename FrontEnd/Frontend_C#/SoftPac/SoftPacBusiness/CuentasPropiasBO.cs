using SoftPacBusiness.CuentasPropiasWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using usuariosDTO = SoftPacBusiness.CuentasPropiasWS.usuariosDTO;

namespace SoftPac.Business
{
    public class CuentasPropiasBO
    {
        private CuentasPropiasWSClient cuentasPropiasClienteSOAP;

        public CuentasPropiasBO()
        {
            this.cuentasPropiasClienteSOAP = new CuentasPropiasWSClient();
        }

        // ========== MÉTODOS CRUD PRINCIPALES ==========

        public int Insertar(cuentasPropiasDTO cuenta)
        {
            try
            {
                return this.cuentasPropiasClienteSOAP.insertarCuentaPropia(cuenta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar cuenta propia.", ex);
            }
        }

        public int Modificar(cuentasPropiasDTO cuenta)
        {
            try
            {
                return this.cuentasPropiasClienteSOAP.modificarCuentaPropia(cuenta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al modificar cuenta propia.", ex);
            }
        }

        public int Eliminar(cuentasPropiasDTO cuenta, usuariosDTO usuarioActual)
        {
            try
            {
                return this.cuentasPropiasClienteSOAP.eliminarCuentaPropia(cuenta, usuarioActual);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al eliminar cuenta propia.", ex);
            }
        }

        public cuentasPropiasDTO ObtenerPorId(int cuentaId)
        {
            try
            {
                return this.cuentasPropiasClienteSOAP.obtenerCuentaPropiaPorId(cuentaId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener cuenta propia por ID.", ex);
            }
        }

        // ========== MÉTODOS DE LISTADO ==========

        public BindingList<cuentasPropiasDTO> ListarTodos()
        {
            try
            {
                cuentasPropiasDTO[] cuentas = this.cuentasPropiasClienteSOAP.listarCuentasPropias();
                if (cuentas == null)
                    cuentas = Array.Empty<cuentasPropiasDTO>();
                return new BindingList<cuentasPropiasDTO>(cuentas.ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar cuentas propias.", ex);
            }
        }

        public BindingList<cuentasPropiasDTO> ListarActivas()
        {
            try
            {
                cuentasPropiasDTO[] cuentas = this.cuentasPropiasClienteSOAP.listarCuentasActivas();
                if (cuentas == null)
                    cuentas = Array.Empty<cuentasPropiasDTO>();
                return new BindingList<cuentasPropiasDTO>(cuentas.ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar cuentas propias activas.", ex);
            }
        }

        public BindingList<cuentasPropiasDTO> ListarPorEntidadBancaria(int entidadBancariaId)
        {
            try
            {
                cuentasPropiasDTO[] cuentas = this.cuentasPropiasClienteSOAP.listarPorEntidadBancaria(entidadBancariaId);
                if (cuentas == null)
                    cuentas = Array.Empty<cuentasPropiasDTO>();
                return new BindingList<cuentasPropiasDTO>(cuentas.ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar cuentas propias por entidad bancaria.", ex);
            }
        }

        // ========== MÉTODO AUXILIAR ==========

        public bool TieneSaldoSuficiente(int cuentaId, decimal montoRequerido)
        {
            try
            {
                return this.cuentasPropiasClienteSOAP.tieneSaldoSuficiente(cuentaId, montoRequerido);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al verificar saldo suficiente.", ex);
            }
        }
    }
}
