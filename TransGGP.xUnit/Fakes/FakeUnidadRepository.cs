using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.xUnit.Fakes;

public class FakeUnidadRepository : IUnidadRepository
{
    public List<Unidad> Unidades { get; } = new();

    public List<Unidad> ObtenerTodos()
    {
        return Unidades;
    }

    public Unidad? ObtenerPorId(int id)
    {
        return Unidades.FirstOrDefault(unidad => unidad.Id == id);
    }

    public void Agregar(Unidad unidad)
    {
        Unidades.Add(unidad);
    }

    public void Actualizar(Unidad unidad)
    {
        var existente = ObtenerPorId(unidad.Id);

        if (existente is not null)
        {
            existente.Clave = unidad.Clave;
            existente.Placa = unidad.Placa;
        }
    }

    public void Eliminar(int id)
    {
        var unidad = ObtenerPorId(id);

        if (unidad is not null)
        {
            Unidades.Remove(unidad);
        }
    }
}