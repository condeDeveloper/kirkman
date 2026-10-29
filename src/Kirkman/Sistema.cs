namespace Conde.Kirkman;

/// <summary>
/// Um sistema triplo de Steiner sobre <c>v</c> pontos: uma lista de trios tal
/// que <b>cada par de pontos aparece em exatamente um trio</b>.
///
/// Essa é a definição inteira, e é ela que esta classe confere — nada de
/// contagem esperta, nada de construção. É o juiz do repositório.
///
/// Da definição sai, sem esforço, o número de trios: cada trio cobre 3 pares e
/// há <c>v(v−1)/2</c> pares, então são <c>v(v−1)/6</c> trios. Que esse número
/// seja inteiro já exclui metade dos tamanhos.
/// </summary>
public sealed class Sistema
{
    private readonly (int A, int B, int C)[] _trios;

    public int Pontos { get; }

    public IReadOnlyList<(int A, int B, int C)> Trios => _trios;

    public Sistema(int pontos, IEnumerable<(int A, int B, int C)> trios)
    {
        ArgumentNullException.ThrowIfNull(trios);
        ArgumentOutOfRangeException.ThrowIfNegative(pontos);
        Pontos = pontos;
        _trios = trios.Select(Ordenar).ToArray();
    }

    private static (int A, int B, int C) Ordenar((int A, int B, int C) t)
    {
        var lista = new[] { t.A, t.B, t.C };
        Array.Sort(lista);
        return (lista[0], lista[1], lista[2]);
    }

    public static int TriosEsperados(int pontos)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pontos);
        return pontos * (pontos - 1) / 6;
    }

    /// <summary>
    /// A condição necessária conhecida: um sistema só pode existir quando
    /// <c>v ≡ 1</c> ou <c>v ≡ 3 (mod 6)</c>. Ela está aqui para ser comparada
    /// com a busca, não para guiá-la — a busca não a consulta.
    /// </summary>
    public static bool CondicaoDeKirkman(int pontos) => pontos % 6 is 1 or 3;

    /// <summary>
    /// Todo par aparece em exatamente um trio? Devolve o par que falha, ou
    /// nulo quando o sistema é válido.
    /// </summary>
    public (int A, int B, int Quantos)? ParQueFalha()
    {
        var conta = new int[Pontos, Pontos];
        foreach (var (a, b, c) in _trios)
        {
            if (a == b || b == c || a < 0 || c >= Pontos)
            {
                return (a, c, -1);
            }

            conta[a, b]++;
            conta[a, c]++;
            conta[b, c]++;
        }

        for (var a = 0; a < Pontos; a++)
        {
            for (var b = a + 1; b < Pontos; b++)
            {
                if (conta[a, b] != 1)
                {
                    return (a, b, conta[a, b]);
                }
            }
        }

        return null;
    }

    public bool Valido() => ParQueFalha() is null;

    /// <summary>
    /// Quantos trios passam por cada ponto. Da definição sai que são sempre
    /// <c>(v−1)/2</c>, igual para todos — e o sistema que não cumpre isso não é
    /// sistema.
    /// </summary>
    public int[] TriosPorPonto()
    {
        var conta = new int[Pontos];
        foreach (var (a, b, c) in _trios)
        {
            conta[a]++;
            conta[b]++;
            conta[c]++;
        }

        return conta;
    }

    /// <summary>O sistema com os pontos trocados por uma permutação.</summary>
    public Sistema Permutado(IReadOnlyList<int> permutacao)
    {
        ArgumentNullException.ThrowIfNull(permutacao);
        return new Sistema(Pontos, _trios.Select(t => (permutacao[t.A], permutacao[t.B], permutacao[t.C])));
    }

    /// <summary>A forma canônica para comparar: os trios ordenados entre si.</summary>
    public string Chave()
    {
        var textos = _trios.Select(t => $"{t.A},{t.B},{t.C}").ToList();
        textos.Sort(StringComparer.Ordinal);
        return string.Join("|", textos);
    }

    public override string ToString() => string.Join(" ", _trios.Select(t => $"{{{t.A}{t.B}{t.C}}}"));
}
