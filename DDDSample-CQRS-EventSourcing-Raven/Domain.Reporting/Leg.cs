using System;
using System.Linq;
using System.Collections.Generic;

namespace DDDSample.Reporting
{
   [Serializable]
   public class Leg
   {
      public string LoadLocation { get; set; }
      public string UnloadLocation { get; set; }
      public DateTime LoadDate { get; set; }
      public DateTime UnloadDate { get; set; }

      public Leg()
      {         
      }

      public Leg(string loadLocation, DateTime loadDate, string unloadLocation, DateTime unloadDate)
      {
         LoadLocation = loadLocation;
         LoadDate = loadDate;
         UnloadLocation = unloadLocation;
         UnloadDate = unloadDate;
      }
   }
}