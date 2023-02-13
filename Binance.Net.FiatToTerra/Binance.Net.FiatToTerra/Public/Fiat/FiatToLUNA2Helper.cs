﻿using Binance.FiatToTerra.Internal.Clients;
using Binance.FiatToTerra.Internal;
using Binance.FiatToTerra.Models.Enums;
using System.Threading.Tasks;
using Terra.Microsoft.Keys;

namespace Binance.FiatToTerra.Public.Fiat
{
    public class FiatToLUNA2Helper 
    {
        private readonly BinanceManager manager;
        private readonly BinanceLCD binanceRelay;
        public readonly string apiKey;
        public string customerTerraAddress;

        public FiatToLUNA2Helper(
            string apiKey,
            string apiSecret,
            StableCoins stableCoin,
            BinanceEnvironment env = BinanceEnvironment.TestNet)
        {
            manager = new BinanceManager().InitializeBinanceKernel(apiKey, apiSecret, TerraCoin.LUNA, stableCoin, env); // Initialize Kernel Processor

            this.apiKey = apiKey;
            this.binanceRelay = manager.GetBinanceLCDAccess();
        }

        /// <summary>
        /// Configures the users's TerraChain access & Customer Address
        /// </summary>
        /// <param name="recoveryWallet"></param>
        /// <returns></returns>
        public FiatToLUNA2Helper ConfigureOnChainTerraWallet(string recoveryWallet)
        {
            customerTerraAddress = new MnemonicKey(recoveryWallet).AccAddress;
            return this;
        }

        public async Task<string> ExecuteSwapForLUNA_WithMarketPrice(decimal amount)
        {
            return await this.binanceRelay.swaps.ExecuteBuySwapForAsset(amount);
        }

        public async Task<string> ExecuteSwapForLUNA_WithLimit(decimal amount, decimal priceLimit)
        {
            return await this.binanceRelay.swaps.ExecuteBuySwapForAssetWithLimit(amount, priceLimit);
        }

        /// <summary>
        /// Executes an Withdrawal (Asset Transfer) from the customer's Binance wallet to their TerraStation Wallet
        /// </summary>
        /// <param name="LUNAAmount">Amount of LUNA to transfer</param>
        /// <param name="memo">An optional memo (description) of the withdrawal</param>
        /// <returns></returns>
        public async Task<string> TransferLUNAToTerraStation(decimal LUNAAmount, string memo = "Binance-OnChainTransfer-TerraMystics")
        {
            return await this.binanceRelay.withdrawals.ExecuteWithdrawalProcessForTerra(this.customerTerraAddress, LUNAAmount, memo);
        }
        public async Task<decimal> GetCurrentMarketPriceForUSD()
        {
            var config = manager.GetBinanceConfiguration();
            return await this.binanceRelay.marketing.GetCurrentMarketPriceForTerra(config.Terra, config.Stable);
        }
    }
}
