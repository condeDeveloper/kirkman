using Conde.Kirkman;
using Xunit;

namespace Conde.Kirkman.Testes;

/// <summary>
/// Os números do README presos aqui. Só entra o que eu vi o medidor imprimir.
/// </summary>
public class MedidasTestes
{
    [Theory]
    [InlineData(3, true, true, 1, 2)]
    [InlineData(4, false, true, 0, 3)]
    [InlineData(5, false, false, 0, 7)]
    [InlineData(6, false, true, 0, 13)]
    [InlineData(7, true, true, 30, 156)]
    [InlineData(8, false, false, 0, 79)]
    [InlineData(9, true, true, 840, 8863)]
    [InlineData(10, false, true, 0, 633)]
    public void ATabelaDaExistencia(int pontos, bool condicao, bool trioInteiro, long sistemas, long nos)
    {
        var l = Medidas.Existencia([pontos])[0];
        Assert.Equal(condicao, l.CondicaoVale);
        Assert.Equal(trioInteiro, l.TrioInteiro);
        Assert.Equal(sistemas, l.Sistemas);
        Assert.Equal(nos, l.Nos);
    }

    [Fact]
    public void ABuscaSoAchaOndeACondicaoVale()
    {
        foreach (var l in Medidas.Existencia([3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15]))
        {
            Assert.Equal(l.CondicaoVale, l.Sistemas != 0);
        }
    }

    [Fact]
    public void TerNumeroInteiroDeTriosNaoBasta()
    {
        // Quatro dos oito tamanhos impossiveis passam na conta obvia e mesmo
        // assim nao tem sistema nenhum.
        var enganam = Medidas.Existencia([4, 5, 6, 8, 10, 11, 12, 14])
            .Where(l => l.TrioInteiro)
            .Select(l => l.Pontos)
            .ToList();

        Assert.Equal([4, 6, 10, 12], enganam);
    }

    [Fact]
    public void ProvarQueNaoHaNadaCustaMaisNoOnzeQueNoDoze()
    {
        var onze = Medidas.Existencia([11])[0];
        var doze = Medidas.Existencia([12])[0];
        Assert.Equal(5036818, onze.Nos);
        Assert.Equal(6331, doze.Nos);
        Assert.True(onze.Nos > doze.Nos * 700);
    }

    [Theory]
    [InlineData(3, 1, 6, 6, 1)]
    [InlineData(7, 30, 168, 5040, 1)]
    [InlineData(9, 840, 432, 362880, 1)]
    public void ATabelaDasSimetrias(int pontos, long sistemas, long simetrias, long fatorial, int classes)
    {
        var l = Medidas.SimetriasTabela([pontos])[0];
        Assert.Equal(sistemas, l.Sistemas);
        Assert.Equal(simetrias, l.SimetriasDeUm);
        Assert.Equal(fatorial, l.Fatorial);
        Assert.Equal(fatorial, l.Produto);
        Assert.Equal(classes, l.Classes);
    }

    [Fact]
    public void OProdutoDaSempreOFatorial()
    {
        foreach (var l in Medidas.SimetriasTabela([3, 7, 9]))
        {
            Assert.Equal(l.Fatorial, l.Produto);
            Assert.Equal(1, l.Classes);
        }
    }

    [Theory]
    [InlineData(3, 1, 2)]
    [InlineData(9, 4, 16)]
    [InlineData(15, 7, 10789596)]
    public void ATabelaDasEscalas(int meninas, int dias, long nos)
    {
        var l = Medidas.Escalas([meninas])[0];
        Assert.Equal(dias, l.Dias);
        Assert.True(l.Conseguiu);
        Assert.Equal(nos, l.Nos);
    }

    [Theory]
    [InlineData(9, 12)]
    [InlineData(15, 35)]
    public void AEscalaPassaNasDuasExigencias(int meninas, int trios)
    {
        var (sistema, dias, quantos) = Medidas.Conferir(meninas);
        Assert.True(sistema);
        Assert.True(dias);
        Assert.Equal(trios, quantos);
    }

    [Fact]
    public void AEscalaDasQuinzeTemSeteDiasDeCincoFilas()
    {
        var linhas = Medidas.Quinze();
        Assert.Equal(7, linhas.Count);
        Assert.StartsWith("dia 1: ABC  DEF  GHI  JKL  MNO", linhas[0]);
        Assert.All(linhas, l => Assert.Equal(5, l.Split("  ").Length));
    }
}
