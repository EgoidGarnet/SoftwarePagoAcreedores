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
    public class PaisesBO
    {
        PaisesDAO paisesDAO;

        public PaisesBO()
        {
            this.paisesDAO = new PaisesDAOImpl();
        }
        public BindingList<PaisesDAO> ListarTodos()
        {
            return (BindingList<PaisesDAO>)this.paisesDAO.ListarTodos();
        }
    }
}
