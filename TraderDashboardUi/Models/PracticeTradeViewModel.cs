namespace TraderDashboardUi.Models
{
    public class PracticeTradeViewModel
    {

        public string Instrument { get; set; }

        public string Strategy { get; set; }

        public bool isRunning { get; set; } = false;

        public int elapsedTime { get; set; } = 0;
    }
}
