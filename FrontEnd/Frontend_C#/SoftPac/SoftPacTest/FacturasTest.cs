using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.FacturasWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class FacturasTest
    {
        private FacturasBO facturasBO = new FacturasBO();
        [TestMethod]
        public void listarConFiltrosTest()
        {
            List<facturasDTO> facturasPend = facturasBO.ListarPendientesPorCriterios(1, DateTime.Parse("2024-12-15 23:59:59")).ToList();

            Console.WriteLine(DateTime.Parse("2024-12-15 23:59:59"));
            foreach(var f in facturasPend)
            {
                Console.WriteLine(f.numero_factura);
            }

        }
    }
}
