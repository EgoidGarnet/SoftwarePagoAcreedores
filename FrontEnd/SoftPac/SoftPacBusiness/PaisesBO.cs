using SoftPacBusiness.PaisesWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class PaisesBO
    {
        PaisesWSClient paisesClienteSOAP;

        public PaisesBO()
        {
            this.paisesClienteSOAP = new PaisesWSClient();
        }
        public BindingList<paisesDTO> ListarTodos()
        {
            paisesDTO[] paises = this.paisesClienteSOAP.listarPaises();

            if (paises == null)
                paises = Array.Empty<paisesDTO>();

            return new BindingList<paisesDTO>(paises.ToList());
        }

        public paisesDTO ObtenerPorId(int paisId)
        {
            return this.paisesClienteSOAP.obtenerPaisPorId(paisId);
        }
    }
}
