{
  "Id": 50331697,
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
  "Conditions": [],
  "Actions": [
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
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetAttacker"
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
                "value": "edot"
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
                "ParamType": 3,
                "VariableId": 100663495,
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
                            "value": "GetAttacker"
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
                "value": "GetAttacker"
              },
              {
                "ParamType": 2,
                "value": "UnitOrderCyclone"
              },
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetTriggerUnit"
              }
            ],
            "value": "IssueTargetOrder"
          }
        }
      ],
      "Else": [
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
                    "value": "edot"
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
                    "ParamType": 3,
                    "VariableId": 100663495,
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
                    "value": "UnitOrderCyclone"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetAttacker"
                  }
                ],
                "value": "IssueTargetOrder"
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
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [],
        "value": "IfThenElseMultiple"
      }
    }
  ]
}