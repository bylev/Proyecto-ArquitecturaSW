using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.xUnit.Fakes;

public class FakeOperadorRepository : IOperadorRepository
{
    public List<Operador> Operadores { get; } = new();

    public List<Operador> ObtenerTodos()
    {
        return Operadores;
    }

    public Operador? ObtenerPorId(int id)
    {
        return Operadores.FirstOrDefault(operador => operador.Id == id);
    }

    public void Agregar(Operador operador)
    {
        Operadores.Add(operador);
    }

    public void Actualizar(Operador operador)
    {
        var existente = ObtenerPorId(operador.Id);

        if (existente is not null)
        {
            existente.NumeroOperador = operador.NumeroOperador;
            existente.Nombre = operador.Nombre;
        }
    }

    public void Eliminar(int id)
    {
        var operador = ObtenerPorId(id);

        if (operador is not null)
        {
            Operadores.Remove(operador);
        }
    }
}