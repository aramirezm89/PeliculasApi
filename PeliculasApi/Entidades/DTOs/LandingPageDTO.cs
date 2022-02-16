using System.Collections.Generic;

namespace PeliculasApi.Entidades.DTOs
{
    public class LandingPageDTO
    {
       public  List<PeliculaDTO> EnCines { get; set; }
        public List<PeliculaDTO> ProximosEstrenos { get; set; }
    }
}
