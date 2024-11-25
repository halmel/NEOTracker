public class Asteroid
{
        public string NeoReferenceId { get; set; }
        public string Name { get; set; }
        public string NasaJplUrl { get; set; }
        public double AbsoluteMagnitudeH { get; set; }
        public double EstimatedDiameterMinKm { get; set; }
        public double EstimatedDiameterMaxKm { get; set; }
        public double AverageDiameterKm => (EstimatedDiameterMinKm + EstimatedDiameterMaxKm) / 2.0f;  
    public string EstimatedDiameterDisplay => AverageDiameterKm.ToString();

        public bool IsPotentiallyHazardous { get; set; }
        public string CloseApproachDate { get; set; }
        public string CloseApproachDateFull { get; set; }
        public double RelativeVelocityKmPerSec { get; set; }
        public double MissDistanceKm { get; set; }
        public string OrbitingBody { get; set; }


         public string SizeIcon
    {
        get
        {
            if (AverageDiameterKm < 0.005f) return "car_icon.png";              if (AverageDiameterKm < 0.05f) return "bus_icon.png";              if (AverageDiameterKm < 0.5f) return "house_icon.png";              if (AverageDiameterKm < 1.0f) return "skyscraper_icon.png";              if (AverageDiameterKm < 2.0f) return "village_icon.png";              return "city_icon.png";          }
    }

    public int SizeIconWidth
    {
        get
        {
                         if (SizeIcon == "car_icon.png")
                return (int)(AverageDiameterKm * 2000);              if (SizeIcon == "bus_icon.png")
                return (int)(AverageDiameterKm * 1000);              if (SizeIcon == "house_icon.png")
                return (int)(AverageDiameterKm * 500);               if (SizeIcon == "skyscraper_icon.png")
                return (int)(AverageDiameterKm * 250);               if (SizeIcon == "village_icon.png")
                return (int)(AverageDiameterKm * 125);               return (int)(AverageDiameterKm * 50);           }
    }

    public int SizeIconHeight
    {
        get
        {
                         return SizeIconWidth;          }
    }

    public double MissDistanceScaledPosition
    {
        get
        {
                         const double moonDistanceKm = 384400;
            return (MissDistanceKm / moonDistanceKm) * 300;          }
    }
    public double MissDistanceOffset
    {
        get
        {
                         const double minDistance = 1000000;               const double maxDistance = 20000000;              const double maxOffset = 150;  
                         double logMin = Math.Log(minDistance);               double logMax = Math.Log(maxDistance);              double logDistance = Math.Log(MissDistanceKm);  
                         double normalizedDistance = (logDistance - logMin) / (logMax - logMin);

                         return Math.Max(0, Math.Min(normalizedDistance * maxOffset, maxOffset));
        }
    }
}
