using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using SoftPacTest.UsuariosWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using usuarioPaisAccesoDTO = SoftPacBusiness.UsuariosWS.usuarioPaisAccesoDTO;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;

namespace SoftPacTest
{
    [TestClass]
    public class UsuariosTest
    {
        UsuariosBO usuariosBO = new UsuariosBO();


        [TestMethod]
        public void ListarTodosTest()
        {
            List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();
            Assert.IsNotNull(listaUsuarios);
            
            foreach(var l  in listaUsuarios)
            {
                Console.WriteLine(l.nombre_de_usuario);
            }
        }

        [TestMethod]

        public void InsertarUsuario()
        {
            usuariosDTO usuarioNuevo = new usuariosDTO();

            usuarioNuevo.usuario_id = 8;
            usuarioNuevo.usuario_idSpecified = true;

            usuarioNuevo.activo = true;
            usuarioNuevo.activoSpecified = true;

            usuarioNuevo.superusuario = false;
            usuarioNuevo.superusuarioSpecified = true;

            usuarioNuevo.apellidos = "Ramon Bullon";
            usuarioNuevo.nombre = "Mauro";
            usuarioNuevo.nombre_de_usuario = "mauro.bullon";
            usuarioNuevo.correo_electronico = "mauro@gmail.com";
            usuarioNuevo.password_hash = "contrasenha";

            int id = usuariosBO.InsertarUsuario(usuarioNuevo);

            if(id!=0) Console.WriteLine(id);

        }
    }
}
