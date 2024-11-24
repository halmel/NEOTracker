public class Asteroid
{
        public string NeoReferenceId { get; set; }
        public string Name { get; set; }
        public string NasaJplUrl { get; set; }
        public double AbsoluteMagnitudeH { get; set; }
        public double EstimatedDiameterMinKm { get; set; }
        public double EstimatedDiameterMaxKm { get; set; }
        public double AverageDiameterKm => (EstimatedDiameterMinKm + EstimatedDiameterMaxKm) / 2.0f; // Computed property

    public string EstimatedDiameterDisplay => AverageDiameterKm.ToString();

        public bool IsPotentiallyHazardous { get; set; }
        public string CloseApproachDate { get; set; }
        public string CloseApproachDateFull { get; set; }
        public double RelativeVelocityKmPerSec { get; set; }
        public double MissDistanceKm { get; set; }
        public string OrbitingBody { get; set; }


    // Calculated properties
    public string SizeIcon
    {
        get
        {
            if (AverageDiameterKm < 0.005f) return "car_icon.png"; // Very small
            if (AverageDiameterKm < 0.05f) return "bus_icon.png"; // Small
            if (AverageDiameterKm < 0.5f) return "house_icon.png"; // Medium
            if (AverageDiameterKm < 1.0f) return "skyscraper_icon.png"; // Large
            if (AverageDiameterKm < 2.0f) return "village_icon.png"; // Larger
            return "city_icon.png"; // Very large
        }
    }

    public int SizeIconWidth
    {
        get
        {
            // Scale for smaller icons, larger multipliers for small asteroids
            if (SizeIcon == "car_icon.png")
                return (int)(AverageDiameterKm * 2000); // Large scaling for small sizes like a car
            if (SizeIcon == "bus_icon.png")
                return (int)(AverageDiameterKm * 1000); // Slightly smaller scaling for bus
            if (SizeIcon == "house_icon.png")
                return (int)(AverageDiameterKm * 500);  // Scaling for medium sizes like a house
            if (SizeIcon == "skyscraper_icon.png")
                return (int)(AverageDiameterKm * 250);  // Scaling for larger sizes like a skyscraper
            if (SizeIcon == "village_icon.png")
                return (int)(AverageDiameterKm * 125);  // Scaling for even larger sizes
            return (int)(AverageDiameterKm * 50);  // Very large asteroids, city size, scaled smaller
        }
    }

    public int SizeIconHeight
    {
        get
        {
            // Keep the height scaling in sync with the width
            return SizeIconWidth; // Same multiplier logic for both width and height
        }
    }

    public double MissDistanceScaledPosition
    {
        get
        {
            // Scale to the bar's range: Earth at 0, Moon at 1 (e.g., Moon is 384,400 km away)
            const double moonDistanceKm = 384400;
            return (MissDistanceKm / moonDistanceKm) * 300; // Scale to UI width of 300
        }
    }
    public double MissDistanceOffset
    {
        get
        {
            // We use logarithmic scaling to handle all ranges of MissDistanceKm
            const double minDistance = 1000000;  // For example, 1 million km (lower bound)
            const double maxDistance = 20000000; // For example, 20 million km (upper bound)
            const double maxOffset = 150; // Max offset in pixels

            // Apply logarithmic scaling to compress the range of values
            double logMin = Math.Log(minDistance);  // Log of the minimum value
            double logMax = Math.Log(maxDistance); // Log of the maximum value
            double logDistance = Math.Log(MissDistanceKm); // Log of the current value

            // Normalize the log distance to the range [0, 1] based on the log of min and max distances
            double normalizedDistance = (logDistance - logMin) / (logMax - logMin);

            // Ensure that the result stays within a range from 0 to maxOffset (150)
            return Math.Max(0, Math.Min(normalizedDistance * maxOffset, maxOffset));
        }
    }
}
