using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class EntidadesBancariasBO
    {
        private EntidadesBancariasDAO entidadesDAO;

        public EntidadesBancariasBO()
        {
            this.entidadesDAO = new EntidadesBancariasDAOImpl();
        }

        public IList<EntidadesBancariasDTO> ListarTodos()
        {
            return entidadesDAO.ListarTodos();
        }

        public EntidadesBancariasDTO ObtenerPorId(int bancoId)
        {
            return entidadesDAO.ObtenerPorId(bancoId);
        }

    }
}
