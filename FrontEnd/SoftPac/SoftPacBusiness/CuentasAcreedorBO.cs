using SoftPacBusiness.CuentasAcreedorWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SoftPac.Business
{
    public class CuentasAcreedorBO
    {
        private CuentasAcreedorWSClient cuentasAcreedorClienteSOAP;

        public CuentasAcreedorBO()
        {
            this.cuentasAcreedorClienteSOAP = new CuentasAcreedorWSClient();
        }

        public IList<cuentasAcreedorDTO> ObtenerPorAcreedor(int acreedor_id)
        {
            try
            {
                var cuentas = this.cuentasAcreedorClienteSOAP.obtenerCuentasPorAcreedor(acreedor_id);
                if (cuentas == null)
                    cuentas = Array.Empty<cuentasAcreedorDTO>();
                return cuentas.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener cuentas por acreedor.", ex);
            }
        }

        public IList<cuentasAcreedorDTO> ListarTodos()
        {
            try
            {
                var cuentas = this.cuentasAcreedorClienteSOAP.listarCuentasAcreedor();
                if (cuentas == null)
                    cuentas = Array.Empty<cuentasAcreedorDTO>();
                return cuentas.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar cuentas de acreedor.", ex);
            }
        }

        public cuentasAcreedorDTO ObtenerPorId(int cuenta_bancaria_id)
        {
            try
            {
                return this.cuentasAcreedorClienteSOAP.obtenerCuentaAcreedor(cuenta_bancaria_id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener cuenta de acreedor por ID.", ex);
            }
        }

        public int insertar(cuentasAcreedorDTO cuenta)
        {
            try
            {
                return this.cuentasAcreedorClienteSOAP.insertarCuentasAcreedor(cuenta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar cuenta de acreedor.", ex);
            }
        }

        public int modificar(cuentasAcreedorDTO cuenta)
        {
            try
            {
                return this.cuentasAcreedorClienteSOAP.modificarCuentasAcreedor(cuenta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al modificar cuenta de acreedor.", ex);
            }
        }

        public int eliminarLogico(int cuenta_bancaria_id, usuariosDTO usuario)
        {
            try
            {
                // Creamos un DTO mínimo con los datos necesarios
                var cuenta = new cuentasAcreedorDTO();
                cuenta.cuenta_bancaria_id = cuenta_bancaria_id;
                cuenta.cuenta_bancaria_idSpecified = true;
                usuario.usuario_idSpecified = true;
                return this.cuentasAcreedorClienteSOAP.eliminarCuentaAcreedorParametros(cuenta, usuario);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al eliminar cuenta de acreedor con parámetros.", ex);
            }
        }

    }
}