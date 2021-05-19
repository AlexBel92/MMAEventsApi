using System.Collections.Generic;

namespace MMAEvents.Api.Models
{
    public class FightCardDTO
    {
        public string Name { get; set; }
        public List<FightDTO> Fights { get; set; }
    }
}