using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class CuentasBancariasDTO : EliminableDTOBase
    {
        private int? cuentaBancariaId;
        private String tipoCuenta;
        private String numeroCuenta;
        private String cci;
        private Boolean activa;
        private EntidadesBancariasDTO entidadBancaria;
        private MonedasDTO moneda;

        public CuentasBancariasDTO() : base()
        {
            this.EntidadBancaria = null;
            this.Moneda = null;
            CuentaBancariaId = null;
            TipoCuenta = null;
            NumeroCuenta = null;
            Cci = null;
            Activa = false;
        }

        public CuentasBancariasDTO(int cuentaBancariaId, string tipoCuenta, string cci, bool activa,
                                   string numeroCuenta, MonedasDTO moneda, EntidadesBancariasDTO entidadBancaria): base()
        {
            CuentaBancariaId = cuentaBancariaId;
            TipoCuenta = tipoCuenta;
            Cci = cci;
            Activa = activa;
            NumeroCuenta = numeroCuenta;
            Moneda = new MonedasDTO(moneda);
            EntidadBancaria = new EntidadesBancariasDTO(entidadBancaria);
        }

        public CuentasBancariasDTO(CuentasBancariasDTO other) : base(other)
        {
            this.CuentaBancariaId = other.CuentaBancariaId;
            this.TipoCuenta = other.TipoCuenta;
            this.NumeroCuenta = other.NumeroCuenta;
            this.Cci = other.Cci;
            this.Activa = other.Activa;
            this.EntidadBancaria = new EntidadesBancariasDTO(other.EntidadBancaria);
            this.Moneda = new MonedasDTO(other.Moneda);
        }

        public int? CuentaBancariaId { get => cuentaBancariaId; set => cuentaBancariaId = value; }
        public string TipoCuenta { get => tipoCuenta; set => tipoCuenta = value; }
        public string NumeroCuenta { get => numeroCuenta; set => numeroCuenta = value; }
        public string Cci { get => cci; set => cci = value; }
        public bool Activa { get => activa; set => activa = value; }
        public EntidadesBancariasDTO EntidadBancaria { get => entidadBancaria; set => entidadBancaria = value; }
        public MonedasDTO Moneda { get => moneda; set => moneda = value; }
    }
}
