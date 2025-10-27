using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System.Collections.Generic;

namespace SoftPac.Business
{
    public class CuentasAcreedorBO
    {
        private CuentasAcreedorDAOImpl cuentasAcreedorDAO;

        public CuentasAcreedorBO()
        {
            this.cuentasAcreedorDAO = new CuentasAcreedorDAOImpl();
        }

        public IList<CuentasAcreedorDTO> obtenerPorAcreedor(int acreedor_id)
        {
            return this.cuentasAcreedorDAO.ObtenerPorAcreedor(acreedor_id);
        }

        public IList<CuentasAcreedorDTO> ListarTodos()
        {
            return this.cuentasAcreedorDAO.ListarTodos();
        }

        public IList<CuentasAcreedorDTO> ObtenerPorAcreedor(int acreedor_id)
        {
            return this.obtenerPorAcreedor(acreedor_id);
        }

        public CuentasAcreedorDTO ObtenerPorId(int cuenta_bancaria_id)
        {
            return this.cuentasAcreedorDAO.ObtenerPorId(cuenta_bancaria_id);
        }

        public int insertar(CuentasAcreedorDTO cuenta)
        {
            return this.cuentasAcreedorDAO.Insertar(cuenta);
        }

        public int modificar(CuentasAcreedorDTO cuenta)
        {
            return this.cuentasAcreedorDAO.Modificar(cuenta);
        }

        public int eliminarLogico(int cuenta_bancaria_id, UsuariosDTO usuario)
        {
            var dto = new CuentasAcreedorDTO
            {
                CuentaBancariaId = cuenta_bancaria_id,
                UsuarioEliminacion = usuario,
                FechaEliminacion = System.DateTime.Now
            };
            return this.cuentasAcreedorDAO.eliminarLogico(dto);
        }

        public int eliminarLogico(CuentasAcreedorDTO cuenta)
        {
            return this.cuentasAcreedorDAO.eliminarLogico(cuenta);
        }
    }
}
