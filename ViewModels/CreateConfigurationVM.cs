namespace backend_app.ViewModels
{
    public class CreateConfigurationVM
    {
        public Guid UserId { get; set; }
        public string BuildingType { get; set; }
        public float BuildingCost { get; set; }
        public float ConstructionTime { get; set; }
    }
}
