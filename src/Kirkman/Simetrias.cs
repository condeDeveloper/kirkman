namespace Conde.Kirkman;

/// <summary>
/// Quantas maneiras existem de renomear os pontos sem mudar o sistema.
///
/// A conta que interessa é esta: <c>(sistemas rotulados) × (simetrias de um
/// deles) = v!</c> quando todos os sistemas são o mesmo a menos de nome. Ela
/// vem da órbita-estabilizador e serve de juiz cruzado entre duas medidas que
/// não se falam — a contagem da busca e a contagem das simetrias.
/// </summary>
public static class Simetrias
{
    /// <summary>Quantas permutações dos pontos devolvem o mesmo sistema.</summary>
    public static long Quantas(Sistema sistema)
    {
        ArgumentNullException.ThrowIfNull(sistema);
        var chave = sistema.Chave();
        long quantas = 0;

        foreach (var permutacao in Permutacoes(sistema.Pontos))
        {
            if (sistema.Permutado(permutacao).Chave() == chave)
            {
                quantas++;
            }
        }

        return quantas;
    }

    /// <summary>Quantas classes de sistemas existem a menos de renomear pontos.</summary>
    public static int Classes(int pontos)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pontos);
        var busca = new Busca(pontos);
        var primeiro = busca.Primeiro();
        if (primeiro is null)
        {
            return 0;
        }

        var vistos = new HashSet<string>();
        var classes = 0;

        foreach (var sistema in Todos(pontos))
        {
            if (vistos.Contains(sistema.Chave()))
            {
                continue;
            }

            classes++;
            foreach (var permutacao in Permutacoes(pontos))
            {
                vistos.Add(sistema.Permutado(permutacao).Chave());
            }
        }

        return classes;
    }

    /// <summary>Todos os sistemas rotulados de um tamanho.</summary>
    public static List<Sistema> Todos(int pontos)
    {
        var achados = new List<Sistema>();
        var coberto = new bool[pontos, pontos];
        var atual = new List<(int, int, int)>();

        Passo();
        return achados;

        void Passo()
        {
            var par = Descoberto();
            if (par is null)
            {
                achados.Add(new Sistema(pontos, atual));
                return;
            }

            var (a, b) = par.Value;
            for (var c = 0; c < pontos; c++)
            {
                if (c == a || c == b || coberto[Math.Min(a, c), Math.Max(a, c)] || coberto[Math.Min(b, c), Math.Max(b, c)])
                {
                    continue;
                }

                Marcar(a, b, c, true);
                Passo();
                Marcar(a, b, c, false);
            }
        }

        (int A, int B)? Descoberto()
        {
            for (var a = 0; a < pontos; a++)
            {
                for (var b = a + 1; b < pontos; b++)
                {
                    if (!coberto[a, b])
                    {
                        return (a, b);
                    }
                }
            }

            return null;
        }

        void Marcar(int a, int b, int c, bool ligar)
        {
            coberto[Math.Min(a, b), Math.Max(a, b)] = ligar;
            coberto[Math.Min(a, c), Math.Max(a, c)] = ligar;
            coberto[Math.Min(b, c), Math.Max(b, c)] = ligar;
            if (ligar)
            {
                atual.Add((a, b, c));
            }
            else
            {
                atual.RemoveAt(atual.Count - 1);
            }
        }
    }

    public static IEnumerable<int[]> Permutacoes(int n)
    {
        var atual = new int[n];
        var usado = new bool[n];
        return Passo(0);

        IEnumerable<int[]> Passo(int i)
        {
            if (i == n)
            {
                yield return atual;
                yield break;
            }

            for (var v = 0; v < n; v++)
            {
                if (usado[v])
                {
                    continue;
                }

                usado[v] = true;
                atual[i] = v;
                foreach (var saida in Passo(i + 1))
                {
                    yield return saida;
                }

                usado[v] = false;
            }
        }
    }

    public static long Fatorial(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        long resultado = 1;
        for (var i = 2; i <= n; i++)
        {
            resultado *= i;
        }

        return resultado;
    }
}
