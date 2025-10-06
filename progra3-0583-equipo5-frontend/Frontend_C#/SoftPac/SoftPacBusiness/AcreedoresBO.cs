using SoftPac.Persistance.DAO;
using SoftPac.Persistance.DAOImpl;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public int insertar(String razon_social, String ruc, String direccion_fiscal,
                 String condicion, int plazo_de_pago, String activo, int id_pais)
        {
            AcreedoresDTO acreedoresDTO = new AcreedoresDTO();
            acreedoresDTO.RazonSocial=razon_social;
            acreedoresDTO.Ruc=ruc;
            acreedoresDTO.PlazoDePago=plazo_de_pago;
            acreedoresDTO.Condicion=condicion;
            acreedoresDTO.DireccionFiscal=direccion_fiscal;
            acreedoresDTO.Activo=activo.Equals("S") ? true : false;
            PaisesDTO pais = new PaisesDTO();
            pais.PaisId=id_pais;
            acreedoresDTO.Pais=pais;

            return this.acreedorDAO.insertar(acreedoresDTO);
        }

        public int modificar(int id_acreedor, String razon_social, String ruc,
                String direccion_fiscal, String condicion, int plazo_de_pago, String activo,
                int id_pais)
        {
            AcreedoresDTO acreedoresDTO = new AcreedoresDTO();
            acreedoresDTO.AcreedorId=id_acreedor;
            acreedoresDTO.RazonSocial=razon_social;
            acreedoresDTO.Ruc=ruc;
            acreedoresDTO.DireccionFiscal=direccion_fiscal;
            acreedoresDTO.Condicion=condicion;
            acreedoresDTO.PlazoDePago=plazo_de_pago;
            acreedoresDTO.Activo = activo.Equals("S") ? true : false;
            acreedoresDTO.PlazoDePago=plazo_de_pago;
            PaisesDTO pais = new PaisesDTO();
            pais.PaisId = id_pais;
            acreedoresDTO.Pais=pais;
            return this.acreedorDAO.modificar(acreedoresDTO);
        }

        public int eliminarLogico(){
            return 0;
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
