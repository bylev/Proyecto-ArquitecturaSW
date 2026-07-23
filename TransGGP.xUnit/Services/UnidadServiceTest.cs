using TransGGP.Application.Services;
using TransGGP.Domain.Models;
using TransGGP.xUnit.Fakes;

namespace TransGGP.xUnit.Services;

public class UnidadServiceTest
{
    [Fact]
    public void RegistrarUnidad_AgregaUnidadCorrectamente()
    {
        // Arrange
        var repositorio = new FakeUnidadRepository();
        var servicio = new UnidadService(repositorio);

        var unidad = new Unidad
        {
            Id = 1,
            Clave = "T-01",
            Placa = "ABC-123"
        };

        // Act
        var resultado = servicio.RegistrarUnidad(unidad);

        // Assert
        Assert.Same(unidad, resultado);
        Assert.Single(repositorio.Unidades);
        Assert.Equal("T-01", repositorio.Unidades[0].Clave);
        Assert.Equal("ABC-123", repositorio.Unidades[0].Placa);
    }

    [Fact]
    public void ObtenerPorId_CuandoExiste_RetornaUnidad()
    {
        // Arrange
        var repositorio = new FakeUnidadRepository();

        repositorio.Unidades.Add(new Unidad
        {
            Id = 5,
            Clave = "T-05",
            Placa = "XYZ-456"
        });

        var servicio = new UnidadService(repositorio);

        // Act
        var resultado = servicio.ObtenerPorId(5);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(5, resultado.Id);
        Assert.Equal("T-05", resultado.Clave);
        Assert.Equal("XYZ-456", resultado.Placa);
    }

    [Fact]
    public void EliminarUnidad_EliminaUnidadExistente()
    {
        // Arrange
        var repositorio = new FakeUnidadRepository();

        repositorio.Unidades.Add(new Unidad
        {
            Id = 10,
            Clave = "T-10",
            Placa = "YYY-999"
        });

        var servicio = new UnidadService(repositorio);

        // Act
        servicio.EliminarUnidad(10);

        // Assert
        Assert.Empty(repositorio.Unidades);
        Assert.Null(servicio.ObtenerPorId(10));
    }
}