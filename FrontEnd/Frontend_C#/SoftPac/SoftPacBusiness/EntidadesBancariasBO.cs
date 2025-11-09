using SoftPacBusiness.EntidadesBancariasWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class EntidadesBancariasBO
    {
        private EntidadesBancariasWSClient entidadesClienteSOAP;

        public EntidadesBancariasBO()
        {
            this.entidadesClienteSOAP = new EntidadesBancariasWSClient();
        }

        public IList<entidadesBancariasDTO> ListarTodos()
        {
            return entidadesClienteSOAP.listarEntidadesBancarias();
        }

        public entidadesBancariasDTO ObtenerPorId(int bancoId)
        {
            return entidadesClienteSOAP.obtenerEntidadBancariaPorId(bancoId);
        }

    }
}
