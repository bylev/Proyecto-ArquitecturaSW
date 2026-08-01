using TransGGP.Application.Services;
using TransGGP.Domain.Models;
using TransGGP.xUnit.Fakes;

namespace TransGGP.xUnit.Services;

public class OperadorServiceTest
{
    [Fact]
    public void RegistrarOperador_AgregaOperadorCorrectamente()
    {
        // Arrange
        var repositorio = new FakeOperadorRepository();
        var servicio = new OperadorService(repositorio);

        var operador = new Operador
        {
            Id = 1,
            NumeroOperador = "OP-001",
            Nombre = "Juan Pérez"
        };

        // Act
        var resultado = servicio.RegistrarOperador(operador);

        // Assert
        Assert.Same(operador, resultado);
        Assert.Single(repositorio.Operadores);
        Assert.Equal("Juan Pérez", repositorio.Operadores[0].Nombre);
        Assert.Equal("OP-001", repositorio.Operadores[0].NumeroOperador);
    }

    [Fact]
    public void ObtenerPorId_CuandoExiste_RetornaOperador()
    {
        // Arrange
        var repositorio = new FakeOperadorRepository();

        repositorio.Operadores.Add(new Operador
        {
            Id = 5,
            NumeroOperador = "OP-005",
            Nombre = "Carlos López"
        });

        var servicio = new OperadorService(repositorio);

        // Act
        var resultado = servicio.ObtenerPorId(5);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(5, resultado.Id);
        Assert.Equal("Carlos López", resultado.Nombre);
        Assert.Equal("OP-005", resultado.NumeroOperador);
    }

    [Fact]
    public void EliminarOperador_EliminaOperadorExistente()
    {
        // Arrange
        var repositorio = new FakeOperadorRepository();

        repositorio.Operadores.Add(new Operador
        {
            Id = 10,
            NumeroOperador = "OP-010",
            Nombre = "Operador a eliminar"
        });

        var servicio = new OperadorService(repositorio);

        // Act
        servicio.EliminarOperador(10);

        // Assert
        Assert.Empty(repositorio.Operadores);
        Assert.Null(servicio.ObtenerPorId(10));
    }
}