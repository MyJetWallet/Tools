using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Newtonsoft.Json;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.AzureStorage;

namespace MigrationFromBinance
{
    class Program
    {
        private static string _askConnectionString = "";

        private static string _bidConnectionString = "";

        static async Task Main(string[] args)
        {
            var storage = new CandlesPersistentAzureStorage(
                () => _bidConnectionString,
                () => _askConnectionString);

            //await LoadInstrument("USDTUSD", "BUSDUSDT", storage, 4, true);
            
            await LoadInstrument("ALGOBTC", "ALGOBTC", storage, 8);
            await LoadInstrument("ALGOUSD", "ALGOBUSD", storage, 4);
            
            await LoadInstrument("BCHBTC", "BCHBTC", storage, 6);
            await LoadInstrument("BCHUSD", "BCHBUSD", storage, 2);
            
            await LoadInstrument("BTCEUR", "BTCEUR", storage, 2);
            await LoadInstrument("BTCUSD", "BTCBUSD", storage, 2);
            
            await LoadInstrument("DASHBTC", "DASHBTC", storage, 6);
            await LoadInstrument("DASHUSD", "DASHBUSD", storage, 2);
            
            await LoadInstrument("EOSBTC", "EOSBTC", storage, 8);
            await LoadInstrument("EOSUSD", "EOSBUSD", storage, 2);
            
            await LoadInstrument("ETHBTC", "ETHBTC", storage, 8);
            await LoadInstrument("ETHEUR", "ETHEUR", storage, 2);
            await LoadInstrument("ETHUSD", "ETHBUSD", storage, 2);
            
            await LoadInstrument("LTCBTC", "LTCBTC", storage, 8);
            await LoadInstrument("LTCUSD", "LTCBUSD", storage, 4);
            
            await LoadInstrument("TRXBTC", "TRXBTC", storage, 8);
            await LoadInstrument("TRXUSD", "TRXBUSD", storage, 6);
            
            await LoadInstrument("XLMBTC", "XLMBTC", storage, 8);
            await LoadInstrument("XLMUSD", "XLMBUSD", storage, 5);
            
            await LoadInstrument("XRPBTC", "XRPBTC", storage, 8);
            await LoadInstrument("XRPUSD", "XRPBUSD", storage, 6);
            
            await LoadInstrument("ZECBTC", "ZECBTC", storage, 8);
            await LoadInstrument("ZECUSD", "ZECBUSD", storage, 2);
            
            
            
            

            Console.WriteLine();
            Console.WriteLine("End of loading");
            Console.ReadLine();






        }

        private static async Task LoadInstrument(string symbol, string source, CandlesPersistentAzureStorage storage,
            int digits, bool isRevert = false)
        {
            Console.WriteLine();
            Console.WriteLine($"========= Load {symbol} from {source} =======");

            var candle = CandleType.Minute;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert);

            candle = CandleType.Hour;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert);

            candle = CandleType.Day;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert);

            candle = CandleType.Month;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert);
        }

        private static async Task LoadInterval(CandleType candle, string source, CandlesPersistentAzureStorage storage,
            string symbol, int digits, bool isRevert)
        {
            Console.WriteLine();
            Console.WriteLine($"----- {candle.ToString()} --------");

            var interval = "";
            switch (candle)
            {
                case CandleType.Minute:
                    interval = "1m";
                    break;
                case CandleType.Hour:
                    interval = "1h";
                    break;
                case CandleType.Day:
                    interval = "1d";
                    break;
                case CandleType.Month:
                    interval = "1M";
                    break;
            }

            var data = await GetCandles(source, 1000, interval, 0, isRevert, digits);

            var count = 0;
            while (data.Any() && count < 45000)
            //while (data.Any() && count < 3000)
            {
                Console.Write($"Read {data.Count} items from Binance ... ");
                await storage.BulkSave(symbol, true, digits, candle, data);
                await storage.BulkSave(symbol, false, digits, candle, data);
                Console.WriteLine($"Save {data.Count} items");

                var lastTime = data.Min(e => e.DateTime).UnixTime();
                count += data.Count;

                data = await GetCandles(source, 1000, interval, lastTime - 1, isRevert, digits);
            }
        }


        private static HttpClient _http = new HttpClient();

        public static async Task<List<BinanceCandle>> GetCandles(string symbol, int limit, string interval,
            long endtime, bool isRevert, int digit)
        {
            var url = "https://api.binance.com/api/v3/klines";

            if (endtime > 0)
                url += $"?symbol={symbol}&limit={limit}&interval={interval}&endTime={endtime}";
            else
                url += $"?symbol={symbol}&limit={limit}&interval={interval}";

            //Console.WriteLine(url);

            var json = await _http.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<string[][]>(json);

            return data.Select(e => BinanceCandle.Create(e, isRevert, digit)).ToList();



            //https://api.binance.com/api/v3/klines?symbol=BTCUSD&limit=10&interval=1m
            //https://api.binance.com/api/v3/klines?symbol=BTCBUSD&limit=2&interval=1m&endTime=1629381839999

        }
    }

    public class BinanceCandle: ICandleModel
    {
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public static BinanceCandle Create(string[] data, bool isRevert, int digits)
        {
            if (data.Length < 5)
                throw new Exception($"Cannot parse data:{JsonConvert.SerializeObject(data)} ");

            var ts = long.Parse(data[0]);



            var candle = new BinanceCandle()
            {
                DateTime = ts.UnixTimeToDateTime(),
                Open = double.Parse(data[1]),
                High = double.Parse(data[2]),
                Low = double.Parse(data[3]),
                Close = double.Parse(data[4])
            };

            if (isRevert)
            {
                candle.Open = Math.Round(1 / candle.Open, digits);
                candle.Close = Math.Round(1 / candle.Close, digits);
                var low = Math.Round(1 / candle.High, digits);
                var high = Math.Round(1 / candle.Low, digits);
                candle.High = high;
                candle.Low = low;
            }

            return candle;
        }
    }
}
