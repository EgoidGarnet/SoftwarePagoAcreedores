using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class AcreedoresTest
    {
        AcreedoresBO acreedoresBO = new AcreedoresBO();
        [TestMethod]
        public void listarTodos()
        {
            BindingList<acreedoresDTO> acreedores = this.acreedoresBO.ListarTodos();
            foreach(var a in acreedores)
            {
                Console.WriteLine(a.razon_social);
            }
        }
    }
}
