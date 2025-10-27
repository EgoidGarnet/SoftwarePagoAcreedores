using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    [Serializable]
    public class CuentasAcreedorDTO : CuentasBancariasDTO
    {
        private AcreedoresDTO acreedor;

        public CuentasAcreedorDTO() : base()
        {
            Acreedor = null;
        }

        public CuentasAcreedorDTO(int cuentaBancariaId, string tipoCuenta, string cci, bool activa,
                                  string numeroCuenta, MonedasDTO moneda, EntidadesBancariasDTO entidadBancaria,
                                  AcreedoresDTO acreedor)
            : base(cuentaBancariaId, tipoCuenta, cci, activa, numeroCuenta, moneda, entidadBancaria)
        {
            Acreedor = new AcreedoresDTO(acreedor);
        }

        public CuentasAcreedorDTO(CuentasAcreedorDTO other) : base(other)
        {
            this.Acreedor = new AcreedoresDTO(other.Acreedor);
        }

        public AcreedoresDTO Acreedor { get => acreedor; set => acreedor = value; }

    }
}
