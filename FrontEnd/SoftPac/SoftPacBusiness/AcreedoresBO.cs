// SoftPac.Business.AcreedoresBO.cs
using System;
using System.ComponentModel;
using SoftPacBusiness.AcreedoresWS;
using System.Linq;

namespace SoftPac.Business
{
    public class AcreedoresBO
    {
        private AcreedoresWSClient acreedoresClienteSOAP;

        public AcreedoresBO()
        {
            this.acreedoresClienteSOAP = new AcreedoresWSClient();
        }

        public int insertar(string razon_social, string ruc, string direccion_fiscal,
                            string condicion, int plazo_de_pago, string activo, int id_pais)
        {


            return this.acreedoresClienteSOAP.insertarAcreedores(razon_social, ruc, direccion_fiscal, condicion, plazo_de_pago, activo, id_pais);
        }

        public int modificar(int id_acreedor, string razon_social, string ruc,
                             string direccion_fiscal, string condicion, int plazo_de_pago, string activo,
                             int id_pais)
        {

            return this.acreedoresClienteSOAP.modificarAcreedores(id_acreedor, razon_social, ruc,
                              direccion_fiscal, condicion, plazo_de_pago, activo, id_pais);
        }

        public int Eliminar(int acreedorId, usuariosDTO usuario)
        {
            acreedoresDTO dto = new acreedoresDTO();
            dto.acreedor_id = acreedorId;
            dto.acreedor_idSpecified = true;
            usuario.usuario_idSpecified = true;
            return this.acreedoresClienteSOAP.eliminarAcreedor(dto, usuario);
        }

        public int Eliminar(acreedoresDTO acreedor)
        {
            usuariosDTO dto = new usuariosDTO();
            dto = acreedor.usuario_eliminacion;
            return Eliminar(acreedor.acreedor_id, dto);
        }

        public acreedoresDTO obtenerPorId(int acreedor_id)
        {
            return this.acreedoresClienteSOAP.obtenerAcreedor(acreedor_id);
        }

        public BindingList<acreedoresDTO> ListarTodos()
        {
            acreedoresDTO[] acreedores = this.acreedoresClienteSOAP.listarAcreedores();

            if (acreedores == null)
                acreedores = Array.Empty<acreedoresDTO>();

            return new BindingList<acreedoresDTO>(acreedores.ToList());
        }

        public BindingList<acreedoresDTO> ListarPorPaises(int[] paises_ids)
        {
            acreedoresDTO[] acreedores = this.acreedoresClienteSOAP.listarAcreedoresPorPaises(paises_ids);

            if (acreedores == null)
                acreedores = Array.Empty<acreedoresDTO>();

            return new BindingList<acreedoresDTO>(acreedores.ToList());
        }
    }
}