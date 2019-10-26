using System;
using System.Threading;
using System.Threading.Tasks;

namespace Problem_2
{
    public class Stock
    {
        // Declare delegate
        public delegate void StockNotification(string stockName, int initialValue, int currentValue, int numOfChanges, DateTime currentTime);
        public event StockNotification StockEvent;

        // Declare .Net EventHandler
        public EventHandler<EventData> StockEventData;

        public string StockName { get; }
        public int InitialValue { get; set; }
        public int CurrentValue { get; set; }
        public int MaxChange { get; }
        public int Threshold { get; }
        public int NumOfChanges { get; set; }

        private readonly Thread _thread;


        public Stock(string stockName, int startingValue, int maxChange, int threshold)
        {
            StockName = stockName;
            InitialValue = startingValue;
            CurrentValue = InitialValue;
            MaxChange = maxChange;
            Threshold = threshold;

            // Thread causes the stock's value to be modified every 500 milliseconds
            _thread = new Thread(Activate);
            _thread.Start();
        }

        // Change the stock's value every 500 milliseconds
        private void Activate()
        {
            for (int i = 0; i < 50; i++)
            {
                ChangeStockValue();
                Thread.Sleep(500);
            }
        }

        // Change the stock value and invoke event to notify stock brokers when the threshold is reach
        private void ChangeStockValue()
        {
            // Generate a random number to within a range that stock can change every time unit and add it to the current stock's value
            Random rand = new Random();
            CurrentValue += rand.Next(-MaxChange, MaxChange);

            // Increase the number of changes in value by 1
            NumOfChanges++;

            // Check if the threshold is reached
            if (Math.Abs(CurrentValue - InitialValue) >= Threshold)
            {
                // Raise the events
                Parallel.Invoke(() => StockEvent?.Invoke(StockName, InitialValue, CurrentValue, NumOfChanges, DateTime.Now),
                                () => StockEventData?.Invoke(this, new EventData(StockName, InitialValue, CurrentValue, NumOfChanges, DateTime.Now)));
            }
        }

    }


    public class EventData : EventArgs
    {
        public string StockName { get; }
        public int InitialValue { get; }
        public int CurrentValue { get; set; }
        public int NumOfChanges { get; set; }
        public DateTime CurrentTime { get; }

        public EventData(string stockName, int initialValue, int currentValue, int numOfChanges, DateTime currentTime)
        {
            StockName = stockName;
            InitialValue = initialValue;
            CurrentValue = currentValue;
            CurrentTime = currentTime;
            NumOfChanges = numOfChanges;
        }
    }
}
