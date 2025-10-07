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

    public int CharacterId { get; set; }

    public int Reward { get; set; }
}
