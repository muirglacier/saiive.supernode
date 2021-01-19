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
              "host": "defichain",
              "port": 8555
            }
          ],
          "rpc": {
            "host": "defichain",
            "port": 8554,
            "username": "${wallet_user}"
            "password": "${wallet_password}"
          }
        }
      }
    }
  }
}