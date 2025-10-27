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
        private static BindingList<EntidadesBancariasDTO> entidadesBancarias = new BindingList<EntidadesBancariasDTO>();

        static EntidadesBancariasDAOImpl()
        {
            if (entidadesBancarias.Any()) return;

            var paisesDAO = new PaisesDAOImpl();
            var peru = paisesDAO.ObtenerPorId(3);
            var colombia = paisesDAO.ObtenerPorId(2);
            var mexico = paisesDAO.ObtenerPorId(1);

            // CORRECCIÓN: Se añadió la información del país a los datos existentes.
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 1,
                Nombre = "Banco de Crédito del Perú",
                CodigoSwift = "BCPLPEPL",
                Pais = peru,
                FormatoAceptado = "MT101"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 2,
                Nombre = "Interbank",
                CodigoSwift = "BINPPEPL",
                Pais = peru,
                FormatoAceptado = "XML"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO(3, "BBVA Perú", "PDF", "BCONPEPL", peru));
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 3,
                Nombre = "Bancolombia",
                CodigoSwift = "COLOCOBM",
                Pais = colombia,
                FormatoAceptado = "TXT"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 4,
                Nombre = "Banco de Bogotá",
                CodigoSwift = "BBOGCOBB",
                Pais = colombia,
                FormatoAceptado = "CSV"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 5,
                Nombre = "Davivienda",
                CodigoSwift = "DAIMCOBB",
                Pais = colombia,
                FormatoAceptado = "XML"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 6,
                Nombre = "Banco BBVA Colombia",
                CodigoSwift = "BBVACOBB",
                Pais = colombia,
                FormatoAceptado = "MT101"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 7,
                Nombre = "BBVA México",
                CodigoSwift = "BCMRMXMM",
                Pais = mexico,
                FormatoAceptado = "XML"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 8,
                Nombre = "Banco Santander México",
                CodigoSwift = "BMSXMXMM",
                Pais = mexico,
                FormatoAceptado = "TXT"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 9,
                Nombre = "Banorte",
                CodigoSwift = "MENOMXMT",
                Pais = mexico,
                FormatoAceptado = "CSV"
            });
            entidadesBancarias.Add(new EntidadesBancariasDTO
            {
                EntidadBancariaId = 10,
                Nombre = "Citibanamex",
                CodigoSwift = "BNMXMXMM",
                Pais = mexico,
                FormatoAceptado = "MT101"
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
