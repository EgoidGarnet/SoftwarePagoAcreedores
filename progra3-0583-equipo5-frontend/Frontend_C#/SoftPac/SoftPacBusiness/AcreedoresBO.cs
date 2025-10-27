// SoftPac.Business.AcreedoresBO.cs
using SoftPac.Persistance.DAOImpl;
using SoftPac.Model;
using System;
using System.ComponentModel;

namespace SoftPac.Business
{
    public class AcreedoresBO
    {
        private AcreedoresDAOImpl acreedorDAO;

        public AcreedoresBO()
        {
            this.acreedorDAO = new AcreedoresDAOImpl();
        }

        public int insertar(string razon_social, string ruc, string direccion_fiscal,
                            string condicion, int plazo_de_pago, string activo, int id_pais)
        {
            AcreedoresDTO dto = new AcreedoresDTO();
            dto.RazonSocial = razon_social;
            dto.Ruc = ruc;
            dto.DireccionFiscal = direccion_fiscal;
            dto.Condicion = condicion;
            dto.PlazoDePago = plazo_de_pago;
            dto.Activo = activo == "S";
            PaisesDTO pais = new PaisesDTO();
            pais.PaisId = id_pais;
            dto.Pais = pais;

            return this.acreedorDAO.insertar(dto);
        }

        public int modificar(int id_acreedor, string razon_social, string ruc,
                             string direccion_fiscal, string condicion, int plazo_de_pago, string activo,
                             int id_pais)
        {
            AcreedoresDTO dto = new AcreedoresDTO();
            dto.AcreedorId = id_acreedor;
            dto.RazonSocial = razon_social;
            dto.Ruc = ruc;
            dto.DireccionFiscal = direccion_fiscal;
            dto.Condicion = condicion;
            dto.PlazoDePago = plazo_de_pago;
            dto.Activo = activo == "S";
            PaisesDTO pais = new PaisesDTO();
            pais.PaisId = id_pais;
            dto.Pais = pais;

            return this.acreedorDAO.modificar(dto);
        }

        public int Eliminar(int acreedorId, UsuariosDTO usuario)
        {
            AcreedoresDTO dto = new AcreedoresDTO();
            dto.AcreedorId = acreedorId;
            dto.UsuarioEliminacion = usuario;
            dto.FechaEliminacion = DateTime.Now;
            return this.acreedorDAO.eliminarLogico(dto);
        }

        public int Eliminar(AcreedoresDTO acreedor)
        {
            return this.acreedorDAO.eliminarLogico(acreedor);
        }

        public AcreedoresDTO obtenerPorId(int acreedor_id)
        {
            return this.acreedorDAO.obtenerPorId(acreedor_id);
        }

        public BindingList<AcreedoresDTO> ListarTodos()
        {
            return (BindingList<AcreedoresDTO>)this.acreedorDAO.ListarTodos();
        }
    }
}
