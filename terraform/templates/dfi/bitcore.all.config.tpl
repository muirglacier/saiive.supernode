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
              "host": "defichain_mainnet",
              "port": 8555
            }
          ],
          "rpc": {
            "host": "defichain_mainnet",
            "port": 8554,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        },
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