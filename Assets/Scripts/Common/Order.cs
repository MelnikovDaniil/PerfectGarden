using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Order
{
    public string Name { get; set; }

    public PotWithPlant PotWithPlant { get; set; }
    
    public string Description { get; set; }


    public int Reward { get; set; }

    // Move to save info

    //public int Stage { get; set; }

    //public string Pot { get; set; }
    // public PlantInfo Plant { get; set; }

    //public List<CareEvent> waitingCareEvents { get; set; }
}
