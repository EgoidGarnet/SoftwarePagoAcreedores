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
using paisesDTO = SoftPacBusiness.UsuariosWS.paisesDTO;
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

            foreach (var l in listaUsuarios)
            {
                Console.WriteLine(l.nombre_de_usuario);
            }
        }

        [TestMethod]

        public void TestInsertarUsuario()
        {
            // Crear el DTO principal
            usuariosDTO usuarioNuevo = new usuariosDTO();

            // Datos del usuario
            //usuarioNuevo.usuario_id = 8;
            //usuarioNuevo.usuario_idSpecified = true;
            usuarioNuevo.activo = true;
            usuarioNuevo.activoSpecified = true;
            usuarioNuevo.superusuario = true;
            usuarioNuevo.superusuarioSpecified = true;

            usuarioNuevo.apellidos = "Random";
            usuarioNuevo.nombre = "Usuario";
            usuarioNuevo.nombre_de_usuario = "usuario";
            usuarioNuevo.correo_electronico = "ramon.mauro@pucp.edu.pe";
            usuarioNuevo.password_hash = "contrasenia";

            // 1. Inicializar el arreglo de accesos (UsuarioPaisAccesoDTO)
            usuarioNuevo.usuario_pais = new usuarioPaisAccesoDTO[2];

            // --- ACCESO 1: PERÚ ---

            // Crear el objeto País
            paisesDTO paisPeru = new paisesDTO();
            paisPeru.pais_id = 1;
            paisPeru.pais_idSpecified = true;
            paisPeru.nombre = "Perú"; // ¡IMPORTANTE! Necesario para el correo

            // Crear el objeto Acceso
            usuarioPaisAccesoDTO accesoPeru = new usuarioPaisAccesoDTO();
            accesoPeru.pais = paisPeru;
            accesoPeru.acceso = true;
            accesoPeru.accesoSpecified = true;

            // Asignar el primer acceso al arreglo
            usuarioNuevo.usuario_pais[0] = accesoPeru;

            // --- ACCESO 2: Mexico ---

            // Crear el objeto País
            paisesDTO paisMexico = new paisesDTO();
            paisMexico.pais_id = 2;
            paisMexico.pais_idSpecified = true;
            paisMexico.nombre = "Mexico"; // ¡IMPORTANTE! Necesario para el correo

            // Crear el objeto Acceso
            usuarioPaisAccesoDTO accesoColombia = new usuarioPaisAccesoDTO();
            accesoColombia.pais = paisMexico;
            accesoColombia.acceso = true;
            accesoColombia.accesoSpecified = true;

            // Asignar el segundo acceso al arreglo
            usuarioNuevo.usuario_pais[1] = accesoColombia;

            // Ejecutar la inserción

            usuariosDTO usuarioActual = new usuariosDTO();
            usuarioActual.correo_electronico = "maurorambull@gmail.com";

            int id = usuariosBO.InsertarUsuario(usuarioNuevo, usuarioActual);

            if (id != 0) Console.WriteLine(id);


        }

        [TestMethod]

        public void TestModificarAccesoUsuario()
        {


            // --- 2. Datos del usuario a modificar ---
            int usuarioId = 8; // ⚠️ Usa un ID existente en tu BD de prueba

            string nuevoNombreUsuario = "usuario_actualizado";
            bool activo = true; // Mantiene activo

            // --- 3. Nueva lista de países ---
            // Ejemplo: cambiamos accesos a Colombia (3)
            List<int> paisesIds = new List<int> { 3 };

            // --- 4. Usuario actual (superusuario que hace la acción) ---
            usuariosDTO usuarioActual = new usuariosDTO();
            usuarioActual.correo_electronico = "maurorambull@gmail.com"; // remitente visible en CC

            // --- 5. Nueva contraseña ---
            string nuevaContrasenha = "contraseniaNueva123";

            // --- 6. Ejecutar la operación ---
            int resultado = usuariosBO.ModificarAccesoUsuario(
                usuarioId,
                nuevoNombreUsuario,
                activo,
                paisesIds,
                usuarioActual,
                nuevaContrasenha
            );

            // --- 7. Mostrar resultado ---
            if (resultado > 0)
            {
                Console.WriteLine("✅ Modificación realizada correctamente. Se debe haber enviado el correo de modificación.");
            }
            else
            {
                Console.WriteLine("❌ La modificación no se realizó o el usuario no existe.");
            }

        }

        [TestMethod]

        public void TestEliminarUsuario()
        {


            int usuarioId = 8; // ID del usuario a eliminar
            int usuarioAdmin = 7;  //ID del usuario administrador que realiza la eliminación
            // ---    Ejecutar la operación ---
            int resultado = usuariosBO.EliminarUsuario(usuarioId, usuarioAdmin);

            // ---    Mostrar resultado ---
            if (resultado > 0)
            {
                Console.WriteLine("✅ Eliminación realizada correctamente. Se debe haber enviado el correo de eliminación.");
            }
            else
            {
                Console.WriteLine("❌ La eliminación no se realizó o el usuario no existe.");
            }

        }


    }
}
