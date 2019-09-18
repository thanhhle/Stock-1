using System;
using System.Collections.Generic;
using System.IO;

namespace Problem_2
{
    public class StockBroker
    {
        private string _brokerName;
        private List<Stock> _stocks;

        private readonly object _locker = new object();

        // Path of the textfile to save the stock's information when the threshold is reached
        private readonly string _docPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));

        public StockBroker(string brokerName)
        {
            _brokerName = brokerName;
            _stocks = new List<Stock>();
        }

        public void AddStock(Stock stock)
        {
            // Add this stock to the list of stocks controlled by the stock broker
            _stocks.Add(stock);

            // Subscribe to the events
            stock.StockEvent += Notify;
            stock.StockEventData += WriteToFile;
    
        }

        // Output to the console the broker's name and the stock's name, initial value, current value, number of changes in value, and time when the threshold is reached
        private void Notify(string stockName, int initialValue, int currentValue, int numOfChanges, DateTime currentTime)
        {
            Console.WriteLine(_brokerName.PadRight(20) + stockName.PadRight(20) + initialValue.ToString().PadRight(20) + currentValue.ToString().PadRight(20) + numOfChanges.ToString().PadRight(20) + currentTime);
        }

        // Output to a textfile the broker's name and the stock's name, initial value, current value, number of changes in value, and time when the threshold is reached
        private void WriteToFile(object sender, EventData e)
        {
            try
            {
                // Wait for the resource to be free
                lock (_locker)
                {
                    using (FileStream file = new FileStream(Path.Combine(_docPath, "output.txt"), FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (StreamWriter outputFile = new StreamWriter(file))
                    {
                        outputFile.WriteLine(_brokerName.PadRight(20) + e.StockName.PadRight(20) + e.InitialValue.ToString().PadRight(20) + e.CurrentValue.ToString().PadRight(20) + e.NumOfChanges.ToString().PadRight(20) + e.CurrentTime);
                    }
                }
            }
            catch (IOException) { }
        }
    }
}