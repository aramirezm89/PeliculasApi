namespace PeliculasApi.Entidades
{
    public class PeliculasCines
    {
        public int CineId { get; set; }
        public int PeliculaId { get; set; }
        public Cine Cine { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
