using System.Globalization;
using Servidor;

bool opened = true;
List<string> historico = [];
string historicoLocal = @".\calculos.txt";
CultureInfo.CurrentCulture = new("pt-BR", false);

void SalvaHistoricoLocal(string expressao)
{
  using StreamWriter escritor = new(historicoLocal, true);
  escritor.WriteLine(expressao);
}

void LeHistoricoLocal()
{
  using StreamReader leitor = new(historicoLocal);
  string linha;
  while ((linha = leitor.ReadLine()) != null)
  {
    historico.Add(linha);
  }
}

void LimpaHistoricoLocal()
{
  using StreamWriter escritor = new(historicoLocal, false);
  escritor.Write("");
}

static double? TransformaEmNumero(string valor)
{
  bool transformou = double.TryParse(valor, NumberStyles.Any, CultureInfo.CurrentCulture, out double numero);
  if (!transformou)
  {
    return null;
  }
  return numero;
}

static bool ValidaSelecao(int valor, string[] opcoes)
{
  return valor >= 0 && valor < opcoes.Length;
}

static (int?, bool) TransformaSelecaoEmInt(string selecao)
{
  bool foiConvertido = int.TryParse(selecao, out int conversao);
  if (!foiConvertido)
  {
    return (null, false);
  }
  return (conversao, true);
}

static void DesenhaTitulo()
{
  Console.ForegroundColor = ConsoleColor.DarkGreen;
  Console.WriteLine("==========================\nCalculadora\n==========================");
  Console.ResetColor();
  return;
}

static void MostraOpções(string[] opcoes)
{
  for (int i = 0; i < opcoes.Length; i++)
  {
    Console.Write($"[{i}] {opcoes[i]}     ");
    if (i == 2 && (i + 1) > opcoes.Length)
    {
      Console.Write("\n");
    }
  }
  return;
}

static int? LeSelecao(string[] opcoes)
{
  Console.Write("\n");
  string selecao = Console.ReadLine() ?? string.Empty;
  if (string.IsNullOrWhiteSpace(selecao))
  {
    return null;
  }
  (int? selecaoConvertida, bool foiConvertido) = TransformaSelecaoEmInt(selecao);
  if (!foiConvertido)
  {
    return null;
  }

  bool selecaoValida = false;

  if (selecaoConvertida.HasValue)
  {
    selecaoValida = ValidaSelecao(selecaoConvertida.Value, opcoes);
  }

  if (!selecaoValida)
  {
    return null;
  }

  return selecaoConvertida;
}

static double LeValor(string mensagem)
{
  double? valor = null;
  while (valor == null)
  {
    Console.WriteLine(mensagem);
    valor = TransformaEmNumero(Console.ReadLine());
    if (valor == null)
    {
      Logger.Erro("Número inválido");
      Thread.Sleep(2000);
      Logger.Limpa(4);
      Console.SetCursorPosition(0, 4);
      continue;
    }
  }
  return valor.Value;
}

void LidaComHistorico()
{
  Console.Clear();
  DesenhaTitulo();
  List<string> opcoes = ["limpar", "voltar"];
  if (historico.Count == 0)
  {
    opcoes.Remove("limpar");
  }
  int? selecao = null;
  while (selecao == null)
  {
    if (historico.Count == 0)
    {
      Logger.Aviso("Hitórico vazio");
    }
    else
    {
      historico.ForEach((value) => Console.WriteLine(value));
    }
    MostraOpções([.. opcoes]);
    selecao = LeSelecao([.. opcoes]);
    switch (selecao)
    {
      case 0:
        LimpaHistoricoLocal();
        historico.Clear();
        break;
      case 1:
        return;
      default:
        Logger.Erro("Selecione uma opção válida");
        Thread.Sleep(1500);
        Console.Clear();
        DesenhaTitulo();
        break;
    }
  }
}

static double Calcula(double num1, double num2, int operacao)
{
  return operacao switch
  {
    1 => num1 - num2,
    2 => num1 * num2,
    3 => num1 / num2,
    _ => num1 + num2,
  };
}

void LidaComCalculos()
{
  bool continuar = true;
  string[] operacoes = ["+", "-", "*", "/", "voltar"];
  int? operacaoEscolhida = null;
  double? num1 = null;
  double? num2 = null;

  void MostraSelecoes()
  {
    if (operacaoEscolhida is int operacao)
    {
      Console.Write($"Operação escolhida: {operacoes[operacao]}   ");
    }

    if (num1 is double n1)
    {
      Console.Write($"Primeiro valor: {n1}   ");
    }

    if (num2 is double n2)
    {
      Console.Write($"Segundo valor: {n2}");
    }
    Console.WriteLine();
    return;
  }

  while (continuar)
  {
    while (operacaoEscolhida == null)
    {
      Console.Clear();
      DesenhaTitulo();
      Console.WriteLine("Escolha uma operação:");
      MostraOpções(operacoes);
      operacaoEscolhida = LeSelecao(operacoes);
      if (operacaoEscolhida == null)
      {
        Logger.Erro("Operação inválida");
        Thread.Sleep(2000);
      }
    }

    if (operacaoEscolhida is int operacao && operacoes[operacao] == "voltar")
    {
      continuar = false;
      continue;
    }

    Console.Clear();
    DesenhaTitulo();
    MostraSelecoes();
    Logger.Limpa(4);
    num1 = LeValor("Digite o primeiro valor:");

    while (num2 == null)
    {
      Console.Clear();
      DesenhaTitulo();
      MostraSelecoes();
      num2 = LeValor("Digite o segundo valor:");
      if (operacaoEscolhida == 3 && num2 == 0)
      {
        Logger.Erro("Segundo valor não pode ser 0");
        num2 = null;
        Thread.Sleep(2000);
      }
    }

    Console.Clear();
    DesenhaTitulo();
    MostraSelecoes();

    double resultado = Calcula((double)num1, (double)num2, (int)operacaoEscolhida);
    string expressao = $"{num1} {operacoes[(int)operacaoEscolhida]} {num2} = {resultado.ToString("#.##")}";
    SalvaHistoricoLocal(expressao);
    historico.Add(expressao);
    Logger.Sucesso(expressao);
    Thread.Sleep(2000);
    operacaoEscolhida = null;
    num1 = null;
    num2 = null;
  }
}

LeHistoricoLocal();
while (opened)
{
  Console.Clear();
  DesenhaTitulo();
  string[] menu = ["calcular", "historico", "sair"];
  int? opcaoDoMenuEscolhida = null;
  while (opcaoDoMenuEscolhida == null)
  {
    Console.WriteLine("Escolha uma opção:");
    MostraOpções(menu);
    opcaoDoMenuEscolhida = LeSelecao(menu);
    if (opcaoDoMenuEscolhida == null)
    {
      Logger.Erro("Escolha uma opção válida");
      Thread.Sleep(1500);
      Console.Clear();
      DesenhaTitulo();
    }
  }

  switch (opcaoDoMenuEscolhida)
  {
    case 0:
      LidaComCalculos();
      continue;
    case 1:
      LidaComHistorico();
      continue;
    case 2:
      opened = false;
      Console.Clear();
      Logger.Sucesso("Encerrando...");
      Thread.Sleep(1000);
      continue;
  }
}
