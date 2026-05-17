using ClickExpress.BusinessLogic.Functions.User;
using ClickExpress.BusinessLogic.Functions.Product;
using ClickExpress.BusinessLogic.Functions.Order;
using ClickExpress.BusinessLogic.Functions.Cart;
using ClickExpress.BusinessLogic.Functions.Vehicle;
using ClickExpress.BusinessLogic.Functions.Driver;
using ClickExpress.BusinessLogic.Functions.Review;
using ClickExpress.BusinessLogic.Functions.Lead;
using ClickExpress.BusinessLogic.Functions.JobApplication;
using ClickExpress.BusinessLogic.Functions.News;
using ClickExpress.BusinessLogic.Functions.SavedLoad;

namespace ClickExpress.BusinessLogic
{
    public class BusinessLogic
    {
        public UserFlow UserFlow { get; } = new UserFlow();
        public ProductFlow ProductFlow { get; } = new ProductFlow();
        public OrderFlow OrderFlow { get; } = new OrderFlow();
        public CartFlow CartFlow { get; } = new CartFlow();
        public VehicleFlow VehicleFlow { get; } = new VehicleFlow();
        public DriverFlow DriverFlow { get; } = new DriverFlow();
        public ReviewFlow ReviewFlow { get; } = new ReviewFlow();
        public LeadFlow LeadFlow { get; } = new LeadFlow();
        public JobApplicationFlow JobApplicationFlow { get; } = new JobApplicationFlow();
        public NewsFlow NewsFlow { get; } = new NewsFlow();
        public SavedLoadFlow SavedLoadFlow { get; } = new SavedLoadFlow();
    }
}
