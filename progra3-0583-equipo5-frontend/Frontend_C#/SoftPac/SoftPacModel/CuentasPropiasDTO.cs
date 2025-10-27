using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    [Serializable]
    public class CuentasPropiasDTO : CuentasBancariasDTO
    {
        private decimal saldoDisponible;

        public CuentasPropiasDTO() : base()
        {
            SaldoDisponible = 0.0M;
        }

        public CuentasPropiasDTO(int cuentaBancariaId, string tipoCuenta, string cci, bool activa,
                                  string numeroCuenta, MonedasDTO moneda, EntidadesBancariasDTO entidadBancaria,
                                  decimal saldoDisponible)
            : base(cuentaBancariaId, tipoCuenta, cci, activa, numeroCuenta, moneda, entidadBancaria)
        {
            SaldoDisponible = saldoDisponible;
        }

        public CuentasPropiasDTO(CuentasPropiasDTO other) : base(other)
        {
            this.SaldoDisponible = other.SaldoDisponible;
        }

        public decimal SaldoDisponible { get => saldoDisponible; set => saldoDisponible = value; }

    }
}
