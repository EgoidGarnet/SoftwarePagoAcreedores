using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class PropuestaPagoTest
    {
        private PropuestasPagoBO propuestaPagoBo = new PropuestasPagoBO();

        [TestMethod]
        public void listarConFiltros()
        {
            List<propuestasPagoDTO> propuestas = propuestaPagoBo.ListarConFiltros(1,1, "APROBADA").ToList();

            Assert.IsNotNull(propuestas);

            foreach(var p in propuestas)
            {
                Console.WriteLine(p.fecha_hora_creacion);
            }

        }

        


        

    }
}
