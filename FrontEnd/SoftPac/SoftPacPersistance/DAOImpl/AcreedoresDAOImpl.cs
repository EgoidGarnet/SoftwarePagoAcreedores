// SoftPac.Persistance.DAOImpl.AcreedoresDAOImpl.cs
using SoftPac.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

namespace SoftPac.Persistance.DAOImpl
{
    public class AcreedoresDAOImpl
    {
        private static BindingList<AcreedoresDTO> acreedores = new BindingList<AcreedoresDTO>();
        private static int NextId = 3;

        static AcreedoresDAOImpl()
        {
            if (acreedores.Count > 0) return;

            acreedores.Add(new AcreedoresDTO
            {
                AcreedorId = 1,
                RazonSocial = "Acreedor Uno",
                Ruc = "123456789",
                DireccionFiscal = "Calle 1",
                Condicion = "Contado",
                PlazoDePago = 30,
                Activo = true,
                Pais = new PaisesDTO(1, "Mexico", "MX", "+56"),
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });

            acreedores.Add(new AcreedoresDTO
            {
                AcreedorId = 2,
                RazonSocial = "Acreedor Dos",
                Ruc = "123456789",
                DireccionFiscal = "Calle 2",
                Condicion = "Contado",
                PlazoDePago = 45,
                Activo = true,
                Pais = new PaisesDTO(2, "Colombia", "CO", "+50"),
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
        }

        public int insertar(AcreedoresDTO acreedor)
        {
            if (acreedor == null) return 0;

            if (acreedor.AcreedorId == null || acreedor.AcreedorId <= 0)
                acreedor.AcreedorId = NextId++;
            else if (acreedores.Any(a => a.AcreedorId == acreedor.AcreedorId))
                return 0;

            acreedores.Add(acreedor);
            return 1;
        }

        public int modificar(AcreedoresDTO acreedor)
        {
            if (acreedor == null || acreedor.AcreedorId == null) return 0;

            AcreedoresDTO existing = acreedores.FirstOrDefault(a => a.AcreedorId == acreedor.AcreedorId);
            if (existing == null) return 0;

            existing.RazonSocial = acreedor.RazonSocial;
            existing.Ruc = acreedor.Ruc;
            existing.DireccionFiscal = acreedor.DireccionFiscal;
            existing.Condicion = acreedor.Condicion;
            existing.PlazoDePago = acreedor.PlazoDePago;
            existing.Activo = acreedor.Activo;
            existing.Pais = acreedor.Pais;

            return 1;
        }

        public int eliminarLogico(AcreedoresDTO acreedor)
        {
            if (acreedor == null || acreedor.AcreedorId == null) return 0;

            AcreedoresDTO existing = acreedores.FirstOrDefault(a => a.AcreedorId == acreedor.AcreedorId);
            if (existing == null) return 0;

            if (existing.Activo)
            {
                existing.Activo = false;
                existing.UsuarioEliminacion = acreedor.UsuarioEliminacion;
                existing.FechaEliminacion = DateTime.Now;
            }
            else
            {
                existing.Activo = true;
                existing.UsuarioEliminacion = null;
                existing.FechaEliminacion = null;
            }
            return 1;
        }

        public AcreedoresDTO obtenerPorId(int acreedorId)
        {
            return acreedores.FirstOrDefault(a => a.AcreedorId == acreedorId);
        }

        public IList<AcreedoresDTO> ListarTodos()
        {
            return new BindingList<AcreedoresDTO>(acreedores.ToList());
        }
    }
}
