using Conde.Kirkman;
using Xunit;

namespace Conde.Kirkman.Testes;

public class BuscaEPasseioTestes
{
    [Theory]
    [InlineData(3, 1)]
    [InlineData(4, 0)]
    [InlineData(5, 0)]
    [InlineData(6, 0)]
    [InlineData(7, 30)]
    [InlineData(8, 0)]
    [InlineData(9, 840)]
    [InlineData(10, 0)]
    public void AQuantidadeDeSistemasRotulados(int pontos, long esperado)
    {
        Assert.Equal(esperado, new Busca(pontos).Contar());
    }

    [Fact]
    public void ABuscaAchaSistemaExatamenteOndeACondicaoPrevê()
    {
        for (var v = 3; v <= 15; v++)
        {
            var achou = new Busca(v).Primeiro() is not null;
            Assert.Equal(Sistema.CondicaoDeKirkman(v), achou);
        }
    }

    [Fact]
    public void TodoSistemaAchadoPassaNoJuiz()
    {
        foreach (var v in new[] { 3, 7, 9, 13, 15 })
        {
            var sistema = new Busca(v).Primeiro();
            Assert.NotNull(sistema);
            Assert.True(sistema!.Valido());
            Assert.Equal(Sistema.TriosEsperados(v), sistema.Trios.Count);
        }
    }

    [Fact]
    public void TodosOsSistemasEnumeradosSaoValidosEDiferentes()
    {
        foreach (var v in new[] { 7, 9 })
        {
            var todos = Simetrias.Todos(v);
            Assert.All(todos, s => Assert.True(s.Valido()));
            Assert.Equal(todos.Count, todos.Select(s => s.Chave()).Distinct().Count());
        }
    }

    [Fact]
    public void AsDuasEnumeracoesContamOMesmo()
    {
        foreach (var v in new[] { 3, 7, 9 })
        {
            Assert.Equal(new Busca(v).Contar(), Simetrias.Todos(v).Count);
        }
    }

    [Theory]
    [InlineData(3, 6)]
    [InlineData(7, 168)]
    [InlineData(9, 432)]
    public void AsSimetriasDeUmSistema(int pontos, long esperado)
    {
        var sistema = Simetrias.Todos(pontos)[0];
        Assert.Equal(esperado, Simetrias.Quantas(sistema));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(9)]
    public void SistemasVezesSimetriasDaOFatorial(int pontos)
    {
        var todos = Simetrias.Todos(pontos);
        Assert.Equal(Simetrias.Fatorial(pontos), todos.Count * Simetrias.Quantas(todos[0]));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(9)]
    public void TodosOsSistemasDesseTamanhoSaoOMesmoARenomear(int pontos)
    {
        Assert.Equal(1, Simetrias.Classes(pontos));
    }

    [Fact]
    public void TodasAsSimetriasDeUmSistemaSaoPermutacoesDeVerdade()
    {
        var sistema = Simetrias.Todos(7)[0];
        var chave = sistema.Chave();
        var quantas = 0;
        foreach (var p in Simetrias.Permutacoes(7))
        {
            if (sistema.Permutado(p).Chave() == chave)
            {
                quantas++;
                Assert.Equal(7, p.Distinct().Count());
            }
        }

        Assert.Equal(168, quantas);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(9, 4)]
    [InlineData(15, 7)]
    public void AEscalaDasColegiaisExiste(int meninas, int dias)
    {
        var passeio = new Passeio(meninas);
        Assert.Equal(dias, passeio.Dias);
        var escala = passeio.Montar();
        Assert.NotNull(escala);
        Assert.Equal(dias, escala!.Count);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(15)]
    public void AEscalaEUmSistemaDeSteinerEOsDiasSaoCompletos(int meninas)
    {
        var escala = new Passeio(meninas).Montar();
        Assert.NotNull(escala);
        Assert.True(Passeio.ComoSistema(meninas, escala!).Valido());
        Assert.True(Passeio.DiasCompletos(meninas, escala!));
    }

    [Fact]
    public void NaEscalaNinguemAndaDuasVezesComAMesmaPessoa()
    {
        var escala = new Passeio(15).Montar();
        Assert.NotNull(escala);

        var juntas = new HashSet<(int, int)>();
        foreach (var dia in escala!)
        {
            foreach (var (a, b, c) in dia)
            {
                Assert.True(juntas.Add((a, b)));
                Assert.True(juntas.Add((a, c)));
                Assert.True(juntas.Add((b, c)));
            }
        }

        Assert.Equal(15 * 14 / 2, juntas.Count);
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Busca(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Passeio(2));
        Assert.Throws<ArgumentException>(() => new Passeio(10));
        Assert.Throws<ArgumentNullException>(() => Simetrias.Quantas(null!));
    }
}
