{
  "bitcoreNode": {
    "services": {
      "api": {
        "wallets": {
          "allowCreationBeforeCompleteSync": true
        }
      }
    },
    "chains": {
      "DFI": {
        "mainnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "seed.defichain.io",
              "port": 8555
            },{
              "host": "seed.mydeficha.in",
              "port": 8555
            }
          ]
        },
        "testnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "defichain_testnet",
              "port": 18555
            },{
              "host": "testnet-seed.defichain.io",
              "port": 18555
            }
          ]
        }
      }
    }
  }
}