using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.PaisesWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SoftPac.Business.Tests
{
    [TestClass]
    public class PaisesBOTests
    {

        [TestMethod]
        public void ListarTodos_DebeRetornarBindingListConPaises()
        {
            var paisesBO = new PaisesBO();

            // Act: llamamos al método que queremos probar
            BindingList<paisesDTO> resultado = paisesBO.ListarTodos();

            foreach (paisesDTO pais in resultado)
            {
                Console.WriteLine(pais.pais_id + "-" + pais.nombre + "-" + pais.codigo_iso + "-" + pais.codigo_telefonico);
            }

        }


    }
}