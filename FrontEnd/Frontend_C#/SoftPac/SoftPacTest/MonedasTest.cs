using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.MonedasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class MonedasTest
    {
        private MonedasBO monedasBO = new MonedasBO();

        [TestMethod]
        public void listarTodosTest()
        {
            BindingList<monedasDTO> monedas = monedasBO.ListarTodos();

            foreach (var m in monedas)
            {
                Console.WriteLine(m.nombre);
            }

        }
    }
}
