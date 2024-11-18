namespace NEOTracker.Models
{
    public class Asteroid
    {
        public int Id { get; set; } // Primární klíč
        public string NeoReferenceId { get; set; }
        public string Name { get; set; }
        public string NasaJplUrl { get; set; }
        public double AbsoluteMagnitudeH { get; set; }

        // Estimated diameter (kilometers)
        public double EstimatedDiameterMinKm { get; set; }
        public double EstimatedDiameterMaxKm { get; set; }

        public bool IsPotentiallyHazardous { get; set; }

        // Close approach data
        public string CloseApproachDate { get; set; }
        public string CloseApproachDateFull { get; set; }
        public double RelativeVelocityKmPerSec { get; set; }
        public double MissDistanceKm { get; set; }
        public string OrbitingBody { get; set; }
    }
}
