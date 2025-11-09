using SoftPacBusiness.MonedasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SoftPac.Business
{
    public class MonedasBO
    {
        private MonedasWSClient monedasClienteSOAP;

        public MonedasBO()
        {
            this.monedasClienteSOAP = new MonedasWSClient();
        }

        public BindingList<monedasDTO> ListarTodos()
        {
            try
            {
                // Llamar al método remoto
                monedasDTO[] monedas = this.monedasClienteSOAP.listarMonedas();

                // Evitar null reference
                if (monedas == null)
                    monedas = Array.Empty<monedasDTO>();

                // Convertir a BindingList para usar en UI o DataBinding
                return new BindingList<monedasDTO>(monedas.ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar monedas.", ex);
            }
        }

        public monedasDTO ObtenerPorId(int monedaId)
        {
            try
            {
                return this.monedasClienteSOAP.obtenerMonedaPorID(monedaId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener moneda por ID.", ex);
            }
        }
    }
}
