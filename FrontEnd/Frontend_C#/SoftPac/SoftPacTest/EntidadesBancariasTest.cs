using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.EntidadesBancariasWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class EntidadesBancariasTest
    {
        private EntidadesBancariasBO entidadesBancariasBO = new EntidadesBancariasBO();

        [TestMethod]
        public void ListarTodosTest()
        {
            List<entidadesBancariasDTO> entidades = this.entidadesBancariasBO.ListarTodos().ToList();
            entidadesBancariasDTO en = new entidadesBancariasDTO();
            
            foreach(var e in entidades)
            {
                Console.WriteLine(e.nombre);
            }

        }


    }
}
