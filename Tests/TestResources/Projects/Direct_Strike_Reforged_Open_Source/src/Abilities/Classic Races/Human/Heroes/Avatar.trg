{
  "Id": 50331752,
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
            "value": "PlayerUnitEventAttacked"
          }
        ],
        "value": "TriggerRegisterAnyUnitEventBJ"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [
    {
      "ElementType": 3,
      "Or": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetTriggerUnit"
                  }
                ],
                "value": "GetUnitTypeId"
              },
              {
                "ParamType": 2,
                "value": "OperatorEqualENE"
              },
              {
                "ParamType": 5,
                "value": "Hmkg"
              }
            ],
            "value": "OperatorCompareUnitCode"
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
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetTriggerUnit"
                  }
                ],
                "value": "GetUnitTypeId"
              },
              {
                "ParamType": 2,
                "value": "OperatorEqualENE"
              },
              {
                "ParamType": 5,
                "value": "Hmbr"
              }
            ],
            "value": "OperatorCompareUnitCode"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [],
        "value": "OrMultiple"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663513,
            "arrayIndexValues": [
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [],
                        "value": "GetTriggerUnit"
                      }
                    ],
                    "value": "GetOwningPlayer"
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
            "value": "OperatorEqualENE"
          },
          {
            "ParamType": 5,
            "value": "true"
          }
        ],
        "value": "OperatorCompareBoolean"
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
                "value": "AHav"
              },
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetTriggerUnit"
              }
            ],
            "value": "GetUnitAbilityLevelSwapped"
          },
          {
            "ParamType": 2,
            "value": "OperatorGreater"
          },
          {
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "OperatorCompareInteger"
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
                "ParamType": 1,
                "parameters": [],
                "value": "GetTriggerUnit"
              },
              {
                "ParamType": 5,
                "value": "AHav"
              }
            ],
            "value": "BlzGetUnitAbilityCooldownRemaining"
          },
          {
            "ParamType": 2,
            "value": "OperatorLessEq"
          },
          {
            "ParamType": 5,
            "value": "0.00"
          }
        ],
        "value": "OperatorCompareReal"
      }
    }
  ],
  "Actions": [
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
                "value": "GetAttacker"
              }
            ],
            "value": "GetUnitLoc"
          }
        ],
        "value": "SetVariable"
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
            "VariableId": 100663298,
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
                "ParamType": 5,
                "value": "300.00"
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
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetFilterUnit"
                          },
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [],
                                "value": "GetTriggerUnit"
                              }
                            ],
                            "value": "GetOwningPlayer"
                          }
                        ],
                        "value": "IsUnitEnemy"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorEqualENE"
                      },
                      {
                        "ParamType": 5,
                        "value": "true"
                      }
                    ],
                    "value": "OperatorCompareBoolean"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetFilterUnit"
                          },
                          {
                            "ParamType": 2,
                            "value": "UnitTypeGround"
                          }
                        ],
                        "value": "IsUnitType"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorEqualENE"
                      },
                      {
                        "ParamType": 5,
                        "value": "true"
                      }
                    ],
                    "value": "OperatorCompareBoolean"
                  }
                ],
                "value": "GetBooleanAnd"
              }
            ],
            "value": "GetUnitsInRangeOfLocMatching"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 1,
      "If": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663298,
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
                "value": "CountUnitsInGroup"
              },
              {
                "ParamType": 2,
                "value": "OperatorGreater"
              },
              {
                "ParamType": 5,
                "value": "2"
              }
            ],
            "value": "OperatorCompareInteger"
          }
        }
      ],
      "Then": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetTriggerUnit"
              },
              {
                "ParamType": 2,
                "value": "UnitOrderAvatar"
              }
            ],
            "value": "IssueImmediateOrder"
          }
        }
      ],
      "Else": [],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [],
        "value": "IfThenElseMultiple"
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
            "value": "call DestroyGroup(udg_UnitGroup)"
          }
        ],
        "value": "CustomScriptCode"
      }
    }
  ]
}