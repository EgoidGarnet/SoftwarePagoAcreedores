using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class MonedasBO
    {
        private MonedasDAO monedasDAO;

        public MonedasBO()
        {
            monedasDAO = new MonedasDAOImpl();
        }
        public BindingList<MonedasDTO> ListarTodos()
        {
            return monedasDAO.ListarTodos() as BindingList<MonedasDTO>;
        }
        public MonedasDTO ObtenerPorId(int monedaId)
        {
            return monedasDAO.ObtenerPorId(monedaId);
        }

    }
}
