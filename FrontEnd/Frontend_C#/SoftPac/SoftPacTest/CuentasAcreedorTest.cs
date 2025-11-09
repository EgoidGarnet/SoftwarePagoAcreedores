using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.CuentasAcreedorWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class CuentasAcreedorTest
    {
        private CuentasAcreedorBO cuentasAcreedorBO = new CuentasAcreedorBO();

        [TestMethod]
        public void listarTodosTest()
        {
            List<cuentasAcreedorDTO> cuentas = cuentasAcreedorBO.ListarTodos().ToList();

            foreach(var c in cuentas)
            {
                Console.WriteLine(c.numero_cuenta);
            }

        }
    }
}
