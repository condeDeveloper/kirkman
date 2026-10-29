using Conde.Kirkman;
using Xunit;

namespace Conde.Kirkman.Testes;

public class SistemaTestes
{
    /// <summary>O plano de Fano: sete pontos, sete retas, cada par numa só.</summary>
    private static Sistema Fano() => new(7,
    [
        (0, 1, 2), (0, 3, 4), (0, 5, 6),
        (1, 3, 5), (1, 4, 6), (2, 3, 6), (2, 4, 5),
    ]);

    [Fact]
    public void OPlanoDeFanoEUmSistemaValido()
    {
        var fano = Fano();
        Assert.True(fano.Valido());
        Assert.Null(fano.ParQueFalha());
        Assert.Equal(7, fano.Trios.Count);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(7, 7)]
    [InlineData(9, 12)]
    [InlineData(13, 26)]
    [InlineData(15, 35)]
    public void AQuantidadeDeTriosSaiDaDefinicao(int pontos, int esperado)
    {
        Assert.Equal(esperado, Sistema.TriosEsperados(pontos));
    }

    [Fact]
    public void CadaPontoEstaNoMesmoNumeroDeTrios()
    {
        var conta = Fano().TriosPorPonto();
        Assert.All(conta, q => Assert.Equal(3, q));
        Assert.Equal((7 - 1) / 2, conta[0]);
    }

    [Fact]
    public void UmParRepetidoEDetectado()
    {
        var torto = new Sistema(7,
        [
            (0, 1, 2), (0, 1, 3), (0, 5, 6),
            (1, 4, 5), (2, 3, 5), (2, 4, 6), (3, 4, 6),
        ]);

        Assert.False(torto.Valido());
        Assert.NotNull(torto.ParQueFalha());
    }

    [Fact]
    public void UmParQueFaltaEDetectado()
    {
        var incompleto = new Sistema(7, [(0, 1, 2), (0, 3, 4)]);
        var falha = incompleto.ParQueFalha();
        Assert.NotNull(falha);
        Assert.Equal(0, falha!.Value.Quantos);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(6, false)]
    [InlineData(7, true)]
    [InlineData(9, true)]
    [InlineData(13, true)]
    [InlineData(15, true)]
    [InlineData(12, false)]
    public void ACondicaoConhecida(int pontos, bool esperado)
    {
        Assert.Equal(esperado, Sistema.CondicaoDeKirkman(pontos));
    }

    [Fact]
    public void OTrioEGuardadoEmOrdem()
    {
        var sistema = new Sistema(5, [(3, 1, 2)]);
        Assert.Equal((1, 2, 3), sistema.Trios[0]);
    }

    [Fact]
    public void PermutarOsPontosNaoEstragaOSistema()
    {
        var fano = Fano();
        foreach (var p in new[]
        {
            new[] { 1, 2, 3, 4, 5, 6, 0 },
            new[] { 6, 5, 4, 3, 2, 1, 0 },
            new[] { 0, 2, 1, 4, 3, 6, 5 },
        })
        {
            Assert.True(fano.Permutado(p).Valido());
        }
    }

    [Fact]
    public void AChaveNaoDependeDaOrdemDosTrios()
    {
        var a = new Sistema(7, [(0, 1, 2), (0, 3, 4), (0, 5, 6), (1, 3, 5), (1, 4, 6), (2, 3, 6), (2, 4, 5)]);
        var b = new Sistema(7, [(2, 4, 5), (1, 4, 6), (0, 5, 6), (0, 1, 2), (2, 3, 6), (1, 3, 5), (0, 3, 4)]);
        Assert.Equal(a.Chave(), b.Chave());
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentNullException>(() => new Sistema(7, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Sistema(-1, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => Sistema.TriosEsperados(-1));
        Assert.Throws<ArgumentNullException>(() => Fano().Permutado(null!));
    }
}
