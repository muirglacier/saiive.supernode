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
        "testnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "defichain_testnet",
              "port": 18555
            }
          ],
          "rpc": {
            "host": "defichain_testnet",
            "port": 18554,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        }
      }
    }
  }
}
