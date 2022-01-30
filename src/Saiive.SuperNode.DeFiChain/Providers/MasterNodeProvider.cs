using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class MasterNodeHelper : BaseDeFiChainProvider
    {
        public MasterNodeHelper(ILogger<MasterNodeHelper> logger, IConfiguration config) : base(logger, config)
        {
        }

        internal async Task<List<Masternode>> LoadAll(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/masternodes/list", network, async () =>
            {
                var url = $"{OceanUrl}/{ApiVersion}/{network}/masternodes";
                var getAllMasternodes = await Helper.LoadAllFromPagedRequest<OceanMasterNodeData>(url);


                var masterNodeList = new List<Masternode>();
                foreach (var mn in getAllMasternodes)
                {
                    masterNodeList.Add(new Masternode
                    {
                        Id = mn.Id,
                        CreationHeight = mn.Creation.Height,
                        MintedBlocks = mn.MintedBlocks,
                        OperatorAuthAddress = mn.Operator.Address,
                        OwnerAuthAddress = mn.Owner.Address,
                        ResignHeight = mn.Resign?.Height ?? -1,
                        ResignTx = mn.Resign?.Tx,
                        State = mn.State
                    });
                }
                return masterNodeList;
            }, null);
        }
    }

    internal class MasterNodeProvider : BaseDeFiChainProvider, IMasterNodeProvider
    {
        public MasterNodeProvider(ILogger<MasterNodeProvider> logger, IConfiguration config, IMasterNodeCache cache) : base(logger, config)
        {
            Cache = cache;
        }

        public IMasterNodeCache Cache { get; }

        public async Task<List<Masternode>> ListActiveMasternodes(string network)
        {
            var mns = await Cache.GetMasterNodes(network);

            return mns.Where(a => a.State == "ENABLED").ToList();
        }

        public Task<List<Masternode>> ListMasternodes(string network)
        {
            return Cache.GetMasterNodes(network);
        }

        
    }
}
