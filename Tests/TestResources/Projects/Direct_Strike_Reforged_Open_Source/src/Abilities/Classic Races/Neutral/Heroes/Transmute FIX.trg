{
  "Id": 50331909,
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
            "ParamType": 2,
            "value": "PlayerUnitEventSpellEffect"
          }
        ],
        "value": "TriggerRegisterAnyUnitEventBJ"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetSpellAbilityId"
          },
          {
            "ParamType": 2,
            "value": "OperatorEqualENE"
          },
          {
            "ParamType": 5,
            "value": "ANtm"
          }
        ],
        "value": "OperatorCompareAbilityId"
      }
    }
  ],
  "Actions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "call AdjustPlayerStateBJ(R2I(GetUnitGoldCost(GetUnitTypeId(GetSpellTargetUnit())) * 0.5), Player(GetConvertedPlayerId(GetOwningPlayer(GetTriggerUnit())) - 7), PLAYER_STATE_RESOURCE_GOLD)"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "set udg_Integer = R2I(GetUnitGoldCost(GetUnitTypeId(GetSpellTargetUnit())) * 0.5)"
          }
        ],
        "value": "CustomScriptCode"
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
            "VariableId": 100663297,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "0"
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
                "ParamType": 1,
                "parameters": [],
                "value": "GetSpellTargetUnit"
              }
            ],
            "value": "GetUnitLoc"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "if(GetLocalPlayer() == GetOwningPlayer(GetTriggerUnit())) then"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "+"
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663312,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "I2S"
              }
            ],
            "value": "OperatorString"
          },
          {
            "ParamType": 3,
            "VariableId": 100663297,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "0"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          },
          {
            "ParamType": 5,
            "value": "0"
          },
          {
            "ParamType": 5,
            "value": "14.00"
          },
          {
            "ParamType": 5,
            "value": "100"
          },
          {
            "ParamType": 5,
            "value": "100"
          },
          {
            "ParamType": 5,
            "value": "0.00"
          },
          {
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "CreateTextTagLocBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedTextTag"
          },
          {
            "ParamType": 2,
            "value": "EnableDisableDisable"
          }
        ],
        "value": "SetTextTagPermanentBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedTextTag"
          },
          {
            "ParamType": 5,
            "value": "4.00"
          }
        ],
        "value": "SetTextTagLifespanBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedTextTag"
          },
          {
            "ParamType": 5,
            "value": "16.00"
          },
          {
            "ParamType": 5,
            "value": "90"
          }
        ],
        "value": "SetTextTagVelocityBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "endif"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "call RemoveLocation(udg_Point)"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "set udg_TotalGoldEarned[GetConvertedPlayerId(GetOwningPlayer(GetTriggerUnit())) - 6] = udg_TotalGoldEarned[GetConvertedPlayerId(GetOwningPlayer(GetTriggerUnit())) - 6] + R2I(GetUnitGoldCost(GetUnitTypeId(GetSpellTargetUnit())) * 0.5)"
          }
        ],
        "value": "CustomScriptCode"
      }
    }
  ]
}