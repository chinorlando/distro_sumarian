using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("ingrese el valor de a:");
        if (!int.TryParse(Console.ReadLine(), out int a))
        {
            Console.WriteLine("valor invalido para a");
            return;
        }

        Console.WriteLine("ingrese el valor de b:");
        if (!int.TryParse(Console.ReadLine(), out int b))
        {
            Console.WriteLine("Valor invalido para b");
            return;
        }

        string mensaje = "";
        double resultado = Dividir(a, b, ref mensaje);

        Console.WriteLine($"resultado: {resultado}");
        Console.WriteLine($"mensaje: {mensaje}");

        Console.ReadLine();
    }

    public static double Dividir(int a, int b, ref string mensaje)
    {
        try
        {
            if (a > b)
            {
                mensaje = "operacion exitosa";
                return (double)a / b;
            }
            else if (b > a)
            {
                mensaje = "operacion exitosa";
                return (double)b / a;
            }
            else
            {
                mensaje = "valores iguales";
                return -1;
            }
        }
        catch (Exception ex)
        {
            mensaje = ex.Message;
            return -1;
        }
    }
}