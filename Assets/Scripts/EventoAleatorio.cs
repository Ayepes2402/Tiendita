using System;


public class EventoAleatorio
{
    private Random random;

    public EventoAleatorio()
    {
        random = new Random();
    }

    public TipoEvento GenerarEvento()
    {
        int valor = random.Next(0, 3);

        if (valor == 0)
            return TipoEvento.Robo;

        if (valor == 1)
            return TipoEvento.Propina;

        return TipoEvento.Nada;
    }
}