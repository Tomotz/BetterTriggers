{
  "Id": 50331725,
  "Comment": "",
  "IsScript": false,
  "RunOnMapInit": false,
  "Script": "",
  "Events": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "0.40"
          }
        ],
        "value": "TriggerRegisterTimerEventPeriodic"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "ElementType": 5,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "2"
              },
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetEnumPlayer"
              },
              {
                "ParamType": 2,
                "value": "PlayerStateGold"
              }
            ],
            "value": "AdjustPlayerStateBJ"
          }
        },
        {
          "ElementType": 9,
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663483,
                "arrayIndexValues": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [],
                        "value": "GetEnumPlayer"
                      }
                    ],
                    "value": "GetConvertedPlayerId"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": null
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663483,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 2,
                    "value": "OperatorAdd"
                  },
                  {
                    "ParamType": 5,
                    "value": "2"
                  }
                ],
                "value": "OperatorInt"
              }
            ],
            "value": "SetVariable"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetPlayersAll"
          }
        ],
        "value": "ForForceMultiple"
      }
    }
  ]
}