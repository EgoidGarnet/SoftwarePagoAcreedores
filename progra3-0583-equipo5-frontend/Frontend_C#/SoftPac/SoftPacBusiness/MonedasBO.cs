using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System.ComponentModel;
using System.Linq;

namespace SoftPac.Business
{
    public class MonedasBO
    {
        private readonly MonedasDAO monedasDAO;

        public MonedasBO()
        {
            this.monedasDAO = new MonedasDAOImpl();
        }

        public BindingList<MonedasDTO> ListarTodos()
        {
            var monedas = this.monedasDAO.ListarTodos();
            return monedas != null ? new BindingList<MonedasDTO>(monedas.ToList()) : new BindingList<MonedasDTO>();
        }

        public MonedasDTO ObtenerPorId(int monedaId)
        {
            return this.monedasDAO.ObtenerPorId(monedaId);
        }
    }
}
