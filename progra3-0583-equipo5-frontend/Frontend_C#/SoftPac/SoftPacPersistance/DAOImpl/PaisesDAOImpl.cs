using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class PaisesDAOImpl : PaisesDAO
    {
        private BindingList<PaisesDTO> paises = new BindingList<PaisesDTO>();

        public PaisesDAOImpl()
        {
            paises.Add(new PaisesDTO {
                PaisId = 1,
                Nombre = "Mexico",
                CodigoIso = "MX",
                CodigoTelefonico = "+56"
            });
            paises.Add(new PaisesDTO {
                PaisId = 2,
                Nombre = "Colombia",
                CodigoIso = "CO",
                CodigoTelefonico = "+50"
            });
        }

        public int Insertar(PaisesDTO pais)
        {
            if (pais.PaisId == null || paises.Any(p => p.PaisId == pais.PaisId))
                return 0;
            paises.Add(pais);
            return 1;
        }
        public int Modificar(PaisesDTO pais)
        {
            var existing = paises.FirstOrDefault(p => p.PaisId == pais.PaisId);
            if (existing == null)
                return 0;
            int idx = paises.IndexOf(existing);
            paises[idx] = pais;
            return 1;
        }
        public int Eliminar(PaisesDTO pais)
        {
            var existing = paises.FirstOrDefault(p => p.PaisId == pais.PaisId);
            if (existing == null)
                return 0;
            paises.Remove(existing);
            return 1;
        }
        public PaisesDTO ObtenerPorId(int paisId)
        {
            return paises.FirstOrDefault(p => p.PaisId == paisId);
        }
        public IList<PaisesDTO> ListarTodos()
        {
            return paises;
        }
    }
}
