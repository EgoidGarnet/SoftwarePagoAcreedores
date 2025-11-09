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
            // Llamas al WS
            paisesDTO[] paises = this.paisesClienteSOAP.listarPaises();

            // Si el WS devuelve null, evitas excepción
            if (paises == null)
                paises = Array.Empty<paisesDTO>();

            // Conviertes a BindingList
            return new BindingList<paisesDTO>(paises.ToList());
        }

        public paisesDTO ObtenerPorId(int paisId)
        {
            return this.paisesClienteSOAP.obtenerPaisPorId(paisId);
        }
    }
}
