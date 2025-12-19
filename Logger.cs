namespace Servidor
{
  public class Logger
  {
    public static void Sucesso(string mensagem)
    {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine(mensagem);
      Console.ForegroundColor = ConsoleColor.Gray;
      return;
    }

    public static void Erro(string mensagem)
    {
      Console.ForegroundColor = ConsoleColor.DarkRed;
      Console.WriteLine(mensagem);
      Console.ForegroundColor = ConsoleColor.Gray;
      return;
    }

    public static void Aviso(string mensagem)
    {
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine(mensagem);
      Console.ForegroundColor = ConsoleColor.Gray;
      return;
    }

    public static void Limpa(int linhaInicio = 0)
    {
      int linhaAtual = Console.CursorTop;
      int colunaAtual = Console.CursorLeft;

      for(int i=linhaInicio; i<Console.WindowHeight;i++)
      {
        Console.SetCursorPosition(0, i);
        Console.Write(new string(' ', Console.WindowWidth));
      }
      Console.SetCursorPosition(colunaAtual, linhaAtual);
      return;
    }
  }
}