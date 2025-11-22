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

        public void ActivarSpecified(cuentasPropiasDTO cuenta)
        {
            cuenta.activaSpecified = true;
            cuenta.cuenta_bancaria_idSpecified = true;
            cuenta.entidad_bancaria.entidad_bancaria_idSpecified = true;
            cuenta.moneda.moneda_idSpecified = true;
            cuenta.saldo_disponibleSpecified = true;
        }


        public int Insertar(cuentasPropiasDTO cuenta)
        {
            try
            {

                ActivarSpecified(cuenta);
                return this.cuentasPropiasClienteSOAP.insertarCuentaPropia(cuenta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar cuenta propia.", ex);
            }
        }

        public int Modificar(cuentasPropiasDTO cuenta,int usuarioId)
        {
            try
            {
                ActivarSpecified(cuenta);
                return this.cuentasPropiasClienteSOAP.modificarCuentaPropia(cuenta,usuarioId);
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
                cuenta.cuenta_bancaria_idSpecified = true;
                usuarioActual.usuario_idSpecified = true;
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

        public BindingList<kardexCuentasPropiasDTO> ObtenerKardexCuentaPropiaPorCuenta(int cuentaPropiaId)
        {
            try
            {
                kardexCuentasPropiasDTO[] kardex = this.cuentasPropiasClienteSOAP.obtenerKardexCuentaPropiaPorCuenta(cuentaPropiaId);
                if(kardex==null || kardex.Length == 0)
                {
                    return new BindingList<kardexCuentasPropiasDTO>();
                }
                return new BindingList<kardexCuentasPropiasDTO>(kardex);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Error al obtener kardex de cuenta propia por Id de cuenta", ex);
            }
        }

        public BindingList<kardexCuentasPropiasDTO> ObtenerKardexCuentaPropiaPorUsuario(int usuarioId)
        {
            kardexCuentasPropiasDTO[] kardex = this.cuentasPropiasClienteSOAP.obtenerKardexCuentaPropiaPorUsuario(usuarioId);
            if (kardex == null || kardex.Length == 0)
            {
                return new BindingList<kardexCuentasPropiasDTO>();
            }
            return new BindingList<kardexCuentasPropiasDTO>(kardex);
        }


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
