public class Day
{
    public Day(int id) {
        this.id = id;
        weather = Tiempo.vacio;
    }

    public int id;
    
    public enum Tiempo { vacio, sol, nublado, lluvia } //Los estados posibles en los que puede estar el clima.

    public Tiempo weather;
}
