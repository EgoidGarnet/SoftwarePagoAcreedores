using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class AcreedoresDAOImpl : AcreedoresDAO
    {
        private BindingList<AcreedoresDTO> acreedores = new BindingList<AcreedoresDTO>();

        public AcreedoresDAOImpl()
        {
            acreedores.Add(new AcreedoresDTO {
                AcreedorId = 1,
                RazonSocial = "Acreedor Uno",
                Ruc = "123456789",
                DireccionFiscal = "Calle 1",
                Condicion = "Contado",
                PlazoDePago = 30,
                Activo = true,
                Pais = new PaisesDTO(1, "Mexico", "MX", "+56")
            });
            acreedores.Add(new AcreedoresDTO {
                AcreedorId = 2,
                RazonSocial = "Acreedor Dos",
                Ruc = "123456789",
                DireccionFiscal = "Calle 2",
                Condicion = "Contado",
                PlazoDePago = 45,
                Activo = true,
                Pais = new PaisesDTO(2, "Colombia", "CO", "+50")
            });
        }

        public int insertar(AcreedoresDTO acreedor)
        {
            if (acreedor.AcreedorId == null || acreedores.Any(a => a.AcreedorId == acreedor.AcreedorId))
                return 0;
            acreedores.Add(acreedor);
            return 1;
        }
        public int modificar(AcreedoresDTO acreedor)
        {
            var existing = acreedores.FirstOrDefault(a => a.AcreedorId == acreedor.AcreedorId);
            if (existing == null)
                return 0;
            int idx = acreedores.IndexOf(existing);
            acreedores[idx] = acreedor;
            return 1;
        }
        public int eliminar(AcreedoresDTO acreedor)
        {
            var existing = acreedores.FirstOrDefault(a => a.AcreedorId == acreedor.AcreedorId);
            if (existing == null)
                return 0;
            acreedores.Remove(existing);
            return 1;
        }
        public int eliminarLogico(AcreedoresDTO acreedor)
        {
            var existing = acreedores.FirstOrDefault(a => a.AcreedorId == acreedor.AcreedorId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = acreedor.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public AcreedoresDTO obtenerPorId(int acreedorId)
        {
            return acreedores.FirstOrDefault(a => a.AcreedorId == acreedorId);
        }
        public IList<AcreedoresDTO> ListarTodos()
        {
            return acreedores;
        }
    }
}
