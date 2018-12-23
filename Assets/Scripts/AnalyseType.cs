using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnalyseCards {
    class AnalyseType {
        public TypeInfo GetTypeInfo(byte[] byteDatas) {
            if (byteDatas == null || byteDatas.Length < 1) {
                return typeInfo;
            }
            Utility.PrepareDatas(byteDatas, out datas);
            onlyTypeInfo = Utility.GetOnlyTypeInfo(datas);
            GetSingleOrPairInfo();
            GetThreeInfo();
            GetBombInfo();
            GetRocketInfo();
            GetSequenceInfo(CardType.sequence);
            GetSequenceInfo(CardType.sequencePair);
            GetSequenceInfo(CardType.sequenceThree);
            return typeInfo;
        }

        SortedDictionary<CardValue, List<byte>> datas;
        Dictionary<CardType, List<CardValue>> onlyTypeInfo;
        TypeInfo typeInfo = new TypeInfo();

        void GetSingleOrPairInfo() {
            if (datas.Count == 1) {
                CardType type = CardType.none;
                if (onlyTypeInfo[CardType.single].Count == ConstData.singleRequireCount) {
                    type = CardType.single;
                } else if (onlyTypeInfo[CardType.pair].Count == ConstData.pairRequireCount) {
                    type = CardType.pair;
                }
                typeInfo.type = type;
                typeInfo.mainValue[type][0] = onlyTypeInfo[type][0];
            }
        }
        void GetThreeInfo() {
            CardType type = CardType.none;
            if (onlyTypeInfo[CardType.three].Count == 1) {
                if (datas.Count == 1) {
                    type = CardType.three;
                } else if (datas.Count == 2) {
                    if (onlyTypeInfo[CardType.single].Count == 1) {
                        type = CardType.threeSingle;
                        typeInfo.postfix.Add(onlyTypeInfo[CardType.single][0]);
                    } else if (onlyTypeInfo[CardType.pair].Count == 1) {
                        type = CardType.threePair;
                        typeInfo.postfix.Add(onlyTypeInfo[CardType.pair][0]);
                    }
                }
                typeInfo.type = type;
                typeInfo.mainValue[type][0] = onlyTypeInfo[CardType.three][0];
            }
        }
        void GetBombInfo() {
            if (typeOnlyInfo.bombKeys.Count == 1) {
                typeInfo.mainValue[0] = typeOnlyInfo.bombKeys[0];

                if (datas.Count == 1) {
                    typeInfo.type = CardType.bomb;
                }
                if (datas.Count == 3) {
                    if (typeOnlyInfo.singleKeys.Count == 2) {
                        typeInfo.type = CardType.bombSingle;
                        typeInfo.prefixValue[0] = typeOnlyInfo.singleKeys[0];
                        typeInfo.prefixValue[1] = typeOnlyInfo.singleKeys[1];
                    }
                    if (typeOnlyInfo.pairKeys.Count == 2) {
                        typeInfo.type = CardType.bombPair;
                        typeInfo.prefixValue[0] = typeOnlyInfo.pairKeys[0];
                        typeInfo.prefixValue[1] = typeOnlyInfo.pairKeys[1];
                    }
                }
            }
        }

        void GetRocketInfo() {
            if (datas.Count == 2 &&
            datas.ContainsKey(CardValue.blackJoker) &&
                datas.ContainsKey(CardValue.redJoker)) {
                typeInfo.type = CardType.rocket;
            }
        }

        void GetSequenceInfo(CardType type) {
            CardValue sequenceStart, sequenceEnd;
            bool isHaveSequence = false;
            Utility.GetSequenceInfo(datas, type, ConstData.minCardValue, out sequenceStart, out sequenceEnd);

            if (isHaveSequence) {
                typeInfo.mainValue[0] = sequenceStart;
                typeInfo.mainValue[1] = sequenceEnd;
                byte count = (byte)(Math.Abs(sequenceEnd - sequenceStart) + 1);
                if (type == CardType.sequenceThree && count != datas.Count) {
                    if (typeOnlyInfo.singleKeys.Count == count && datas.Count == count * 2) {
                        for (byte i = 0; i < typeOnlyInfo.singleKeys.Count; i++) {
                            typeInfo.prefixValue[i] = typeOnlyInfo.singleKeys[i];
                        }
                        typeInfo.type = CardType.sequenceThreeSingle;
                    }
                    if (typeOnlyInfo.pairKeys.Count == count && datas.Count == count * 2) {
                        for (byte i = 0; i < typeOnlyInfo.pairKeys.Count; i++) {
                            typeInfo.prefixValue[i] = typeOnlyInfo.pairKeys[i];
                        }
                        typeInfo.type = CardType.sequenceThreePair;
                    }
                } else {
                    typeInfo.type = type;
                }
            }
        }

    }

}