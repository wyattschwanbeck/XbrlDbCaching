using System;
using Xbrl.Helpers;
using System.Collections.Generic;

namespace Xbrl.FinancialStatement
{
    public static class FinancialStatementHelper
    {
        public static FinancialStatement CreateFinancialStatement(this XbrlInstanceDocument doc)
        {
            FinancialStatement ToReturn = new FinancialStatement();

            //Get the context reference to focus on
            XbrlContext focus_context;
            try
            {
                focus_context = doc.FindNormalPeriodPrimaryContext();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            #region "Contextual (misc) info"

            //Period start and end
            ToReturn.PeriodStart = focus_context.StartDate;
            ToReturn.PeriodEnd = focus_context.EndDate;

            //Common Stock Shares Outstanding
            try
            {
                ToReturn.CommonStockSharesOutstanding = doc.GetValueFromPriorities(doc.PrimaryPeriodContextId, "CommonStockSharesOutstanding", "EntityCommonStockSharesOutstanding", "WeightedAverageNumberOfDilutedSharesOutstanding").ValueAsLong();
            }
            catch
            {
                ToReturn.CommonStockSharesOutstanding = null;
            }

            #endregion

            #region "Income Statement"
            //Revenue
            try
            {
                ToReturn.Revenue = doc.GetValueFromPriorities(focus_context.Id, "Revenue", "Revenues", "RevenueFromContractWithCustomerExcludingAssessedTax", "RevenueFromContractWithCustomerIncludingAssessedTax", "RevenueFromContractWithCustomerBeforeReimbursementsExcludingAssessedTax", "SalesRevenueNet", "SalesRevenueGoodsNet", "TotalRevenuesAndOtherIncome").ValueAsFloat();
            }
            catch
            {
                ToReturn.Revenue = null;
            }

            //Net income
            try
            {
                ToReturn.NetIncome = doc.GetValueFromPriorities(focus_context.Id, "NetIncomeLoss", "IncomeLossFromContinuingOperationsIncludingPortionAttributableToNoncontrollingInterest", "ProfitLoss").ValueAsFloat();
            }
            catch
            {
                ToReturn.NetIncome = null;
            }

            //Operating Income
            try
            {
                ToReturn.OperatingIncome = doc.GetValueFromPriorities(focus_context.Id, "OperatingIncomeLoss", "IncomeLossFromContinuingOperationsBeforeIncomeTaxesExtraordinaryItemsNoncontrollingInterest").ValueAsFloat();
            }
            catch
            {
                ToReturn.OperatingIncome = null;
            }

            //Selling general and administrative expense
            try
            {
                ToReturn.SellingGeneralAndAdministrativeExpense = doc.GetValueFromPriorities(focus_context.Id, "SellingGeneralAndAdministrativeExpense").ValueAsFloat();
            }
            catch
            {
                ToReturn.SellingGeneralAndAdministrativeExpense = null;
            }

            //Research and development expense
            try
            {
                ToReturn.ResearchAndDevelopmentExpense = doc.GetValueFromPriorities(focus_context.Id, "ResearchAndDevelopmentExpense", "ResearchAndDevelopmentExpenseExcludingAcquiredInProcessCost").ValueAsFloat();
            }
            catch
            {
                ToReturn.ResearchAndDevelopmentExpense = null;
            }


            #endregion

            #region "Balance Sheet"

            //Assets
            try
            {
                ToReturn.Assets = doc.GetValueFromPriorities(focus_context.Id, "Assets").ValueAsFloat();
            }
            catch
            {
                ToReturn.Assets = null;
            }

            //Liabilities
            try
            {
                ToReturn.Liabilities = doc.GetValueFromPriorities(focus_context.Id, "Liabilities").ValueAsFloat();
            }
            catch
            {
                ToReturn.Liabilities = null;
            }

            //Equity
            try
            {
                ToReturn.Equity = doc.GetValueFromPriorities(focus_context.Id, "Equity", "StockholdersEquity", "StockholdersEquityIncludingPortionAttributableToNoncontrollingInterest").ValueAsFloat();
            }
            catch
            {
                ToReturn.Equity = null;
            }

            //If only liabilities or equity were able to be found, fill in the rest
            if (ToReturn.Assets != null)
            {
                if (ToReturn.Liabilities == null && ToReturn.Equity != null)
                {
                    ToReturn.Liabilities = ToReturn.Assets - ToReturn.Equity;
                }
                else if (ToReturn.Equity == null && ToReturn.Liabilities != null)
                {
                    ToReturn.Equity = ToReturn.Assets - ToReturn.Liabilities;
                }
            }
            else
            {
                if (ToReturn.Liabilities != null && ToReturn.Equity != null)
                {
                    ToReturn.Assets = ToReturn.Liabilities + ToReturn.Equity;
                }
            }

            //Cash
            try
            {
                ToReturn.Cash = doc.GetValueFromPriorities(focus_context.Id, "CashAndCashEquivalents", "CashAndCashEquivalentsAtCarryingValue", "CashCashEquivalentsRestrictedCashAndRestrictedCashEquivalents", "CashCashEquivalentsRestrictedCashAndRestrictedCashEquivalents").ValueAsFloat();
            }
            catch
            {
                ToReturn.Cash = null;
            }

            //Current Assets
            try
            {
                ToReturn.CurrentAssets = doc.GetValueFromPriorities(focus_context.Id, "AssetsCurrent").ValueAsFloat();
            }
            catch
            {
                ToReturn.CurrentAssets = null;
            }

            //Current Libilities
            try
            {
                ToReturn.CurrentLiabilities = doc.GetValueFromPriorities(focus_context.Id, "LiabilitiesCurrent").ValueAsFloat();
            }
            catch
            {
                ToReturn.CurrentLiabilities = null;
            }

            //Retained Earnings
            try
            {
                ToReturn.RetainedEarnings = doc.GetValueFromPriorities(focus_context.Id, "RetainedEarningsAccumulatedDeficit").ValueAsFloat();
            }
            catch
            {
                ToReturn.RetainedEarnings = null;
            }

            try
            {
                ToReturn.ShortTermDebtInterestRate = doc.GetValueFromPriorities(focus_context.Id, "ShortTermDebtWeightedAverageInterestRate").ValueAsFloat();
            }
            catch
            {
                ToReturn.ShortTermDebtInterestRate = null;
            }
            try
            {
                ToReturn.ShortTermDebt = doc.GetValueFromPriorities(focus_context.Id, "ShortTermDebt").ValueAsFloat();
            }
            catch
            {
                ToReturn.ShortTermDebt = null;
            }

            try
            {
                ToReturn.LongTermDebtInterestRate = doc.GetValueFromPriorities(focus_context.Id, "LongtermDebtWeightedAverageInterestRate").ValueAsFloat();
            }
            catch
            {
                ToReturn.LongTermDebtInterestRate = null;
            }
            try
            {
                ToReturn.LongTermDebt = doc.GetValueFromPriorities(focus_context.Id, "LongTermDebt").ValueAsFloat();
            }
            catch
            {
                ToReturn.LongTermDebt = null;
            }

            try
            {
                ToReturn.OperationsIncomeTaxRate = doc.GetValueFromPriorities(focus_context.Id, "EffectiveIncomeTaxRateContinuingOperations").ValueAsFloat();
            }
            catch
            {
                ToReturn.OperationsIncomeTaxRate = null;
            }


            #endregion

            #region "Cash Flows"

            //Operating Cash Flows
            try
            {
                ToReturn.OperatingCashFlows = doc.GetValueFromPriorities(focus_context.Id, "NetCashProvidedByUsedInOperatingActivities", "NetCashProvidedByUsedInOperatingActivitiesContinuingOperations").ValueAsFloat();
            }
            catch
            {
                ToReturn.OperatingCashFlows = null;
            }

            //Investing Cash Flows
            try
            {
                ToReturn.InvestingCashFlows = doc.GetValueFromPriorities(focus_context.Id, "NetCashProvidedByUsedInInvestingActivities", "NetCashProvidedByUsedInInvestingActivitiesContinuingOperations").ValueAsFloat();
            }
            catch
            {
                ToReturn.InvestingCashFlows = null;
            }

            //Finance cash flows
            try
            {
                ToReturn.FinancingCashFlows = doc.GetValueFromPriorities(focus_context.Id, "NetCashProvidedByUsedInFinancingActivities", "NetCashProvidedByUsedInFinancingActivitiesContinuingOperations").ValueAsFloat();
            }
            catch
            {
                ToReturn.FinancingCashFlows = null;
            }

            //ProceedsFromIssuanceOfDebt
            try
            {
                ToReturn.ProceedsFromIssuanceOfDebt = doc.GetValueFromPriorities(focus_context.Id, "ProceedsFromIssuanceOfDebt", "ProceedsFromDebtMaturingInMoreThanThreeMonths", "ProceedsFromIssuanceOfLongTermDebt", "ProceedsFromIssuanceOfLongTermDebtAndCapitalSecuritiesNet").ValueAsFloat();
            }
            catch
            {
                ToReturn.ProceedsFromIssuanceOfDebt = null;
            }

            //Payments of debt
            try
            {
                ToReturn.PaymentsOfDebt = doc.GetValueFromPriorities(focus_context.Id, "RepaymentsOfDebt", "RepaymentsOfDebtMaturingInMoreThanThreeMonths", "RepaymentsOfLongTermDebt", "RepaymentsOfLongTermDebtAndCapitalSecurities").ValueAsFloat();
            }
            catch
            {
                ToReturn.PaymentsOfDebt = null;
            }

            //Dividends paid
            try
            {
                ToReturn.DividendsPaid = doc.GetValueFromPriorities(focus_context.Id, "PaymentsOfDividendsCommonStock", "PaymentsOfDividends").ValueAsFloat();
            }
            catch
            {
                ToReturn.DividendsPaid = null;
            }

            #endregion

            #region "StockMetrics"
            try
            {
                ToReturn.PreferredDividends = doc.GetValueFromPriorities(focus_context.Id, "PreferredDividendsNetOfTax", "DividendsPreferredStock").ValueAsFloat();
            }
            catch
            {
                ToReturn.PreferredDividends = 0;
            }

            try
            {
                ToReturn.EarningsPerShareDiluted = doc.GetValueFromPriorities(focus_context.Id, "EarningsPerShareDiluted").ValueAsFloat();
            }
            catch
            {
                ToReturn.EarningsPerShareDiluted= 0;
            }
            try
            {
                ToReturn.EarningsPerShareBasic = doc.GetValueFromPriorities(focus_context.Id, "EarningsPerShareBasic").ValueAsFloat();
            }
            catch
            {
                ToReturn.EarningsPerShareBasic = 0;
            }
            //
            #endregion




            return ToReturn;
        }
    }
}