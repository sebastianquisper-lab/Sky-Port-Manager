using SkyPort.Application.Common.Constants;

namespace SkyPort.Application.UseCases.Vuelos.Rules;

public static class TransicionesEstadoVuelo
{
    public static bool EsValida(string desde, string hasta) => (desde, hasta) switch
    {
        (EstadoVuelo.Programado, EstadoVuelo.Embarcando or EstadoVuelo.Retrasado or EstadoVuelo.Cancelado) => true,
        (EstadoVuelo.Retrasado, EstadoVuelo.Programado or EstadoVuelo.Embarcando or EstadoVuelo.Cancelado) => true,
        (EstadoVuelo.Embarcando, EstadoVuelo.Despegado or EstadoVuelo.Retrasado or EstadoVuelo.Cancelado) => true,
        _ => false
    };
}
