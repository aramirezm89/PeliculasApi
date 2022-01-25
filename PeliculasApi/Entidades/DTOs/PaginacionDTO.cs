namespace PeliculasApi.Entidades.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        public int recordsPorPagina = 10;
        private readonly int cantidadMaximaPorPagina = 50;

        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value  > cantidadMaximaPorPagina)? cantidadMaximaPorPagina:value; }
        }
    }
}
