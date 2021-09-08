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
            var q = new Queue<string>();
            q.Enqueue(null);

            var masterNodeList = new List<Masternode>();

            while (q.Count > 0)
            {
                var q2 = q.Dequeue();
                var masterNodePage = await LoadMasterNode(network, q2);

                foreach (var mn in masterNodePage.Data)
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

                if (masterNodePage.Page != null && !String.IsNullOrEmpty(masterNodePage.Page.Next))
                {
                    q.Enqueue(masterNodePage.Page.Next);
                }
            }
            return masterNodeList;
        }

        private async Task<OceanMasterNode> LoadMasterNode(string network, string nextPage)
        {
            var url = $"{OceanUrl}/v0/{network}/masternodes";
            if (!String.IsNullOrEmpty(nextPage))
            {
                url += $"?next={nextPage}";
            }

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OceanMasterNode>(data);
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
