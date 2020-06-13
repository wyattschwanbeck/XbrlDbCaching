﻿using System;
using Xbrl;
using Xbrl.FinancialStatement;
using System.IO;
using Newtonsoft.Json;
using SecuritiesExchangeCommission.Edgar;

namespace FunctionalTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSingleDocument();
        }

        static void TestSingleDocument()
        {
            string path = "C:\\Users\\tihanewi\\Downloads\\XOM 2010.xml";
            Stream s = System.IO.File.OpenRead(path);
            XbrlInstanceDocument doc = XbrlInstanceDocument.Create(s);
            Console.WriteLine("Period context ref: '" + doc.PrimaryPeriodContextId + "'");
            Console.WriteLine("Instant context ref: '" + doc.PrimaryInstantContextId + "'");
            FinancialStatement fs = doc.CreateFinancialStatement();
            string json = JsonConvert.SerializeObject(fs);
            Console.WriteLine(json);
        }

        static void TestAll10Ks()
        {

            do
            {
                Console.WriteLine("Symbol?");
                string symbol = Console.ReadLine();

                EdgarSearch es = EdgarSearch.CreateAsync(symbol, "10-K").Result;

                foreach (EdgarSearchResult esr in es.Results)
                {
                    if (esr.Filing == "10-K")
                    {
                        if (esr.InteractiveDataUrl != null && esr.InteractiveDataUrl != "")
                        {
                            try
                            {
                                Stream s = esr.DownloadXbrlDocumentAsync().Result;
                                XbrlInstanceDocument doc = XbrlInstanceDocument.Create(s);
                                FinancialStatement fs = doc.CreateFinancialStatement();
                                string rev = "";
                                if (fs.Revenue.HasValue)
                                {
                                    rev = fs.Revenue.Value.ToString("#,##0");
                                }
                                else
                                {
                                    rev = "?";
                                }
                                Console.WriteLine(fs.PeriodEnd.ToShortDateString() + " - " + rev);
                            }
                            catch
                            {
                                Console.WriteLine("Critical error (filed on " + esr.FilingDate.ToShortDateString());
                            }
                        }
                    }
                }
                Console.WriteLine();

            } while (true);

        }
    }
}
