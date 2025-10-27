using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftPac.Business
{
    public class CuentasPropiasBO
    {
        private CuentasPropiasDAO cuentasDAO;

        public CuentasPropiasBO()
        {
            // En tu arquitectura actual, el BO instancia directamente el DAOImpl.
            this.cuentasDAO = new CuentasPropiasDAOImpl();
        }

        public IList<CuentasPropiasDTO> ListarTodos()
        {
            return cuentasDAO.ListarTodos();
        }

        public CuentasPropiasDTO ObtenerPorId(int id)
        {
            return cuentasDAO.ObtenerPorId(id);
        }

        public int Insertar(CuentasPropiasDTO cuenta)
        {
            // Aquí puedes añadir validaciones de negocio. Por ejemplo:
            if (string.IsNullOrWhiteSpace(cuenta.NumeroCuenta))
            {
                throw new Exception("El número de cuenta es obligatorio.");
            }
            if (cuenta.SaldoDisponible < 0)
            {
                throw new Exception("El saldo disponible no puede ser negativo.");
            }
            return cuentasDAO.Insertar(cuenta);
        }

        public int Modificar(CuentasPropiasDTO cuenta)
        {
            if (string.IsNullOrWhiteSpace(cuenta.NumeroCuenta))
            {
                throw new Exception("El número de cuenta es obligatorio.");
            }
            return cuentasDAO.Modificar(cuenta);
        }

        public int Eliminar(int cuentaId, int usuarioEliminacionId)
        {
            CuentasPropiasDTO cuenta = cuentasDAO.ObtenerPorId(cuentaId);
            if (cuenta == null) return 0;

            cuenta.UsuarioEliminacion = new UsuariosDTO { UsuarioId = usuarioEliminacionId };
            return cuentasDAO.eliminarLogico(cuenta);
        }

        public IList<CuentasPropiasDTO> ListarPorEntidadBancaria(int bancoId)
        {
            return cuentasDAO.ListarTodos()
                    .Where(c => c.Activa &&
                                c.EntidadBancaria?.EntidadBancariaId == bancoId)
                    .ToList();
        }
    }
}
