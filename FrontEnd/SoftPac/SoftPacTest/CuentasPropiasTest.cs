using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.CuentasPropiasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPacTest
{
    [TestClass]
    public class CuentasPropiasTest
    {
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();
        [TestMethod]
        public void listarTodosTest()
        {
            BindingList<cuentasPropiasDTO> cuentas = cuentasPropiasBO.ListarTodos();

            foreach(var c in cuentas)
            {
                Console.WriteLine(c.numero_cuenta);
            }

        }

        [TestMethod]
        public void insertarCuentaTest()
        {
            cuentasPropiasDTO c = new cuentasPropiasDTO();
            c.activa = true;
            c.entidad_bancaria = new entidadesBancariasDTO();
            c.entidad_bancaria.entidad_bancaria_id = 1;
            c.moneda = new monedasDTO();
            c.moneda.moneda_id = 1;
            c.tipo_cuenta = "Ahorros";
            c.numero_cuenta = "123456785589";
            c.cci = "1113556008770044455";
            c.saldo_disponible = 5000000;

            c.cuenta_bancaria_idSpecified = true;
            c.entidad_bancaria.entidad_bancaria_idSpecified = true;
            c.moneda.moneda_idSpecified = true;
            c.activaSpecified = true;
            c.saldo_disponibleSpecified = true;

            int resultado = cuentasPropiasBO.Insertar(c);

            if(resultado != 0)
            Console.WriteLine(resultado);


        }

        [TestMethod]
        public void eliminarCuentaTest()
        {
            cuentasPropiasDTO c = new cuentasPropiasDTO();
            c.cuenta_bancaria_id = 18;
            //c.cuenta_bancaria_idSpecified = true;

            usuariosDTO usuarios = new usuariosDTO();
            usuarios.usuario_id = 1;
            //usuarios.usuario_idSpecified = true;

            int resultado = cuentasPropiasBO.Eliminar(c,usuarios);

            if (resultado != 0)
                Console.WriteLine(resultado);


        }

    }
}
