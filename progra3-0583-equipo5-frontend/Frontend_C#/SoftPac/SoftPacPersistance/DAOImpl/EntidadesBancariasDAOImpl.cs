using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class EntidadesBancariasDAOImpl : EntidadesBancariasDAO
    {
        private BindingList<EntidadesBancariasDTO> entidadesBancarias = new BindingList<EntidadesBancariasDTO>();

        public EntidadesBancariasDAOImpl()
        {
            entidadesBancarias.Add(new EntidadesBancariasDTO {
                EntidadBancariaId = 1,
                Nombre = "Banco Uno",
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO {
                EntidadBancariaId = 2,
                Nombre = "Banco Dos",
            });
        }

        public int Insertar(EntidadesBancariasDTO entidadBancaria)
        {
            if (entidadBancaria.EntidadBancariaId == null || entidadesBancarias.Any(e => e.EntidadBancariaId == entidadBancaria.EntidadBancariaId))
                return 0;
            entidadesBancarias.Add(entidadBancaria);
            return 1;
        }
        public int Modificar(EntidadesBancariasDTO entidadBancaria)
        {
            var existing = entidadesBancarias.FirstOrDefault(e => e.EntidadBancariaId == entidadBancaria.EntidadBancariaId);
            if (existing == null)
                return 0;
            int idx = entidadesBancarias.IndexOf(existing);
            entidadesBancarias[idx] = entidadBancaria;
            return 1;
        }
        public int Eliminar(EntidadesBancariasDTO entidadBancaria)
        {
            var existing = entidadesBancarias.FirstOrDefault(e => e.EntidadBancariaId == entidadBancaria.EntidadBancariaId);
            if (existing == null)
                return 0;
            entidadesBancarias.Remove(existing);
            return 1;
        }
        public EntidadesBancariasDTO ObtenerPorId(int entidadBancariaId)
        {
            return entidadesBancarias.FirstOrDefault(e => e.EntidadBancariaId == entidadBancariaId);
        }
        public IList<EntidadesBancariasDTO> ListarTodos()
        {
            return entidadesBancarias.ToList();
        }
    }
}
