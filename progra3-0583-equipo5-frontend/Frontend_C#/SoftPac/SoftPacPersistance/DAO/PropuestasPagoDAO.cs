using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface PropuestasPagoDAO
    {
        int Insertar(PropuestasPagoDTO propuestaPago);
        int Modificar(PropuestasPagoDTO propuestaPago);
        int Eliminar(PropuestasPagoDTO propuestaPago);
        int EliminarLogico(PropuestasPagoDTO propuestaPago);
        PropuestasPagoDTO ObtenerPorId(int propuestaPagoId);
        IList<PropuestasPagoDTO> ListarTodos();
        IList<PropuestasPagoDTO> ListarTodaActividad(); // <-- AÑADIR ESTA LÍNEA

    }
}
